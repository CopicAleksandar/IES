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
	public class ConductingEquipment : Equipment
	{
		private List<long> _terminals = new List<long>();
		
		public ConductingEquipment(long globalId) : base(globalId)
		{
		}

		public List<long> Terminals { get => _terminals; set => _terminals = value; }

		public override bool Equals(object obj)
		{
			if (base.Equals(obj))
			{
				ConductingEquipment x = (ConductingEquipment)obj;

				return CompareHelper.CompareLists(this.Terminals, x.Terminals);
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
				case ModelCode.CONDEQ_TERMINALS:
					return true;
				default:
					return base.HasProperty(property);
			}
		}

		public override void GetProperty(Property prop)
		{
			switch (prop.Id)
			{
				case ModelCode.CONDEQ_TERMINALS:
					prop.SetValue(Terminals);
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
				return Terminals.Count != 0 || base.IsReferenced;
			}
		}

		public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
		{
			if(Terminals != null && Terminals.Count != 0
										   && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
			{
				references[ModelCode.CONDEQ_TERMINALS] = Terminals.GetRange(0, Terminals.Count);
			}

			base.GetReferences(references, refType);
		}

		public override void AddReference(ModelCode referenceId, long globalId)
		{
			switch (referenceId)
			{
				case ModelCode.TERMINAL_CONDEQ:
					Terminals.Add(globalId);
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
				case ModelCode.TERMINAL_CONDEQ:
					if (Terminals.Contains(globalId))
						Terminals.Remove(globalId);
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
