using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    public class DayType : IdentifiedObject
    {
        private List<long> seasonDayTypeSchedules = new List<long>();

        public DayType(long globalId) : base(globalId)
        {
        }

        public List<long> SeasonDayTypeSchedules { get => seasonDayTypeSchedules; set => seasonDayTypeSchedules = value; }

        public override bool Equals(object x)
        {
            if (base.Equals(x))
            {
                DayType dt = (DayType)x;

                return CompareHelper.CompareLists(this.SeasonDayTypeSchedules, dt.SeasonDayTypeSchedules);
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
                case ModelCode.DAY_TYPE_SEASON_DAYTYPE_SCHEDS:
                    return true;
                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property prop)
        {
            switch (prop.Id)
            {
                case ModelCode.DAY_TYPE_SEASON_DAYTYPE_SCHEDS:
                    prop.SetValue(SeasonDayTypeSchedules);
                    break;
                default:
                    base.GetProperty(prop);
                    break;
            }
        }

        public override void SetProperty(Property property)
        {
            base.SetProperty(property);
        }

        #endregion IAccess implementation

        #region IReference implementation

        public override bool IsReferenced
        {
            get
            {
                return SeasonDayTypeSchedules.Count != 0 || base.IsReferenced;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (SeasonDayTypeSchedules != null && SeasonDayTypeSchedules.Count != 0
                                           && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.DAY_TYPE_SEASON_DAYTYPE_SCHEDS] = SeasonDayTypeSchedules.GetRange(0, SeasonDayTypeSchedules.Count);
            }

            base.GetReferences(references, refType);
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.SEASON_DAY_TYPE_SCHEDULE_DAYTYPE:
                    SeasonDayTypeSchedules.Add(globalId);
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
                case ModelCode.SEASON_DAY_TYPE_SCHEDULE_DAYTYPE:
                    if (SeasonDayTypeSchedules.Contains(globalId))
                        SeasonDayTypeSchedules.Remove(globalId);
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
