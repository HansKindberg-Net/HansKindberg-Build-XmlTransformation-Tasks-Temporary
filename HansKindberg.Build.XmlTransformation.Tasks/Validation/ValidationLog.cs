using System.Collections.Generic;
using System.Linq;

namespace HansKindberg.Build.XmlTransformation.Tasks.Validation
{
	public class ValidationLog : IValidationLog
	{
		#region Fields

		private readonly IList<ValidationMessage> _validationMessages = new List<ValidationMessage>();

		#endregion

		#region Properties

		public virtual IEnumerable<ValidationMessage> ValidationMessages
		{
			get { return this.ValidationMessagesInternal.ToArray(); }
		}

		protected internal virtual IList<ValidationMessage> ValidationMessagesInternal
		{
			get { return this._validationMessages; }
		}

		#endregion

		#region Methods

		public virtual void LogError(string information)
		{
			this.ValidationMessagesInternal.Add(new ValidationError(information));
		}

		public virtual void LogWarning(string information)
		{
			this.ValidationMessagesInternal.Add(new ValidationWarning(information));
		}

		#endregion
	}
}