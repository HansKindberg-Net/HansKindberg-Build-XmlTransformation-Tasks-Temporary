using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using HansKindberg.Build.XmlTransformation.Tasks.Framework;
using HansKindberg.Build.XmlTransformation.Tasks.Xdt;
using HansKindberg.Build.XmlTransformation.Tasks.Xdt.Extensions;
using Microsoft.Build.Framework;

namespace HansKindberg.Build.XmlTransformation.Tasks.IoC
{
	public class DefaultServiceLocator : IServiceLocator
	{
		#region Fields

		private readonly IFileSystem _fileSystem = new FileSystem();
		private readonly IEqualityComparer<ITaskItem> _taskItemComparer = new TaskItemComparer();
		private readonly IXmlTransformableDocumentExtension _xmlTransformableDocumentExtension = new DefaultIXmlTransformableDocumentExtension();
		private IXmlTransformationDecoratorFactory _xmlTransformationDecoratorFactory;
		private readonly IXmlTransformationFactory _xmlTransformationFactory = new XmlTransformationFactory();

		#endregion

		#region Properties

		protected internal virtual IFileSystem FileSystem
		{
			get { return this._fileSystem; }
		}

		protected internal virtual IEqualityComparer<ITaskItem> TaskItemComparer
		{
			get { return this._taskItemComparer; }
		}

		[CLSCompliant(false)]
		protected internal virtual IXmlTransformableDocumentExtension XmlTransformableDocumentExtension
		{
			get { return this._xmlTransformableDocumentExtension; }
		}

		protected internal virtual IXmlTransformationDecoratorFactory XmlTransformationDecoratorFactory
		{
			get { return this._xmlTransformationDecoratorFactory ?? (this._xmlTransformationDecoratorFactory = new XmlTransformationDecoratorFactory(this.FileSystem, this.TaskItemComparer)); }
		}

		[CLSCompliant(false)]
		protected internal virtual IXmlTransformationFactory XmlTransformationFactory
		{
			get { return this._xmlTransformationFactory; }
		}

		#endregion

		#region Methods

		public virtual object GetService(Type serviceType)
		{
			if(serviceType == null)
				throw new ArgumentNullException("serviceType");

			if(serviceType == typeof(IEqualityComparer<ITaskItem>))
				return this.TaskItemComparer;

			if(serviceType == typeof(IFileSystem))
				return this.FileSystem;

			if(serviceType == typeof(IXmlTransformableDocumentExtension))
				return this.XmlTransformableDocumentExtension;

			if(serviceType == typeof(IXmlTransformationDecoratorFactory))
				return this.XmlTransformationDecoratorFactory;

			if(serviceType == typeof(IXmlTransformationFactory))
				return this.XmlTransformationFactory;

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