namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public interface ITransformableXmlFile
	{
		#region Properties

		IPotentialFile Identity { get; }
		bool IsAppConfig { get; }
		IPotentialFile PreTransform { get; }
		bool PreTransformIsValid { get; }
		IPotentialFile Source { get; }
		IPotentialFile Transform { get; }
		bool TransformIsValid { get; }

		#endregion
	}
}