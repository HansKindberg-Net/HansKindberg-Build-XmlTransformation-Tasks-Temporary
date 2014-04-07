using System;
using System.Collections.Generic;
using Microsoft.Build.Framework;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public class XmlTransformFactory
	{
		#region Fields

		private readonly IPotentialFileFactory _potentialFileFactory;

		#endregion

		#region Constructors

		public XmlTransformFactory(IPotentialFileFactory potentialFileFactory)
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

		public virtual IXmlFileToTransform Create(ITaskItem file, string transformName, XmlTransformMode xmlTransformMode, IEnumerable<IXmlTransformationMap> xmlTransformationMaps)
		{
			if(file == null)
				throw new ArgumentNullException("file");

			if(xmlTransformationMaps == null)
				throw new ArgumentNullException("xmlTransformationMaps");

			var xmlFileTransform = new XmlFileToTransform
			{
				XmlFile = this.PotentialFileFactory.Create(file),
			};

			xmlFileTransform.Transform = this.PotentialFileFactory.Create(xmlFileTransform.XmlFile.OriginalPathWithoutExtension + "." + (transformName ?? string.Empty) + xmlFileTransform.XmlFile.Extension);

			return xmlFileTransform;

			//var xmlFileToTransform = this.PotentialFileFactory.Create(file);

			//var xmlTransformationMap = new XmlTransformationMap();

			//var commonBuildTransform = xmlTransformationMapTaskItem.GetMetadata("CommonBuildTransform");

			//if(commonBuildTransform != null)
			//	xmlTransformationMap.CommonBuildTransform = this.FileSystem.FileInfo.FromFileName(commonBuildTransform);

			//var commonPublishTransform = xmlTransformationMapTaskItem.GetMetadata("CommonPublishTransform");

			//if(commonPublishTransform != null)
			//	xmlTransformationMap.CommonPublishTransform = this.FileSystem.FileInfo.FromFileName(commonPublishTransform);

			//var source = xmlTransformationMapTaskItem.GetMetadata("Source");

			//if(source != null)
			//	xmlTransformationMap.Source = this.FileSystem.FileInfo.FromFileName(source);

			//return xmlTransformationMap;
		}

		#endregion
	}
}