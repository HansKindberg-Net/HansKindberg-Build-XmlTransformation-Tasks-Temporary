using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Abstractions;
using System.Linq;
using HansKindberg.Build.XmlTransformation.Tasks.Validation;
using Microsoft.Build.Framework;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public class XmlTransformFactory : IXmlTransformFactory
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

		public virtual IFileSystem FileSystem
		{
			get { return this.PotentialFileFactory.FileSystem; }
		}

		public virtual IPotentialFileFactory PotentialFileFactory
		{
			get { return this._potentialFileFactory; }
		}

		#endregion

		#region Methods

		public virtual IXmlFileToTransform Create(ITaskItem file, string transformName, XmlTransformMode xmlTransformMode, IEnumerable<IXmlTransformationMap> xmlTransformationMaps, IValidationLog validationLog)
		{
			if(file == null)
				throw new ArgumentNullException("file");

			if(xmlTransformationMaps == null)
				throw new ArgumentNullException("xmlTransformationMaps");

			if(validationLog == null)
				throw new ArgumentNullException("validationLog");

			var xmlFileToTransform = this.CreateXmlFileToTransform(file, validationLog);

			var xmlTransformationMap = this.GetXmlTransformationMap(xmlFileToTransform, xmlTransformationMaps, validationLog);

			if(xmlTransformationMap != null)
			{
				xmlFileToTransform.PreTransform = xmlTransformMode == XmlTransformMode.Build ? xmlTransformationMap.CommonBuildTransform : xmlTransformationMap.CommonPublishTransform;
				xmlFileToTransform.Source = xmlTransformationMap.Source;
			}

			xmlFileToTransform.Transform = this.PotentialFileFactory.Create(this.FileSystem.Path.GetFileNameWithoutExtension(xmlFileToTransform.XmlFile.OriginalPath) + "." + (transformName ?? string.Empty) + xmlFileToTransform.XmlFile.Extension);

			return xmlFileToTransform;
		}

		protected internal virtual XmlFileToTransform CreateXmlFileToTransform(ITaskItem file, IValidationLog validationLog)
		{
			if(validationLog == null)
				throw new ArgumentNullException("validationLog");

			var xmlFileTransform = new XmlFileToTransform
			{
				XmlFile = this.PotentialFileFactory.Create(file),
			};

			if(!xmlFileTransform.XmlFile.Exists)
				validationLog.LogError(string.Format(CultureInfo.InvariantCulture, "The file \"{0}\" does not exist.", xmlFileTransform.XmlFile.OriginalPath));

			return xmlFileTransform;
		}

		protected internal virtual IXmlTransformationMap GetXmlTransformationMap(IXmlFileToTransform xmlFileToTransform, IEnumerable<IXmlTransformationMap> xmlTransformationMaps, IValidationLog validationLog)
		{
			if(validationLog == null)
				throw new ArgumentNullException("validationLog");

			List<IXmlTransformationMap> xmlTransformationMapList = new List<IXmlTransformationMap>();

			IEnumerable<IXmlTransformationMap> xmlTransformationMapsCopy = xmlTransformationMaps.ToArray();

			xmlTransformationMapList.AddRange(xmlTransformationMapsCopy.Where(xmlTransformationMap => xmlFileToTransform.XmlFile.OriginalPath.Equals(xmlTransformationMap.Identity.OriginalPath)));
			xmlTransformationMapList.AddRange(xmlTransformationMapsCopy.Where(xmlTransformationMap => xmlFileToTransform.XmlFile.FullName.Equals(xmlTransformationMap.Identity.FullName)));

			if(xmlTransformationMapList.Count > 1)
				validationLog.LogWarning(string.Format(CultureInfo.InvariantCulture, "The file \"{0}\" has multiple XmlTransformationMaps. The first will be used. See the \"XmlTransformationMap\"-itemgroup with identity \"{0}\".", xmlFileToTransform.XmlFile.OriginalPath));

			return xmlTransformationMapList.FirstOrDefault();
		}

		#endregion
	}
}