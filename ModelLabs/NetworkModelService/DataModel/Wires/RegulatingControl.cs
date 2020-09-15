using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTN.Services.NetworkModelService.DataModel.Wires
{
    public class RegulatingControl : PowerSystemResource
    {
        private bool isDiscrete;
        private RegulatingControlModeKind mode;
        private PhaseCode monitoredPhase;
        private float targetRange;
        private float targetValue;
        private long terminal;
        private List<long> regulationSchedules = new List<long>();
        private List<long> regulatingCondEqs = new List<long>();

        public RegulatingControl(long globalId) : base(globalId)
        {
        }

        public bool IsDiscrete { get => isDiscrete; set => isDiscrete = value; }
        public RegulatingControlModeKind Mode { get => mode; set => mode = value; }
        public PhaseCode MonitoredPhase { get => monitoredPhase; set => monitoredPhase = value; }
        public float TargetRange { get => targetRange; set => targetRange = value; }
        public float TargetValue { get => targetValue; set => targetValue = value; }
        public long Terminal { get => terminal; set => terminal = value; }
        public List<long> RegulationSchedules { get => regulationSchedules; set => regulationSchedules = value; }
        public List<long> RegulatingCondEqs { get => regulatingCondEqs; set => regulatingCondEqs = value; }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                var x = (RegulatingControl)obj;
                return x.IsDiscrete == this.IsDiscrete && x.Mode == this.Mode &&
                       x.MonitoredPhase == this.MonitoredPhase && x.TargetRange == this.TargetRange &&
                       x.TargetValue == this.TargetValue && x.Terminal == this.Terminal &&
                       CompareHelper.CompareLists(x.RegulationSchedules, this.RegulationSchedules) &&
                       CompareHelper.CompareLists(x.RegulatingCondEqs, this.RegulatingCondEqs);
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
                case ModelCode.REGULATING_CONTROL_MONITORED_PHASE:
                case ModelCode.REGULATING_CONTROL_MODE:
                case ModelCode.REGULATING_CONTROL_DISCRETE:
                case ModelCode.REGULATING_CONTROL_TARGET_RANGE:
                case ModelCode.REGULATING_CONTROL_TARGET_VALUE:
                case ModelCode.REGULATING_CONTROL_TERMINAL:
                case ModelCode.REGULATING_CONTROL_REG_SCHEDS:
                case ModelCode.REGULATING_CONTROL_REG_CONDEQS:
                    return true;
                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.REGULATING_CONTROL_MODE:
                    property.SetValue((short)mode);
                    break;
                case ModelCode.REGULATING_CONTROL_MONITORED_PHASE:
                    property.SetValue((short)monitoredPhase);
                    break;
                case ModelCode.REGULATING_CONTROL_DISCRETE:
                    property.SetValue(isDiscrete);
                    break;
                case ModelCode.REGULATING_CONTROL_TARGET_VALUE:
                    property.SetValue(targetValue);
                    break;
                case ModelCode.REGULATING_CONTROL_TARGET_RANGE:
                    property.SetValue(targetRange);
                    break;
                case ModelCode.REGULATING_CONTROL_TERMINAL:
                    property.SetValue(terminal);
                    break;
                case ModelCode.REGULATING_CONTROL_REG_SCHEDS:
                    property.SetValue(regulationSchedules);
                    break;
                case ModelCode.REGULATING_CONTROL_REG_CONDEQS:
                    property.SetValue(regulatingCondEqs);
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
                case ModelCode.REGULATING_CONTROL_MODE:
                    mode = (RegulatingControlModeKind)property.AsEnum();
                    break;
                case ModelCode.REGULATING_CONTROL_MONITORED_PHASE:
                    monitoredPhase = (PhaseCode)property.AsEnum();
                    break;
                case ModelCode.REGULATING_CONTROL_DISCRETE:
                    isDiscrete = property.AsBool();
                    break;
                case ModelCode.REGULATING_CONTROL_TARGET_VALUE:
                    targetValue = property.AsFloat();
                    break;
                case ModelCode.REGULATING_CONTROL_TARGET_RANGE:
                    targetRange = property.AsFloat();
                    break;
                case ModelCode.REGULATING_CONTROL_TERMINAL:
                    terminal = property.AsLong();
                    break;
                default:
                    base.SetProperty(property);
                    break;
            }
        }

        #endregion IAccess implementation

        #region IReference implementation

        public override bool IsReferenced
        {
            get
            {
                return regulationSchedules.Count != 0 || regulatingCondEqs.Count != 0 || base.IsReferenced;
            }
        }
        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (terminal != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.REGULATING_CONTROL_TERMINAL] = new List<long>()
                {
                    terminal,
                };
            }

            if (regulationSchedules != null && regulationSchedules.Count != 0
                                            && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.REGULATING_CONTROL_REG_SCHEDS] = regulationSchedules.GetRange(0, regulationSchedules.Count);
            }

            if (regulatingCondEqs != null && regulatingCondEqs.Count != 0
                                            && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.REGULATING_CONTROL_REG_CONDEQS] = RegulatingCondEqs.GetRange(0, RegulatingCondEqs.Count);
            }

                base.GetReferences(references, refType);
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.REGULATION_SCHEDULE_REG_CONTROL:
                    regulationSchedules.Add(globalId);
                    break;
                case ModelCode.REGULATING_CONDEQ_REG_CONTROL:
                    RegulatingCondEqs.Add(globalId);
                    break;
                default:
                    base.AddReference(referenceId, globalId);
                    break;
            }
        }

        public override void RemoveReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.REGULATION_SCHEDULE_REG_CONTROL:
                    if (regulationSchedules.Contains(globalId))
                        regulationSchedules.Remove(globalId);
                    else
                        CommonTrace.WriteTrace(CommonTrace.TraceWarning, "Entity (GID = 0x{0:x16}) doesn't contain reference 0x{1:x16}.", this.GlobalId, globalId);
                    break;
                case ModelCode.REGULATING_CONDEQ_REG_CONTROL:
                    if (RegulatingCondEqs.Contains(globalId))
                        RegulatingCondEqs.Remove(globalId);
                    else
                        CommonTrace.WriteTrace(CommonTrace.TraceWarning, "Entity (GID = 0x{0:x16}) doesn't contain reference 0x{1:x16}.", this.GlobalId, globalId);
                    break;
                default:
                    base.RemoveReference(referenceId, globalId);
                    break;
            }
        }

        #endregion IReference implementation
    }
}
