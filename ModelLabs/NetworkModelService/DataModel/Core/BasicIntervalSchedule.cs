using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    public class BasicIntervalSchedule : IdentifiedObject
    {
        private DateTime startTime;
        private UnitMultiplier value1Multiplier;
        private UnitSymbol value1Unit;
        private UnitMultiplier value2Multiplier;
        private UnitSymbol value2Unit;

        public BasicIntervalSchedule(long globalId) : base(globalId)
        {
        }

        public DateTime StartTime { get => startTime; set => startTime = value; }
        public UnitMultiplier Value1Multiplier { get => value1Multiplier; set => value1Multiplier = value; }
        public UnitSymbol Value1Unit { get => value1Unit; set => value1Unit = value; }
        public UnitMultiplier Value2Multiplier { get => value2Multiplier; set => value2Multiplier = value; }
        public UnitSymbol Value2Unit { get => value2Unit; set => value2Unit = value; }

        public override bool Equals(object x)
        {
            if (base.Equals(x))
            {
                BasicIntervalSchedule bis = (BasicIntervalSchedule)x;

                return bis.startTime  == this.startTime  && bis.value1Multiplier == this.value1Multiplier &&
                       bis.Value1Unit == this.Value1Unit && bis.Value2Multiplier == this.Value2Multiplier &&
                       bis.Value2Unit == this.Value2Unit;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region IAccess implementation

        public override bool HasProperty(ModelCode property)
        {
            switch (property)
            {
                case ModelCode.BASIC_INT_SCHED_STARTTIME:
                case ModelCode.BASIC_INT_SCHED_VAL1MUL:
                case ModelCode.BASIC_INT_SCHED_VAL1UNIT:
                case ModelCode.BASIC_INT_SCHED_VAL2MUL:
                case ModelCode.BASIC_INT_SCHED_VAL2UNIT:
                    return true;
                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.BASIC_INT_SCHED_STARTTIME:
                    property.SetValue(StartTime);
                    break;

                case ModelCode.BASIC_INT_SCHED_VAL1MUL:
                    property.SetValue((short)Value1Multiplier);
                    break;
                case ModelCode.BASIC_INT_SCHED_VAL1UNIT:
                    property.SetValue((short)Value1Unit);
                    break;
                case ModelCode.BASIC_INT_SCHED_VAL2MUL:
                    property.SetValue((short)Value2Multiplier);
                    break;
                case ModelCode.BASIC_INT_SCHED_VAL2UNIT:
                    property.SetValue((short)Value2Unit);
                    break;
                default:
                    base.GetProperty(property);
                    break;
            }
        }

        public override void SetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.BASIC_INT_SCHED_STARTTIME:
                    StartTime = property.AsDateTime();
                    break;
                case ModelCode.BASIC_INT_SCHED_VAL1MUL:
                    Value1Multiplier = (UnitMultiplier)property.AsEnum();
                    break;
                case ModelCode.BASIC_INT_SCHED_VAL1UNIT:
                    Value1Unit = (UnitSymbol)property.AsEnum();
                    break;
                case ModelCode.BASIC_INT_SCHED_VAL2MUL:
                    Value2Multiplier = (UnitMultiplier)property.AsEnum();
                    break;
                case ModelCode.BASIC_INT_SCHED_VAL2UNIT:
                    Value2Unit = (UnitSymbol)property.AsEnum();
                    break;
                default:
                    base.SetProperty(property);
                    break;
            }
        }

        #endregion
    }
}
