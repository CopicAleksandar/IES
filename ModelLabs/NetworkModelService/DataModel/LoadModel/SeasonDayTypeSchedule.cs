using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    public class SeasonDayTypeSchedule : RegularIntervalSchedule
    {
        private long dayType = 0;

        public SeasonDayTypeSchedule(long globalId) : base(globalId)
        {
        }

        public long DayType { get => dayType; set => dayType = value; }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                SeasonDayTypeSchedule ss = (SeasonDayTypeSchedule)obj;

                return this.DayType == ss.DayType;
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
                case ModelCode.SEASON_DAY_TYPE_SCHEDULE_DAYTYPE:
                    return true;
                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property prop)
        {
            switch (prop.Id)
            {
                case ModelCode.SEASON_DAY_TYPE_SCHEDULE_DAYTYPE:
                    prop.SetValue(DayType);
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
                case ModelCode.SEASON_DAY_TYPE_SCHEDULE_DAYTYPE:
                    DayType = property.AsReference();
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
            if (DayType != 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.SEASON_DAY_TYPE_SCHEDULE_DAYTYPE] = new List<long>()
                {
                    DayType
                };
            }

            base.GetReferences(references, refType);
        }
        
        #endregion IReference implementation
    }
}
