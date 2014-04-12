using System;
using System.Collections.Generic;
using Microsoft.Build.Framework;

namespace HansKindberg.Build.XmlTransformation.Tasks.Framework
{
	public class TaskItemComparer : IEqualityComparer<ITaskItem>
	{
		#region Methods

		public virtual bool Equals(ITaskItem x, ITaskItem y)
		{
			if(x == null || y == null)
				return false;

			return x.ItemSpec.Equals(y.ItemSpec, StringComparison.OrdinalIgnoreCase);
		}

		public virtual int GetHashCode(ITaskItem obj)
		{
			if(obj == null)
				throw new ArgumentNullException("obj");

			return obj.ItemSpec.GetHashCode();
		}

		#endregion
	}
}