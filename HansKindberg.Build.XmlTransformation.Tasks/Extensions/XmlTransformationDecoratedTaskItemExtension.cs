using System;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace HansKindberg.Build.XmlTransformation.Tasks.Extensions
{
	public static class XmlTransformationDecoratedTaskItemExtension
	{
		#region Fields

		public const string DestinationMetadataName = "Destination";
		public const string FirstTransformMetadataName = "FirstTransform";
		public const string IsAppConfigMetadataName = "IsAppConfig";
		public const string ObjectiveMetadataName = "Objective";
		public const string TransformsExceptFirstMetadataName = "TransformsExceptFirst";
		public const string TransformsMetadataName = "Transforms";

		#endregion

		#region Methods

		public static string Destination(this ITaskItem taskItem)
		{
			return taskItem.GetString(DestinationMetadataName);
		}

		public static void Destination(this ITaskItem taskItem, string value)
		{
			taskItem.SetString(DestinationMetadataName, value);
		}

		public static string FirstTransform(this ITaskItem taskItem)
		{
			return taskItem.GetString(FirstTransformMetadataName);
		}

		public static void FirstTransform(this ITaskItem taskItem, string value)
		{
			taskItem.SetString(FirstTransformMetadataName, value);
		}

		private static bool? GetNullableBoolean(this ITaskItem taskItem, string metadataName)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			if(metadataName == null)
				throw new ArgumentNullException("metadataName");

			if(!taskItem.MetadataNames.Cast<string>().Contains(metadataName))
				return null;

			bool value;

			if(bool.TryParse(taskItem.GetMetadata(metadataName), out value))
				return value;

			return null;
		}

		private static string GetString(this ITaskItem taskItem, string metadataName)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			if(metadataName == null)
				throw new ArgumentNullException("metadataName");

			return taskItem.MetadataNames.Cast<string>().Contains(metadataName) ? taskItem.GetMetadata(metadataName) : null;
		}

		public static bool IsAppConfig(this ITaskItem taskItem)
		{
			var isAppConfig = taskItem.GetNullableBoolean(IsAppConfigMetadataName);

			if(!isAppConfig.HasValue)
			{
				isAppConfig = taskItem.ItemSpec.Equals("App.config", StringComparison.OrdinalIgnoreCase);
				taskItem.SetNullableBoolean(IsAppConfigMetadataName, isAppConfig);
			}

			return isAppConfig.Value;
		}

		public static ITaskItem Objective(this ITaskItem taskItem)
		{
			var objective = taskItem.GetString(ObjectiveMetadataName);

			return objective == null ? null : new TaskItem(objective);
		}

		public static void Objective(this ITaskItem taskItem, string value)
		{
			taskItem.SetString(ObjectiveMetadataName, value);
		}

		private static void SetNullableBoolean(this ITaskItem taskItem, string metadataName, bool? value)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			if(metadataName == null)
				throw new ArgumentNullException("metadataName");

			if(value == null)
				taskItem.RemoveMetadata(metadataName);
			else
				taskItem.SetMetadata(metadataName, value.ToString());
		}

		private static void SetString(this ITaskItem taskItem, string metadataName, string value)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			if(metadataName == null)
				throw new ArgumentNullException("metadataName");

			if(value == null)
				taskItem.RemoveMetadata(metadataName);
			else
				taskItem.SetMetadata(metadataName, value);
		}

		public static string Transforms(this ITaskItem taskItem)
		{
			return taskItem.GetString(TransformsMetadataName);
		}

		public static void Transforms(this ITaskItem taskItem, string value)
		{
			taskItem.SetString(TransformsMetadataName, value);
		}

		public static string TransformsExceptFirst(this ITaskItem taskItem)
		{
			return taskItem.GetString(TransformsExceptFirstMetadataName);
		}

		public static void TransformsExceptFirst(this ITaskItem taskItem, string value)
		{
			taskItem.SetString(TransformsExceptFirstMetadataName, value);
		}

		#endregion
	}
}