using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTN.Services.NetworkModelService.DataModel.Wires
{
    public class RegulatingCondEq : ConductingEquipment
    {
        private long regulatingControl = 0;

        public RegulatingCondEq(long globalId) : base(globalId)
        {
        }

        public long RegulatingControl { get => regulatingControl; set => regulatingControl = value; }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                RegulatingCondEq ss = (RegulatingCondEq)obj;

                return this.RegulatingControl == ss.RegulatingControl;
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
                case ModelCode.REGULATING_CONDEQ_REG_CONTROL:
                    return true;
                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property prop)
        {
            switch (prop.Id)
            {
                case ModelCode.REGULATING_CONDEQ_REG_CONTROL:
                    prop.SetValue(RegulatingControl);
                    break;
                default:
                    base.GetProperty(prop);
                    break;
            }
        }

        public override void SetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.REGULATING_CONDEQ_REG_CONTROL:
                    RegulatingControl = property.AsReference();
                    break;
                default:
                    base.SetProperty(property);
                    break;
            }
        }

        #endregion IAccess implementation

        #region IReference implementation

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (RegulatingControl != 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.SEASON_DAY_TYPE_SCHEDULE_DAYTYPE] = new List<long>()
                {
                    RegulatingControl
                };
            }

            base.GetReferences(references, refType);
        }

        #endregion IReference implementation
    }
}
