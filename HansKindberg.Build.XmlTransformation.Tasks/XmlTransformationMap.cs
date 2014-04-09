using System;
using System.Collections;
using Microsoft.Build.Framework;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public class XmlTransformationMap : IXmlTransformationMap
	{
		#region Fields

		private readonly string _commonBuildTransform;
		private readonly string _commonPublishTransform;
		private readonly string _source;
		private readonly ITaskItem _taskItem;

		#endregion

		#region Constructors

		public XmlTransformationMap(ITaskItem taskItem)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			this._commonBuildTransform = taskItem.GetMetadata("CommonBuildTransform");
			this._commonPublishTransform = taskItem.GetMetadata("CommonPublishTransform");
			this._source = taskItem.GetMetadata("Source");
			this._taskItem = taskItem;
		}

		#endregion

		#region Properties

		public virtual string CommonBuildTransform
		{
			get { return this._commonBuildTransform; }
		}

		public virtual string CommonPublishTransform
		{
			get { return this._commonPublishTransform; }
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

		public virtual string Source
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