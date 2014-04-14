using System;
using HansKindberg.Build.XmlTransformation.Tasks.Validation;
using Microsoft.Build.Framework;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public abstract class Task : Microsoft.Build.Utilities.Task
	{
		#region Fields

		private MessageImportance _logImportance = MessageImportance.Normal;
		private readonly IValidationLog _validationLog = new ValidationLog();
		private ValidationMode _validationMode = Validation.ValidationMode.Warning;

		#endregion

		#region Properties

		public virtual string LogImportance
		{
			get { return this.LogImportanceInternal.ToString(); }
			set { this.LogImportanceInternal = (MessageImportance) Enum.Parse(typeof(MessageImportance), value, true); }
		}

		protected internal virtual MessageImportance LogImportanceInternal
		{
			get { return this._logImportance; }
			set { this._logImportance = value; }
		}

		protected internal virtual IValidationLog ValidationLog
		{
			get { return this._validationLog; }
		}

		public virtual string ValidationMode
		{
			get { return this.ValidationModeInternal.ToString(); }
			set { this.ValidationModeInternal = (ValidationMode) Enum.Parse(typeof(ValidationMode), value, true); }
		}

		protected internal virtual ValidationMode ValidationModeInternal
		{
			get { return this._validationMode; }
			set { this._validationMode = value; }
		}

		#endregion

		#region Methods

		protected internal virtual void LogValidationMessage(ValidationMessage validationMessage)
		{
			if(validationMessage == null)
				throw new ArgumentNullException("validationMessage");

			if(this.ValidationModeInternal == Validation.ValidationMode.Message)
			{
				this.Log.LogMessage(validationMessage.Information);
				return;
			}

			if(this.ValidationModeInternal == Validation.ValidationMode.Warning)
			{
				this.Log.LogWarning(validationMessage.Information);
				return;
			}

			if(validationMessage is ValidationError)
			{
				this.Log.LogError(validationMessage.Information);
				return;
			}

			this.Log.LogWarning(validationMessage.Information);
		}

		protected internal virtual bool TransferValidationLog()
		{
			var thereAreValidationErrors = false;

			foreach(var validationMessage in this.ValidationLog.ValidationMessages)
			{
				if(validationMessage is ValidationError)
					thereAreValidationErrors = true;

				this.LogValidationMessage(validationMessage);
			}

			return !(thereAreValidationErrors && this.ValidationModeInternal == Validation.ValidationMode.Error);
		}

		#endregion
	}
}