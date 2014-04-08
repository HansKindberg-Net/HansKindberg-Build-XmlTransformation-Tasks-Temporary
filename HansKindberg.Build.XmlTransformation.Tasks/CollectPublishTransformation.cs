using System.IO.Abstractions;
using HansKindberg.Build.XmlTransformation.Tasks.IoC;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public class CollectPublishTransformation : CollectXmlTransformation
	{
		#region Constructors

		public CollectPublishTransformation() : this(ServiceLocator.Instance.GetService<IFileSystem>(), ServiceLocator.Instance.GetService<IXmlTransformationDecoratorFactory>()) {}
		public CollectPublishTransformation(IFileSystem fileSystem, IXmlTransformationDecoratorFactory xmlTransformationDecoratorFactory) : base(fileSystem, xmlTransformationDecoratorFactory) {}

		#endregion

		#region Properties

		protected internal override TransformMode TransformMode
		{
			get { return Tasks.TransformMode.Publish; }
		}

		#endregion
	}
}