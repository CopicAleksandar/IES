using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI
{
    public static class StringAppender
    {
        public static void AppendReferenceVector(StringBuilder sb, Property property)
        {
            sb.Append($"\t{property.Id}: {Environment.NewLine}");
            foreach (long gid in property.AsReferences())
            {
                sb.Append($"\t\tGid: 0x{gid:X16}{ Environment.NewLine}");
            }
        }

        public static void AppendReference(StringBuilder sb, Property property)
        {
            sb.Append($"\t{property.Id}: 0x{property.AsReference():X16}{Environment.NewLine}");
        }

        public static void AppendString(StringBuilder sb, Property property)
        {
            sb.Append($"\t{property.Id}: {property.AsString()}{Environment.NewLine}");
        }

        public static void AppendFloat(StringBuilder sb, Property property)
        {
            sb.Append($"\t{property.Id}: {property.AsFloat()}{Environment.NewLine}");
        }

        public static void AppendLong(StringBuilder sb, Property property)
        {
            sb.Append($"\t{property.Id}: 0x{property.AsLong():X16}{Environment.NewLine}");
        }
    }
}
