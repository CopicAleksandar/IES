using FTN.Common;
using FTN.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI
{
	public class GDA : IDisposable
	{
		private ModelResourcesDesc modelResourcesDesc = new ModelResourcesDesc();
		private NetworkModelGDAProxy _gdaQueryProxy = null;		

		public GDA()
		{
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}

		private NetworkModelGDAProxy GdaQueryProxy
		{
			get
			{
				if (_gdaQueryProxy != null)
				{
					_gdaQueryProxy.Abort();
					_gdaQueryProxy = null;
				}

				_gdaQueryProxy = new NetworkModelGDAProxy("NetworkModelGDAEndpoint");
				_gdaQueryProxy.Open();

				return _gdaQueryProxy;
			}
		}

		#region GDA Queries
		
		public ResourceDescription GetValues(long globalId, List<ModelCode> properties)
		{
			var message = "Getting values method started.";
			CommonTrace.WriteTrace(CommonTrace.TraceError, message);

			ResourceDescription rd = null;

			try
			{
				rd = GdaQueryProxy.GetValues(globalId, properties);

				message = "Getting values method successfully finished.";
				CommonTrace.WriteTrace(CommonTrace.TraceError, message);
			}
			catch (Exception e)
			{
				message = string.Format("Getting values method for entered id = {0} failed.\n\t{1}", globalId, e.Message);
				CommonTrace.WriteTrace(CommonTrace.TraceError, message);
			}

			return rd;
		}

		public List<long> GetExtentValues(ModelCode modelCodeType, List<ModelCode> properties, StringBuilder sb)
		{
			var message = "Getting extent values method started.";
			CommonTrace.WriteTrace(CommonTrace.TraceError, message);

			int iteratorId = 0;
			int resourcesLeft = 0;
			int numberOfResources = 300;
			var ids = new List<long>();
			var tempSb = new StringBuilder();
			
			try
			{
				iteratorId = GdaQueryProxy.GetExtentValues(modelCodeType, properties);
				resourcesLeft = GdaQueryProxy.IteratorResourcesLeft(iteratorId);

				while (resourcesLeft > 0)
				{
					List<ResourceDescription> rds = GdaQueryProxy.IteratorNext(numberOfResources, iteratorId);

					foreach (var rd in rds)
					{
						if (rd == null)
							continue;
						tempSb.Append($"\nEntity with gid: 0x{rd.Id:X16}" + Environment.NewLine);

						foreach (var property in rd.Properties)
						{
							switch (property.Type)
							{
								case PropertyType.Int64:
									StringAppender.AppendLong(tempSb, property);
									break;
								case PropertyType.Float:
									StringAppender.AppendFloat(tempSb, property);
									break;
								case PropertyType.String:
									StringAppender.AppendString(tempSb, property);
									break;
								case PropertyType.Reference:
									StringAppender.AppendReference(tempSb, property);
									break;
								case PropertyType.ReferenceVector:
									StringAppender.AppendReferenceVector(tempSb, property);
									break;

								default:
									tempSb.Append(
										$"\t{property.Id}: {property.PropertyValue.LongValue}{Environment.NewLine}");
									break;
							}
						}

						ids.Add(rd.Id);
					}

					resourcesLeft = GdaQueryProxy.IteratorResourcesLeft(iteratorId);
				}

				GdaQueryProxy.IteratorClose(iteratorId);

				message = "Getting extent values method successfully finished.";
				CommonTrace.WriteTrace(CommonTrace.TraceError, message);
			}
			catch (Exception e)
			{
				message = string.Format("Getting extent values method failed for {0}.\n\t{1}", modelCodeType, e.Message);
				CommonTrace.WriteTrace(CommonTrace.TraceError, message);
			}

			if (sb != null)
			{
				sb.Append(tempSb.ToString());
			}

			return ids;
		}

		public List<long> GetRelatedValues(long sourceGlobalId, List<ModelCode> properties, Association association, StringBuilder sb)
		{
			var message = "Getting related values method started.";
			CommonTrace.WriteTrace(CommonTrace.TraceError, message);

			int iteratorId = 0;
			int resourcesLeft = 0;
			int numberOfResources = 500;
			var resultIds = new List<long>();
			var tempSb = new StringBuilder();

			try
			{
				iteratorId = GdaQueryProxy.GetRelatedValues(sourceGlobalId, properties, association);
				resourcesLeft = GdaQueryProxy.IteratorResourcesLeft(iteratorId);

				while (resourcesLeft > 0)
				{
					List<ResourceDescription> rds = GdaQueryProxy.IteratorNext(numberOfResources, iteratorId);

					foreach (var rd in rds)
					{
						if (rd == null)
							continue;
						tempSb.Append($"Entity with gid: 0x{rd.Id:X16}" + Environment.NewLine);

						foreach (Property property in rd.Properties)
						{
							switch (property.Type)
							{
								case PropertyType.Int64:
									StringAppender.AppendLong(tempSb, property);
									break;
								case PropertyType.Float:
									StringAppender.AppendFloat(tempSb, property);
									break;
								case PropertyType.String:
									StringAppender.AppendString(tempSb, property);
									break;
								case PropertyType.Reference:
									StringAppender.AppendReference(tempSb, property);
									break;
								case PropertyType.ReferenceVector:
									StringAppender.AppendReferenceVector(tempSb, property);
									break;

								default:
									tempSb.Append(
										$"\t{property.Id}: {property.PropertyValue.LongValue}{Environment.NewLine}");
									break;
							}
						}

						resultIds.Add(rd.Id);
					}

					resourcesLeft = GdaQueryProxy.IteratorResourcesLeft(iteratorId);
				}

				GdaQueryProxy.IteratorClose(iteratorId);

				message = "Getting related values method successfully finished.";
				CommonTrace.WriteTrace(CommonTrace.TraceError, message);
			}
			catch (Exception e)
			{
				message =
					$"Getting related values method  failed for sourceGlobalId = {sourceGlobalId} and association (propertyId = {association.PropertyId}, type = {association.Type}). Reason: {e.Message}";
				CommonTrace.WriteTrace(CommonTrace.TraceError, message);
			}

			if (sb != null)
			{
				sb.Append(tempSb.ToString());
			}

			return resultIds;
		}

		#endregion

	}
}
