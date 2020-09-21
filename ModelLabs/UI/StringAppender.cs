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

        public static void AppendEnum(StringBuilder sb, Property property)
        {            
            if (property.Id.ToString() == "BASIC_INT_SCHED_VAL1MUL" || property.Id.ToString() == "BASIC_INT_SCHED_VAL2MUL")
            {
                sb.Append($"\t{property.Id}: {(UnitMultiplier)property.AsEnum()}{Environment.NewLine}");
            }
            else if (property.Id.ToString() == "BASIC_INT_SCHED_VAL1UNIT" || property.Id.ToString() == "BASIC_INT_SCHED_VAL2UNIT")
            {
                sb.Append($"\t{property.Id}: {(UnitSymbol)property.AsEnum()}{Environment.NewLine}");
            }
            else if (property.Id.ToString() == "REGULATING_CONTROL_MONITORED_PHASE")
            {
                sb.Append($"\t{property.Id}: {(PhaseCode)property.AsEnum()}{Environment.NewLine}");
            }
            else if (property.Id.ToString() == "REGULATING_CONTROL_MODE")
            {
                sb.Append($"\t{property.Id}: {(RegulatingControlModeKind)property.AsEnum()}{Environment.NewLine}");
            }
            else
                sb.Append($"\t{property.Id}: {property.AsEnum()}{Environment.NewLine}");
        }

        public static void AppendDateTime(StringBuilder sb, Property property)
        {
            sb.Append($"\t{property.Id}: {property.AsDateTime()}{Environment.NewLine}");
        }
    }
}
