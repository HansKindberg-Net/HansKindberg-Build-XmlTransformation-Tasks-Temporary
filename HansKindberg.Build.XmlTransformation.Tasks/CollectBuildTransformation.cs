using System.IO.Abstractions;
using HansKindberg.Build.XmlTransformation.Tasks.IoC;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public class CollectBuildTransformation : CollectXmlTransformation
	{
		#region Constructors

		public CollectBuildTransformation() : this(ServiceLocator.Instance.GetService<IFileSystem>(), ServiceLocator.Instance.GetService<IXmlTransformationDecoratorFactory>()) {}
		public CollectBuildTransformation(IFileSystem fileSystem, IXmlTransformationDecoratorFactory xmlTransformationDecoratorFactory) : base(fileSystem, xmlTransformationDecoratorFactory) {}

		#endregion

		#region Properties

		protected internal override TransformMode TransformMode
		{
			get { return Tasks.TransformMode.Build; }
		}

		#endregion
	}
}