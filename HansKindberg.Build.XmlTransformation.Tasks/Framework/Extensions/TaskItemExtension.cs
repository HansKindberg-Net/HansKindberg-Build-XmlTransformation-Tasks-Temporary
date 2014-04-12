using System;
using Microsoft.Build.Framework;

// ReSharper disable CheckNamespace

namespace HansKindberg.Build.Framework.Extensions // ReSharper restore CheckNamespace
{
	public static class TaskItemExtension
	{
		#region Methods

		public static string DependentUpon(this ITaskItem taskItem)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			return taskItem.GetMetadata("DependentUpon");
		}

		public static string DestinationRelativePath(this ITaskItem taskItem)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			return taskItem.GetMetadata("DestinationRelativePath");
		}

		public static string Extension(this ITaskItem taskItem)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			return taskItem.GetMetadata("Extension");
		}

		public static string FullPath(this ITaskItem taskItem)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			return taskItem.GetMetadata("FullPath");
		}

		#endregion
	}
}