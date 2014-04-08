using System;
using System.Globalization;
using System.IO.Abstractions;
using HansKindberg.Build.XmlTransformation.Tasks.Validation;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public class TransformableXmlFile : ITransformableXmlFile
	{
		#region Fields

		private readonly IPotentialFile _identity;
		private readonly IPotentialFile _preTransform;
		private readonly IPotentialFile _source;
		private readonly IPotentialFile _transform;

		#endregion

		#region Constructors

		public TransformableXmlFile(ITaskItem xmlFile, string transformName, XmlTransformMode transformMode, IFileSystem fileSystem, IPotentialFileFactory potentialFileFactory, IXmlTransformationMapRepository xmlTransformationMapRepository, IValidationLog validationLog)
		{
			if(xmlFile == null)
				throw new ArgumentNullException("xmlFile");

			if(fileSystem == null)
				throw new ArgumentNullException("fileSystem");

			if(potentialFileFactory == null)
				throw new ArgumentNullException("potentialFileFactory");

			if(xmlTransformationMapRepository == null)
				throw new ArgumentNullException("xmlTransformationMapRepository");

			if(validationLog == null)
				throw new ArgumentNullException("validationLog");

			this._identity = potentialFileFactory.CreatePotentialFile(xmlFile);

			if(!this._identity.Exists)
				validationLog.LogWarning(string.Format(CultureInfo.InvariantCulture, "The xml-file \"{0}\" does not exist.", this._identity));

			var xmlTransformationMap = xmlTransformationMapRepository.GetXmlTransformationMap(xmlFile, validationLog);

			if(xmlTransformationMap != null)
			{
				this._preTransform = transformMode == XmlTransformMode.Build ? xmlTransformationMap.CommonBuildTransform : xmlTransformationMap.CommonPublishTransform;
				this._source = xmlTransformationMap.Source ?? this._identity;
			}

			this._transform = potentialFileFactory.CreatePotentialFile(new TaskItem(fileSystem.Path.GetFileNameWithoutExtension(this._identity.ToString()) + "." + (transformName ?? string.Empty) + this._identity.Extension));
		}

		#endregion

		#region Properties

		public virtual IPotentialFile Identity
		{
			get { return this._identity; }
		}

		public virtual bool IsAppConfig
		{
			get { return this.Identity.ToString().Equals("App.config", StringComparison.OrdinalIgnoreCase); }
		}

		public virtual IPotentialFile PreTransform
		{
			get { return this._preTransform; }
		}

		public virtual bool PreTransformIsValid
		{
			get
			{
				if(!this.PreTransform.Exists)
					return false;

				if(!this.Source.Exists)
					return false;

				return true;
			}
		}

		public virtual IPotentialFile Source
		{
			get { return this._source; }
		}

		public virtual IPotentialFile Transform
		{
			get { return this._transform; }
		}

		public virtual bool TransformIsValid
		{
			get
			{
				if(!this.Transform.Exists)
					return false;

				if(!this.Source.Exists)
					return false;

				return true;
			}
		}

		#endregion

		#region Methods

		public override string ToString()
		{
			return this.Identity.ToString();
		}

		#endregion
	}
}