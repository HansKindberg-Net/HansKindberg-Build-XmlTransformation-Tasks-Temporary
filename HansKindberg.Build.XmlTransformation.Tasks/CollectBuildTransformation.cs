using System.Collections.Generic;
using System.IO.Abstractions;
using HansKindberg.Build.XmlTransformation.Tasks.IoC;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public class CollectBuildTransformation : CollectXmlTransformation
	{
		#region Constructors

		public CollectBuildTransformation() : base(ServiceLocator.Instance.GetService<IFileSystem>(), ServiceLocator.Instance.GetService<IXmlTransformationMapFactory>()) {}

		#endregion

		#region Methods

		public override bool Execute()
		{
			var filesToTransform = new List<ITaskItem>();
			filesToTransform.Add(new TaskItem("First"));
			filesToTransform.Add(new TaskItem("Second"));
			filesToTransform.Add(new TaskItem("Third"));
			this.FilesToTransform = filesToTransform.ToArray();

			this.Log.LogMessageFromText("----------- CollectBuildTransformation", MessageImportance.High);
			return true;
		}

		#endregion
	}
}