using System;
using System.IO.Abstractions;
using HansKindberg.Build.XmlTransformation.Tasks.Validation;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public class XmlTransformationDecoratorFactory : IXmlTransformationDecoratorFactory
	{
		#region Fields

		private readonly IFileSystem _fileSystem;

		#endregion

		#region Constructors

		public XmlTransformationDecoratorFactory(IFileSystem fileSystem)
		{
			if(fileSystem == null)
				throw new ArgumentNullException("fileSystem");

			this._fileSystem = fileSystem;
		}

		#endregion

		#region Properties

		protected internal virtual IFileSystem FileSystem
		{
			get { return this._fileSystem; }
		}

		#endregion

		#region Methods

		public virtual IXmlTransformationDecorator Create(IXmlTransformationSettings xmlTransformationSettings, IValidationLog validationLog)
		{
			return new XmlTransformationDecorator(this.FileSystem, xmlTransformationSettings, validationLog);
		}

		#endregion
	}
}