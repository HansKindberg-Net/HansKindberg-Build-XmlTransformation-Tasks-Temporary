using System;
using System.Collections;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public class XmlTransformationMap : IXmlTransformationMap
	{
		#region Fields

		public const string GeneralBuildTransformMetadataName = "GeneralBuildTransform";
		public const string GeneralPublishTransformMetadataName = "GeneralPublishTransform";
		public const string SourceMetadataName = "Source";
		private readonly ITaskItem _generalBuildTransform;
		private readonly ITaskItem _generalPublishTransform;
		private readonly ITaskItem _source;
		private readonly ITaskItem _taskItem;

		#endregion

		#region Constructors

		public XmlTransformationMap(ITaskItem taskItem)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			this._generalBuildTransform = new TaskItem(taskItem.GetMetadata(GeneralBuildTransformMetadataName));
			this._generalPublishTransform = new TaskItem(taskItem.GetMetadata(GeneralPublishTransformMetadataName));
			this._source = new TaskItem(taskItem.GetMetadata(SourceMetadataName));
			this._taskItem = taskItem;
		}

		#endregion

		#region Properties

		public virtual ITaskItem GeneralBuildTransform
		{
			get { return this._generalBuildTransform; }
		}

		public virtual ITaskItem GeneralPublishTransform
		{
			get { return this._generalPublishTransform; }
		}

		public virtual string ItemSpec
		{
			get { return this.TaskItem.ItemSpec; }
			set { this.TaskItem.ItemSpec = value; }
		}

		public virtual int MetadataCount
		{
			get { return this.TaskItem.MetadataCount; }
		}

		public virtual ICollection MetadataNames
		{
			get { return this.TaskItem.MetadataNames; }
		}

		public virtual ITaskItem Source
		{
			get { return this._source; }
		}

		protected internal virtual ITaskItem TaskItem
		{
			get { return this._taskItem; }
		}

		#endregion

		#region Methods

		public virtual IDictionary CloneCustomMetadata()
		{
			return this.TaskItem.CloneCustomMetadata();
		}

		public virtual void CopyMetadataTo(ITaskItem destinationItem)
		{
			this.TaskItem.CopyMetadataTo(destinationItem);
		}

		public virtual string GetMetadata(string metadataName)
		{
			return this.TaskItem.GetMetadata(metadataName);
		}

		public virtual void RemoveMetadata(string metadataName)
		{
			this.TaskItem.RemoveMetadata(metadataName);
		}

		public virtual void SetMetadata(string metadataName, string metadataValue)
		{
			this.TaskItem.SetMetadata(metadataName, metadataValue);
		}

		public override string ToString()
		{
			return this.TaskItem.ToString();
		}

		#endregion
	}
}