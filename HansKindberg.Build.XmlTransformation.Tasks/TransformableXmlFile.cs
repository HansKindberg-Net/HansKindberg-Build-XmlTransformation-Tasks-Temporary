using System;
using System.Collections;
using HansKindberg.Build.Framework.Extensions;
using Microsoft.Build.Framework;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public class TransformableXmlFile : ITransformableXmlFile
	{
		#region Fields

		private IPotentialFile _destination;
		private bool _isAppConfig;
		private IPotentialFile _preTransform;
		private IPotentialFile _source;
		private readonly ITaskItem _taskItem;
		private IPotentialFile _transform;

		#endregion

		#region Constructors

		public TransformableXmlFile(ITaskItem taskItem)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			this._taskItem = taskItem;
			this.SetIsAppConfig();
		}

		#endregion

		#region Properties

		public virtual IPotentialFile Destination
		{
			get { return this._destination; }
			set
			{
				this.SetMetadata("Destination", value);
				this._destination = value;
			}
		}

		public virtual bool IsAppConfig
		{
			get { return this._isAppConfig; }
		}

		public virtual string ItemSpec
		{
			get { return this.TaskItem.ItemSpec; }
			set
			{
				this.TaskItem.ItemSpec = value;
				this.SetIsAppConfig();
			}
		}

		public virtual int MetadataCount
		{
			get { return this.TaskItem.MetadataCount; }
		}

		public virtual ICollection MetadataNames
		{
			get { return this.TaskItem.MetadataNames; }
		}

		public virtual IPotentialFile PreTransform
		{
			get { return this._preTransform; }
			set
			{
				this.SetMetadata("PreTransform", value);
				this._preTransform = value;
			}
		}

		public virtual IPotentialFile Source
		{
			get { return this._source; }
			set
			{
				this.SetMetadata("Source", value);
				this._source = value;
			}
		}

		public virtual ITaskItem TaskItem
		{
			get { return this._taskItem; }
		}

		public virtual IPotentialFile Transform
		{
			get { return this._transform; }
			set
			{
				this.SetMetadata("Transform", value);
				this._transform = value;
			}
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

		private void SetIsAppConfig()
		{
			this._taskItem.SetMetadata("IsAppConfig", this._taskItem.IsAppConfig().ToString());
			this._isAppConfig = this._taskItem.IsAppConfig();
		}

		protected internal virtual void SetMetadata(string metadataName, IPotentialFile potentialFile)
		{
			string value = potentialFile != null ? potentialFile.OriginalPath : string.Empty;
			this.SetMetadata(metadataName, value);
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