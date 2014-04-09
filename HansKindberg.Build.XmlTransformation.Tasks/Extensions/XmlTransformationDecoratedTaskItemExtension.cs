using System;
using Microsoft.Build.Framework;

namespace HansKindberg.Build.XmlTransformation.Tasks.Extensions
{
	public static class XmlTransformationDecoratedTaskItemExtension
	{
		#region Fields

		private const string _destinationIsValidMetadataName = "DestinationIsValid";
		private const string _destinationMetadataName = "Destination";
		private const string _isAppConfigMetadataName = "IsAppConfig";
		private const string _isValidMetadataName = "IsValid";
		private const string _originalItemSpecificationMetadataName = "OriginalItemSpecification";
		private const string _preTransformIsValidMetadataName = "PreTransformIsValid";
		private const string _preTransformMetadataName = "PreTransform";
		private const string _sourceIsValidMetadataName = "SourceIsValid";
		private const string _sourceMetadataName = "Source";
		private const string _transformIsValidMetadataName = "TransformIsValid";
		private const string _transformMetadataName = "Transform";

		#endregion

		#region Methods

		public static string Destination(this ITaskItem taskItem)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			return taskItem.GetMetadata(_destinationMetadataName);
		}

		public static void Destination(this ITaskItem taskItem, string value)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			taskItem.SetMetadata(_destinationMetadataName, value);
		}

		public static bool DestinationIsValid(this ITaskItem taskItem)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			bool destinationIsValid;

			return bool.TryParse(taskItem.GetMetadata(_destinationIsValidMetadataName), out destinationIsValid) && destinationIsValid;
		}

		public static void DestinationIsValid(this ITaskItem taskItem, bool value)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			taskItem.SetMetadata(_destinationIsValidMetadataName, value.ToString());
		}

		public static bool IsAppConfig(this ITaskItem taskItem)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			bool isAppConfig;

			return bool.TryParse(taskItem.GetMetadata(_isAppConfigMetadataName), out isAppConfig) && isAppConfig;
		}

		public static void IsAppConfig(this ITaskItem taskItem, bool value)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			taskItem.SetMetadata(_isAppConfigMetadataName, value.ToString());
		}

		public static bool IsValid(this ITaskItem taskItem)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			bool isValid;

			return bool.TryParse(taskItem.GetMetadata(_isValidMetadataName), out isValid) && isValid;
		}

		public static void IsValid(this ITaskItem taskItem, bool value)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			taskItem.SetMetadata(_isValidMetadataName, value.ToString());
		}

		public static string OriginalItemSpecification(this ITaskItem taskItem)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			return taskItem.GetMetadata(_originalItemSpecificationMetadataName);
		}

		public static void OriginalItemSpecification(this ITaskItem taskItem, string value)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			taskItem.SetMetadata(_originalItemSpecificationMetadataName, value);
		}

		public static string PreTransform(this ITaskItem taskItem)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			return taskItem.GetMetadata(_preTransformMetadataName);
		}

		public static void PreTransform(this ITaskItem taskItem, string value)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			taskItem.SetMetadata(_preTransformMetadataName, value);
		}

		public static bool PreTransformIsValid(this ITaskItem taskItem)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			bool preTransformIsValid;

			return bool.TryParse(taskItem.GetMetadata(_preTransformIsValidMetadataName), out preTransformIsValid) && preTransformIsValid;
		}

		public static void PreTransformIsValid(this ITaskItem taskItem, bool value)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			taskItem.SetMetadata(_preTransformIsValidMetadataName, value.ToString());
		}

		public static void RemovePreTransform(this ITaskItem taskItem)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			taskItem.RemoveMetadata(_preTransformMetadataName);
		}

		public static void RemovePreTransformIsValid(this ITaskItem taskItem)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			taskItem.RemoveMetadata(_preTransformIsValidMetadataName);
		}

		public static void RemoveSource(this ITaskItem taskItem)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			taskItem.RemoveMetadata(_sourceMetadataName);
		}

		public static void RemoveSourceIsValid(this ITaskItem taskItem)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			taskItem.RemoveMetadata(_sourceIsValidMetadataName);
		}

		public static string Source(this ITaskItem taskItem)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			return taskItem.GetMetadata(_sourceMetadataName);
		}

		public static void Source(this ITaskItem taskItem, string value)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			taskItem.SetMetadata(_sourceMetadataName, value);
		}

		public static bool SourceIsValid(this ITaskItem taskItem)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			bool sourceIsValid;

			return bool.TryParse(taskItem.GetMetadata(_sourceIsValidMetadataName), out sourceIsValid) && sourceIsValid;
		}

		public static void SourceIsValid(this ITaskItem taskItem, bool value)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			taskItem.SetMetadata(_sourceIsValidMetadataName, value.ToString());
		}

		public static string Transform(this ITaskItem taskItem)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			return taskItem.GetMetadata(_transformMetadataName);
		}

		public static void Transform(this ITaskItem taskItem, string value)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			taskItem.SetMetadata(_transformMetadataName, value);
		}

		public static bool TransformIsValid(this ITaskItem taskItem)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			bool transformIsValid;

			return bool.TryParse(taskItem.GetMetadata(_transformIsValidMetadataName), out transformIsValid) && transformIsValid;
		}

		public static void TransformIsValid(this ITaskItem taskItem, bool value)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			taskItem.SetMetadata(_transformIsValidMetadataName, value.ToString());
		}

		#endregion
	}
}