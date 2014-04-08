using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;

namespace HansKindberg.Build.XmlTransformation.Tasks.IoC
{
	public class DefaultServiceLocator : IServiceLocator
	{
		#region Fields

		private readonly IFileSystem _fileSystem;
		private IXmlTransformationDecoratorFactory _xmlTransformationDecoratorFactory;

		#endregion

		#region Constructors

		public DefaultServiceLocator()
		{
			this._fileSystem = new FileSystem();
		}

		#endregion

		#region Properties

		protected internal virtual IFileSystem FileSystem
		{
			get { return this._fileSystem; }
		}

		protected internal virtual IXmlTransformationDecoratorFactory XmlTransformationDecoratorFactory
		{
			get { return this._xmlTransformationDecoratorFactory ?? (this._xmlTransformationDecoratorFactory = new XmlTransformationDecoratorFactory(this.FileSystem)); }
		}

		#endregion

		#region Methods

		public virtual object GetService(Type serviceType)
		{
			if(serviceType == null)
				throw new ArgumentNullException("serviceType");

			if(serviceType == typeof(IFileSystem))
				return this.FileSystem;

			if(serviceType == typeof(IXmlTransformationDecoratorFactory))
				return this.XmlTransformationDecoratorFactory;

			return Activator.CreateInstance(serviceType);
		}

		public virtual T GetService<T>()
		{
			return (T) this.GetService(typeof(T));
		}

		public virtual T GetService<T>(string key)
		{
			return (T) this.GetService(typeof(T), key);
		}

		public virtual object GetService(Type serviceType, string key)
		{
			if(key == null)
				return this.GetService(serviceType);

			throw new NotImplementedException();
		}

		public virtual IEnumerable<T> GetServices<T>()
		{
			return this.GetServices(typeof(T)).OfType<T>();
		}

		public virtual IEnumerable<object> GetServices(Type serviceType)
		{
			return new[] {this.GetService(serviceType)};
		}

		#endregion
	}
}