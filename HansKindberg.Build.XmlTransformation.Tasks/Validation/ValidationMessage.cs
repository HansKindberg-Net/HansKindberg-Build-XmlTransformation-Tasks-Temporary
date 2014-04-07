namespace HansKindberg.Build.XmlTransformation.Tasks.Validation
{
	public abstract class ValidationMessage
	{
		#region Fields

		private readonly string _information;

		#endregion

		#region Constructors

		protected ValidationMessage(string information)
		{
			this._information = information ?? string.Empty;
		}

		#endregion

		#region Properties

		public virtual string Information
		{
			get { return this._information; }
		}

		#endregion
	}
}