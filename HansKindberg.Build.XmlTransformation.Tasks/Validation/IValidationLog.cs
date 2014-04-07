using System.Collections.Generic;

namespace HansKindberg.Build.XmlTransformation.Tasks.Validation
{
	public interface IValidationLog
	{
		#region Properties

		IEnumerable<ValidationMessage> ValidationMessages { get; }

		#endregion

		#region Methods

		void LogError(string information);
		void LogWarning(string information);

		#endregion
	}
}