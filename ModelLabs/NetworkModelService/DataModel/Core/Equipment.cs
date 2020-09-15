using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using FTN.Common;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
	public class Equipment : PowerSystemResource
	{		
		private bool isAggregate;
		private bool isNormallyInService;
						
		public Equipment(long globalId) : base(globalId) 
		{
		}
			
        public bool IsAggregate { get => isAggregate; set => isAggregate = value; }
        public bool IsNormallyInService { get => isNormallyInService; set => isNormallyInService = value; }

        public override bool Equals(object obj)
		{
			if (base.Equals(obj))
			{
				Equipment x = (Equipment)obj;

				return ((x.IsAggregate == this.IsAggregate) &&
						(x.IsNormallyInService == this.IsNormallyInService));
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
				case ModelCode.EQUIPMENT_AGGREGATE:
				case ModelCode.EQUIPMENT_NORMALLYINSERVICE:
					return true;
				default:
					return base.HasProperty(property);
			}
		}

		public override void GetProperty(Property property)
		{
			switch (property.Id)
			{
				case ModelCode.EQUIPMENT_AGGREGATE:
					property.SetValue(IsAggregate);
					break;

				case ModelCode.EQUIPMENT_NORMALLYINSERVICE:
					property.SetValue(IsNormallyInService);
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
				case ModelCode.EQUIPMENT_AGGREGATE:					
					IsAggregate = property.AsBool();
					break;

				case ModelCode.EQUIPMENT_NORMALLYINSERVICE:
					IsNormallyInService = property.AsBool();
					break;
			
				default:
					base.SetProperty(property);
					break;
			}
		}		

		#endregion IAccess implementation
	}
}
