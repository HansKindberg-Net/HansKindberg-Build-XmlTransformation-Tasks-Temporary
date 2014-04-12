using System;
using System.IO;

namespace HansKindberg.Build.XmlTransformation.Tasks.IntegrationTests
{
	public static class Project
	{
		#region Fields

		private static readonly string _directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
		private static readonly string _file = Path.Combine(_directory, "HansKindberg.Build.XmlTransformation.Tasks.IntegrationTests.csproj");
		private const string _intermediateLocation = "obj";
		private static readonly string _solutionDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
		private static readonly string _xmlTransformationBuildIntermediateLocation = Path.Combine(_intermediateLocation, @"XmlTransformation\transformed");
		private static readonly string _xmlTransformationPublishIntermediateLocation = Path.Combine(_intermediateLocation, @"ProfileXmlTransformation\transformed");

		#endregion

		#region Properties

		public static string Directory
		{
			get { return _directory; }
		}

		public static string File
		{
			get { return _file; }
		}

		public static string SolutionDirectory
		{
			get { return _solutionDirectory; }
		}

		public static string XmlTransformationBuildIntermediateLocation
		{
			get { return _xmlTransformationBuildIntermediateLocation; }
		}

		public static string XmlTransformationPublishIntermediateLocation
		{
			get { return _xmlTransformationPublishIntermediateLocation; }
		}

		#endregion
	}
}