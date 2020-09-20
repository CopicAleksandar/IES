using CIM.Model;
using FTN.Common;
using FTN.ESI.SIMES.CIM.CIMAdapter.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTN.ESI.SIMES.CIM.CIMAdapter.Importer
{
	public class IES1Importer
	{
		private static IES1Importer ptImporter = null;
		private static object singletonLock = new object();

		private ConcreteModel concreteModel;
		private Delta delta;
		private ImportHelper importHelper;
		private TransformAndLoadReport report;


		#region Properties

		public static IES1Importer Instance
		{
			get
			{
				if (ptImporter == null)
				{
					lock (singletonLock)
					{
						if (ptImporter == null)
						{
							ptImporter = new IES1Importer();
							ptImporter.Reset();
						}
					}
				}
				return ptImporter;
			}
		}

		public Delta NMSDelta
		{
			get { return delta; }
		}

		#endregion Properties


		public void Reset()
		{
			concreteModel = null;
			delta = new Delta();
			importHelper = new ImportHelper();
			report = null;
		}

		public TransformAndLoadReport CreateNMSDelta(ConcreteModel cimConcreteModel)
		{
			LogManager.Log("Importing IES2 Elements...", LogLevel.Info);
			report = new TransformAndLoadReport();
			concreteModel = cimConcreteModel;
			delta.ClearDeltaOperations();

			if (concreteModel != null && concreteModel.ModelMap != null)
			{
				try
				{
					// convert into DMS elements
					ConvertModelAndPopulateDelta();
				}
				catch (Exception ex)
				{
					var message = $"{DateTime.Now} - ERROR in data import - {ex.Message}";
					LogManager.Log(message);
					report.Report.AppendLine(ex.Message);
					report.Success = false;
				}
			}

			LogManager.Log("Importing IES2 Elements - END.", LogLevel.Info);

			return report;
		}

		/// <summary>
		/// Method performs conversion of network elements from CIM based concrete model into DMS model.
		/// </summary>
		private void ConvertModelAndPopulateDelta()
		{
			LogManager.Log("Loading elements and creating delta...", LogLevel.Info);

			//// import all concrete model types (DMSType enum)
			
			Import<RegulatingControl>(DMSType.REGULATING_CONTROL, "FTN.RegulatingControl");
			Import<StaticVarCompensator>(DMSType.STATIC_VAR_COMPENSATOR, "FTN.StaticVarCompensator");
			Import<ShuntCompensator>(DMSType.SHUNT_COMPENSATOR, "FTN.ShuntCompensator");
			Import<DayType>(DMSType.DAY_TYPE, "FTN.DayType");
			Import<RegulationSchedule>(DMSType.REGULATION_SCHEDULE, "FTN.RegulationSchedule");
			Import<Terminal>(DMSType.TERMINAL, "FTN.Terminal");

			LogManager.Log("Loading elements and creating delta completed.", LogLevel.Info);
		}

		#region Import

		/// <summary>
		/// Generic method to import cim objects based on DMSType
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dmsType"></param>
		/// <param name="typeName"></param>
		private void Import<T>(DMSType dmsType, string typeName) where T : IdentifiedObject
		{
			SortedDictionary<string, object> cimObjects = concreteModel.GetAllObjectsOfType(typeof(T).FullName);
			if (cimObjects == null)
				return;

			foreach (var kvp in cimObjects)
			{
				T cimObj = (T)kvp.Value;
				var rd = CreateResourceDescription(cimObj, dmsType);

				if (rd == null)
				{
					report.Report.Append($"{typeof(T).Name} ID = ").Append(cimObj.ID).AppendLine(" FAILED to be converted");
					continue;
				}
				else
				{
					delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
					report.Report.Append($"{typeof(T).Name} ID = ").Append(cimObj.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
				}

				report.Report.AppendLine();
			}
		}

		/// <summary>
		/// Generic method to create resource description based on DMSType
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cimObj"></param>
		/// <param name="dmsType"></param>
		/// <returns></returns>
		private ResourceDescription CreateResourceDescription<T>(T cimObj, DMSType dmsType) where T : IdentifiedObject
		{
			if (cimObj == null)
				return null;

			long gid = ModelCodeHelper.CreateGlobalId(0, (short)dmsType,
				importHelper.CheckOutIndexForDMSType(dmsType));

			var rd = new ResourceDescription(gid);

			importHelper.DefineIDMapping(cimObj.ID, gid);

			IES1Converter.PopulateProperties(cimObj, rd, importHelper, report);

			return rd;
		}

		#endregion
	}
}
