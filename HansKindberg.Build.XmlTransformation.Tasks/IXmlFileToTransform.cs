namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public interface IXmlFileToTransform
	{
		#region Properties

		bool IsAppConfig { get; }
		IPotentialFile PreTransform { get; }
		bool PreTransformIsValid { get; }
		IPotentialFile Source { get; }
		IPotentialFile Transform { get; }
		bool TransformIsValid { get; }
		IPotentialFile XmlFile { get; }

		#endregion
	}
}