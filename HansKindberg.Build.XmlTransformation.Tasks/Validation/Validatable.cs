using System;

namespace HansKindberg.Build.XmlTransformation.Tasks.Validation
{
	public class Validatable<T> : IValidatable<T>
	{
		#region Properties

		public virtual Exception Exception { get; set; }

		public virtual bool IsValid
		{
			get { return this.Exception == null; }
		}

		public virtual T Value { get; set; }

		#endregion
	}
}