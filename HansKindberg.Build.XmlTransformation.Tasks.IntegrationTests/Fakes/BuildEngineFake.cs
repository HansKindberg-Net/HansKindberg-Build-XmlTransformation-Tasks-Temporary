using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Framework;

namespace HansKindberg.Build.XmlTransformation.Tasks.IntegrationTests.Fakes
{
	public class BuildEngineFake : IBuildEngine
	{
		#region Fields

		private readonly IList<string> _logMessages = new List<string>();

		#endregion

		#region Properties

		public virtual int ColumnNumberOfTaskNode
		{
			get { return 0; }
		}

		public virtual bool ContinueOnError
		{
			get { return false; }
		}

		public virtual int LineNumberOfTaskNode
		{
			get { return 0; }
		}

		public virtual IEnumerable<string> LogMessages
		{
			get { return this.LogMessagesInternal.ToArray(); }
		}

		protected internal virtual IList<string> LogMessagesInternal
		{
			get { return this._logMessages; }
		}

		public virtual string ProjectFileOfTaskNode
		{
			get { return Project.File; }
		}

		#endregion

		#region Methods

		public virtual bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties, IDictionary targetOutputs)
		{
			throw new NotImplementedException();
		}

		public virtual void LogCustomEvent(CustomBuildEventArgs e)
		{
			if(e == null)
				throw new ArgumentNullException("e");

			this.LogMessagesInternal.Add(e.Message);
		}

		public virtual void LogErrorEvent(BuildErrorEventArgs e)
		{
			if(e == null)
				throw new ArgumentNullException("e");

			this.LogMessagesInternal.Add(e.Message);
		}

		public virtual void LogMessageEvent(BuildMessageEventArgs e)
		{
			if(e == null)
				throw new ArgumentNullException("e");

			this.LogMessagesInternal.Add(e.Message);
		}

		public virtual void LogWarningEvent(BuildWarningEventArgs e)
		{
			if(e == null)
				throw new ArgumentNullException("e");

			this.LogMessagesInternal.Add(e.Message);
		}

		#endregion
	}
}