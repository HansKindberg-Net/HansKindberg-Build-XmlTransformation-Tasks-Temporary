using System;
using System.Globalization;
using HansKindberg.Build.XmlTransformation.Tasks.Validation;
using Microsoft.Build.Framework;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public class XmlTransformationMapFactory : IXmlTransformationMapFactory
	{
		#region Fields

		private readonly IPotentialFileFactory _potentialFileFactory;

		#endregion

		#region Constructors

		public XmlTransformationMapFactory(IPotentialFileFactory potentialFileFactory)
		{
			if(potentialFileFactory == null)
				throw new ArgumentNullException("potentialFileFactory");

			this._potentialFileFactory = potentialFileFactory;
		}

		#endregion

		#region Properties

		protected internal virtual IPotentialFileFactory PotentialFileFactory
		{
			get { return this._potentialFileFactory; }
		}

		#endregion

		#region Methods

		public virtual IXmlTransformationMap Create(ITaskItem xmlTransformationMapTaskItem, IValidationLog validationLog)
		{
			if(xmlTransformationMapTaskItem == null)
				throw new ArgumentNullException("xmlTransformationMapTaskItem");

			if(validationLog == null)
				throw new ArgumentNullException("validationLog");

			var xmlTransformationMap = new XmlTransformationMap
			{
				Identity = this.PotentialFileFactory.Create(xmlTransformationMapTaskItem)
			};

			if(!xmlTransformationMap.Identity.Exists)
				this.LogWarning(string.Format(CultureInfo.InvariantCulture, "The file \"{0}\" does not exist.", xmlTransformationMap.Identity.OriginalPath), xmlTransformationMap, validationLog);

			var commonBuildTransform = xmlTransformationMapTaskItem.GetMetadata("CommonBuildTransform");

			if(commonBuildTransform != null)
			{
				xmlTransformationMap.CommonBuildTransform = this.PotentialFileFactory.Create(commonBuildTransform);

				if(!xmlTransformationMap.CommonBuildTransform.Exists)
					this.LogWarning(string.Format(CultureInfo.InvariantCulture, "The \"CommonBuildTransform\" \"{0}\" does not exist.", xmlTransformationMap.CommonBuildTransform.OriginalPath), xmlTransformationMap, validationLog);
			}

			var commonPublishTransform = xmlTransformationMapTaskItem.GetMetadata("CommonPublishTransform");

			if(commonPublishTransform != null)
			{
				xmlTransformationMap.CommonPublishTransform = this.PotentialFileFactory.Create(commonPublishTransform);

				if(!xmlTransformationMap.CommonPublishTransform.Exists)
					this.LogWarning(string.Format(CultureInfo.InvariantCulture, "The \"CommonPublishTransform\" \"{0}\" does not exist.", xmlTransformationMap.CommonPublishTransform.OriginalPath), xmlTransformationMap, validationLog);
			}

			var source = xmlTransformationMapTaskItem.GetMetadata("Source");

			if(source != null)
			{
				xmlTransformationMap.Source = this.PotentialFileFactory.Create(source);

				if(!xmlTransformationMap.Source.Exists)
					this.LogWarning(string.Format(CultureInfo.InvariantCulture, "The \"Source\" \"{0}\" does not exist.", xmlTransformationMap.Source.OriginalPath), xmlTransformationMap, validationLog);
			}

			return xmlTransformationMap;
		}

		protected internal virtual void LogWarning(string information, IXmlTransformationMap xmlTransformationMap, IValidationLog validationLog)
		{
			if(xmlTransformationMap == null)
				throw new ArgumentNullException("xmlTransformationMap");

			if(validationLog == null)
				throw new ArgumentNullException("validationLog");

			validationLog.LogWarning(information + string.Format(CultureInfo.InvariantCulture, " See the \"XmlTransformationMap\"-itemgroup with identity \"{0}\".", xmlTransformationMap.Identity.OriginalPath));
		}

		#endregion
	}
}