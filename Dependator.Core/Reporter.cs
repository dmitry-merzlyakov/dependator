using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dependator.Core
{
    public class Reporter
    {
        public string PrintFromToEndpoints(ReferenceModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Separator);
            sb.AppendLine("== Classes that are referenced in indirect way");
            sb.AppendLine(Separator);
            sb.AppendLine(PrintReferenceEndpoints(model.ReferenceTo.OrderByDescending(p => p.References.Count())));
            sb.AppendLine();
            sb.AppendLine(Separator);
            sb.AppendLine("== Classes that reference in indirect way");
            sb.AppendLine(Separator);
            sb.AppendLine(PrintReferenceEndpoints(model.ReferenceFrom.OrderByDescending(p => p.References.Count())));
            sb.AppendLine(Separator);

            return sb.ToString();
        }

        public string PrintReferenceEndpoints(IEnumerable<ReferenceEndpoint> endPoints)
        {
            if (endPoints == null)
                throw new ArgumentNullException("endPoints");

            StringBuilder sb = new StringBuilder();

            foreach(var endPoint in endPoints)
            {
                sb.AppendFormat("[{0} {1}] {2} reference(s)", endPoint.Symbol.TypeKind, endPoint.Symbol, endPoint.References.Count());
                sb.AppendLine();

                foreach(var grp in endPoint.References.GroupBy(r => new Tuple<string,string>(r.Inverse(endPoint).Symbol.ToString(), r.ReferenceType.ToString())).OrderByDescending(g => g.Count()))
                {
                    sb.AppendFormat("   ({0}) {1} - {2} reference(s)", grp.Key.Item1, grp.Key.Item2, grp.Count());
                    sb.AppendLine();
                }
            }
            sb.AppendLine();

            return sb.ToString();
        }

        private readonly string Separator = new string('=', 50);
    }
}
