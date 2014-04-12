using System;

namespace HansKindberg.Build.XmlTransformation.Tasks.Validation
{
	public interface IValidatable<out T>
	{
		#region Properties

		Exception Exception { get; }
		bool IsValid { get; }
		T Value { get; }

		#endregion
	}
}