using System;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public class XmlFileToTransform : IXmlFileToTransform
	{
		#region Fields

		private IPotentialFile _source;

		#endregion

		#region Properties

		public virtual bool IsAppConfig
		{
			get { return this.XmlFile != null && this.XmlFile.Extension.Equals("App.config", StringComparison.OrdinalIgnoreCase); }
		}

		public virtual IPotentialFile PreTransform { get; set; }

		public virtual bool PreTransformIsValid
		{
			get
			{
				if(!this.Source.Exists)
					return false;

				if(!this.PreTransform.Exists)
					return false;

				return true;
			}
		}

		public virtual IPotentialFile Source
		{
			get { return this._source ?? this.XmlFile; }
			set { this._source = value; }
		}

		public virtual IPotentialFile Transform { get; set; }

		public virtual bool TransformIsValid
		{
			get
			{
				if(!this.Source.Exists)
					return false;

				if(!this.Transform.Exists)
					return false;

				return true;
			}
		}

		public virtual IPotentialFile XmlFile { get; set; }

		#endregion
	}
}