using System;
using System.Collections.Generic;
using HansKindberg.Build.XmlTransformation.Tasks.Extensions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace HansKindberg.Build.XmlTransformation.Tasks.IntegrationTests.Helpers
{
	public static class FileHelper
	{
		#region Methods

		public static ITaskItem CreateFile(string identity)
		{
			if(identity == null)
				throw new ArgumentNullException("identity");

			return new TaskItem(identity);
		}

		public static ITaskItem CreateFile(string identity, string destination, bool? isAppConfig, string objective, string transform)
		{
			var file = CreateFile(identity);

			if(destination != null)
				file.SetMetadata(XmlTransformationDecoratedTaskItemExtension.DestinationMetadataName, destination);

			if(isAppConfig != null)
				file.SetMetadata(XmlTransformationDecoratedTaskItemExtension.IsAppConfigMetadataName, isAppConfig.Value.ToString());

			if(objective != null)
				file.SetMetadata(XmlTransformationDecoratedTaskItemExtension.ObjectiveMetadataName, objective);

			if(transform != null)
				file.SetMetadata(XmlTransformationDecoratedTaskItemExtension.TransformMetadataName, transform);

			return file;
		}

		public static IEnumerable<ITaskItem> CreateFiles(IEnumerable<string> fileNames)
		{
			var files = new List<ITaskItem>();

			if(fileNames == null)
				return files.ToArray();

			foreach(var fileName in fileNames)
			{
				if(fileName == null)
					throw new ArgumentException("The file-names parameter can not contain null values.", "fileNames");

				files.Add(new TaskItem(fileName));
			}

			return files.ToArray();
		}

		#endregion
	}
}