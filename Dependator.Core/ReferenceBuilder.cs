using Dependator.Core.ReferenceDefinitions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dependator.Core
{
    public sealed class ReferenceBuilder
    {
        public void Build(DependencyModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            // Classes
            Parallel.ForEach(model.SolutionSymbols.Where(s => s.TypeKind == TypeKind.Class), typeSymbol =>
            {
                CheckBaseTypeReference(model, typeSymbol);
                CheckInterfaces(model, typeSymbol);
                CheckMethods(model, typeSymbol);


            });

            // Stucts
            Parallel.ForEach(model.SolutionSymbols.Where(s => s.TypeKind == TypeKind.Struct || s.TypeKind == TypeKind.Structure), structSymbol =>
            {
                CheckBaseTypeReference(model, structSymbol);
                CheckInterfaces(model, structSymbol);
                CheckMethods(model, structSymbol);


            });

            // Interfaces
            Parallel.ForEach(model.SolutionSymbols.Where(s => s.TypeKind == TypeKind.Interface), interfaceSymbol =>
            {
                CheckBaseTypeReference(model, interfaceSymbol);
                CheckInterfaces(model, interfaceSymbol);
                CheckMethods(model, interfaceSymbol);


            });

            // fields, properties!!!

        }

        private void CheckBaseTypeReference(DependencyModel model, INamedTypeSymbol symbol)
        {
            if (symbol.BaseType != null)
            {
                Parallel.ForEach(symbol.BaseType.ResolveGeneralizedType(),
                    type => AddReference(model, symbol, type, (f, t) => new BaseTypeReference(f, t)));
            }
        }

        private void CheckInterfaces(DependencyModel model, INamedTypeSymbol symbol)
        {
            if (symbol.Interfaces.Any())
            {
                Parallel.ForEach(symbol.Interfaces.ResolveGeneralizedTypes(),
                    type => AddReference(model, symbol, type, (f, t) => new InterfaceReference(f, t)));
            }
        }

        private void CheckMethods(DependencyModel model, INamedTypeSymbol symbol)
        {
            Parallel.ForEach(symbol.GetMembers().OfType<IMethodSymbol>(), methodSymbol =>
            {
                // Generalization
                if (methodSymbol.IsGenericMethod)
                {
                    Parallel.ForEach(methodSymbol.TypeArguments.OfType<INamedTypeSymbol>().ResolveGeneralizedTypes(),
                        type => AddReference(model, symbol, type, (f, t) => new MethodGenericReference(f, t, methodSymbol)));
                }

                // Parameters
                if (methodSymbol.Parameters.Any())
                {
                    Parallel.ForEach(methodSymbol.Parameters.Select(p => p.Type).OfType<INamedTypeSymbol>().ResolveGeneralizedTypes(),
                        type => AddReference(model, symbol, type, (f, t) => new MethodParameterReference(f, t, methodSymbol)));
                }

                // Return Type
                if (!methodSymbol.ReturnsVoid)
                {
                    var namedType = methodSymbol.ReturnType as INamedTypeSymbol;
                    if (namedType != null)
                    {
                        Parallel.ForEach(namedType.ResolveGeneralizedType(),
                            type => AddReference(model, symbol, type, (f, t) => new MethodResultReference(f, t, methodSymbol)));
                    }

                }

                var compilation = model.GetCompilation(methodSymbol.ContainingType);
                Parallel.ForEach(ReferenceBuilderExtensions.GetMethodBodySymbols(methodSymbol, compilation).OfType<INamedTypeSymbol>(),
                    type => AddReference(model, symbol, type, (f, t) => new MethodBodyReference(f, t, methodSymbol)));

            });
        }


        private static void AddReference(DependencyModel model, INamedTypeSymbol symbolFrom, INamedTypeSymbol symbolTo, Func<ReferenceEndpoint,ReferenceEndpoint,ReferenceDefinition> factory)
        {
            if (model.IsSolutionSymbol(symbolTo))
            {
                var referenceFrom = model.GetOrAddReferenceFrom(symbolFrom);
                var referenceTo = model.GetOrAddReferenceTo(symbolTo);
                model.AddReferenceDefinition(factory(referenceFrom,referenceTo));
            }
        }
    }

    public static class ReferenceBuilderExtensions
    {
        public static IEnumerable<INamedTypeSymbol> ResolveGeneralizedTypes(this IEnumerable<INamedTypeSymbol> namedTypeSymbols)
        {
            return namedTypeSymbols.SelectMany(s => ResolveGeneralizedType(s));
        }

        public static IEnumerable<INamedTypeSymbol> ResolveGeneralizedType(this INamedTypeSymbol namedTypeSymbol)
        {
            if (namedTypeSymbol.IsGenericType && !namedTypeSymbol.IsDefinition)
            {
                foreach (var type in ResolveGeneralizedType(namedTypeSymbol.OriginalDefinition))
                    yield return type;

                foreach (var typeArgument in namedTypeSymbol.TypeArguments.OfType<INamedTypeSymbol>())
                    foreach (var type in ResolveGeneralizedType(typeArgument))
                        yield return type;
            }
            else
            {
                yield return namedTypeSymbol;
            }
        }

        public static IEnumerable<ISymbol> GetMethodBodySymbols(IMethodSymbol methodSymbol, Compilation compilation)
        {
            if (methodSymbol == null)
                throw new ArgumentNullException("methodSymbol");
            if (compilation == null)
                throw new ArgumentNullException("compilation");

            if (!methodSymbol.DeclaringSyntaxReferences.Any())
                return Enumerable.Empty<ISymbol>();

            SyntaxNode syntaxNode = methodSymbol.DeclaringSyntaxReferences.Single().GetSyntax();
            BlockSyntax body;

            switch (methodSymbol.MethodKind)
            {
                case MethodKind.Constructor:
                    body = ((ConstructorDeclarationSyntax)syntaxNode).Body;
                    break;

                case MethodKind.PropertyGet:
                case MethodKind.PropertySet:
                    body = ((AccessorDeclarationSyntax)syntaxNode).Body;
                    break;

                case MethodKind.Ordinary:
                    body = ((MethodDeclarationSyntax)syntaxNode).Body;
                    break;

                case MethodKind.UserDefinedOperator:
                    body = ((OperatorDeclarationSyntax)syntaxNode).Body;
                    break;

                case MethodKind.Conversion:
                    body = ((ConversionOperatorDeclarationSyntax)syntaxNode).Body;
                    break;

                default:
                    throw new NotImplementedException();
            }

            if (body != null)
            {
                SemanticModel model = compilation.GetSemanticModel(body.SyntaxTree); ;
                return body.DescendantNodes().Select(node => model.GetSymbolInfo(node).Symbol ?? model.GetDeclaredSymbol(node)).Where(symbol => symbol != null);
            }
            else
            {
                return Enumerable.Empty<ISymbol>();
            }
        }
    }
}
