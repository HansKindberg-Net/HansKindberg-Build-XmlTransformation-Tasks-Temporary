﻿using Microsoft.Build.Framework;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public interface IPotentialFile
	{
		#region Properties

		bool Exists { get; }
		string Extension { get; }
		string FullName { get; }
		bool IsFile { get; }
		string Name { get; }
		string OriginalPath { get; }
		ITaskItem TaskItem { get; }

		#endregion
	}
}