﻿namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public interface IXmlTransformationMap
	{
		#region Properties

		IPotentialFile CommonBuildTransform { get; }
		IPotentialFile CommonPublishTransform { get; }
		IPotentialFile Source { get; }

		#endregion
	}
}