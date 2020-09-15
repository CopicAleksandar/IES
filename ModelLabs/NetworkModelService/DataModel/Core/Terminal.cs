using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    public class Terminal : IdentifiedObject
    {
        private long _conductingEquipment = 0;
        private List<long> _regulatingControls = new List<long>();

        public Terminal(long globalId) : base(globalId)
        {
        }

        public long ConductingEquipment { get => _conductingEquipment; set => _conductingEquipment = value; }

        public List<long> RegulatingControls { get => _regulatingControls; set => _regulatingControls = value; }

        public override bool Equals(object x)
        {
            if (base.Equals(x))
            {
                Terminal t = (Terminal)x;

                return CompareHelper.CompareLists(t.RegulatingControls, this.RegulatingControls) && 
                       t.ConductingEquipment == this.ConductingEquipment;
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
                case ModelCode.TERMINAL_CONDEQ:
                    return true;
                case ModelCode.TERMINAL_REGULATING_CONTROLS:
                    return true;
                default:
                    return base.HasProperty(property);                    
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.TERMINAL_CONDEQ:
                    property.SetValue(ConductingEquipment);
                    break;
                case ModelCode.TERMINAL_REGULATING_CONTROLS:
                    property.SetValue(RegulatingControls);
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
                case ModelCode.TERMINAL_CONDEQ:
                    ConductingEquipment = property.AsReference();
                    break;
                default:
                    base.SetProperty(property);
                    break;
            }
        }

        #endregion

        #region IReference implementation

        public override bool IsReferenced
        {
            get
            {
                return RegulatingControls.Count != 0 || base.IsReferenced;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (ConductingEquipment != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.TERMINAL_CONDEQ] = new List<long>()
                {
                    ConductingEquipment,
                };
            }

            if (RegulatingControls != null && RegulatingControls.Count != 0 &&
               (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.TERMINAL_REGULATING_CONTROLS] = RegulatingControls.GetRange(0, RegulatingControls.Count);
            }

            base.GetReferences(references, refType);
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.REGULATING_CONTROL_TERMINAL:
                    RegulatingControls.Add(globalId);
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
                case ModelCode.REGULATING_CONTROL_TERMINAL:
                    if (RegulatingControls.Contains(globalId))
                        RegulatingControls.Remove(globalId);
                    else
                        CommonTrace.WriteTrace(CommonTrace.TraceWarning, "Entity (GID = 0x{0:x16}) doesn't contain reference 0x{1:x16}.", this.GlobalId, globalId);
                    break;
                default:
                    base.RemoveReference(referenceId, globalId);
                    break;
            }
        }

        #endregion
    }
}
