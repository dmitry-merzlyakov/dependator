using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dependator.CommandLine
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var result = Parser.Default.ParseArguments<IndirectDependenciesVerb>(args);

                var options = result.MapResult(
                        (IndirectDependenciesVerb opts) => (BaseOptions)opts,
                        _ => new EmptyVerb());

                options.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine("RUNTIME ERROR(S):");
                foreach (var innerException in ExpandExceptions(ex))
                    Console.WriteLine(innerException.Message);

                Environment.ExitCode = 1;
            }
        }

        private static IEnumerable<Exception> ExpandExceptions(Exception ex)
        {
                if (ex is AggregateException)
                foreach (var innerException in ((AggregateException)ex).InnerExceptions)
                    foreach (var exposedException in ExpandExceptions(innerException))
                        yield return exposedException;
            else
                yield return ex;
        }
    }
}
