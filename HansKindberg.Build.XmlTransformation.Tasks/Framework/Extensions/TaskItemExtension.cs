using System;
using Microsoft.Build.Framework;

// ReSharper disable CheckNamespace

namespace HansKindberg.Build.Framework.Extensions // ReSharper restore CheckNamespace
{
	public static class TaskItemExtension
	{
		#region Methods

		public static string DestinationRelativePath(this ITaskItem taskItem)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			return taskItem.GetMetadata("DestinationRelativePath") ?? string.Empty;
		}

		public static string Extension(this ITaskItem taskItem)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			return taskItem.GetMetadata("Extension") ?? string.Empty;
		}

		public static string FullPath(this ITaskItem taskItem)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			return taskItem.GetMetadata("FullPath") ?? string.Empty;
		}

		public static bool IsAppConfig(this ITaskItem taskItem)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			return taskItem.ItemSpec.Equals("App.config", StringComparison.OrdinalIgnoreCase);
		}

		#endregion
	}
}