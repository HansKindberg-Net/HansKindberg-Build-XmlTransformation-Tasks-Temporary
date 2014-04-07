using System.IO.Abstractions;
using HansKindberg.Build.XmlTransformation.Tasks.IoC;
using Microsoft.Build.Framework;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public class CollectPublishTransformation : CollectXmlTransformation
	{
		#region Constructors

		public CollectPublishTransformation() : base(ServiceLocator.Instance.GetService<IFileSystem>(), ServiceLocator.Instance.GetService<IXmlTransformationMapFactory>()) {}

		#endregion

		#region Methods

		public override bool Execute()
		{
			this.Log.LogMessageFromText("----------- CollectPublishTransformation", MessageImportance.High);
			return true;
		}

		#endregion
	}
}