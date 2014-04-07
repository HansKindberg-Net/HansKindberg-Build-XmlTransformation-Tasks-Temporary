using System;
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

		public virtual IXmlTransformationMap Create(ITaskItem xmlTransformationMapTaskItem)
		{
			if(xmlTransformationMapTaskItem == null)
				throw new ArgumentNullException("xmlTransformationMapTaskItem");

			var xmlTransformationMap = new XmlTransformationMap();

			var commonBuildTransform = xmlTransformationMapTaskItem.GetMetadata("CommonBuildTransform");

			if(commonBuildTransform != null)
				xmlTransformationMap.CommonBuildTransform = this.PotentialFileFactory.Create(commonBuildTransform);

			var commonPublishTransform = xmlTransformationMapTaskItem.GetMetadata("CommonPublishTransform");

			if(commonPublishTransform != null)
				xmlTransformationMap.CommonPublishTransform = this.PotentialFileFactory.Create(commonPublishTransform);

			var source = xmlTransformationMapTaskItem.GetMetadata("Source");

			if(source != null)
				xmlTransformationMap.Source = this.PotentialFileFactory.Create(source);

			return xmlTransformationMap;
		}

		#endregion
	}
}