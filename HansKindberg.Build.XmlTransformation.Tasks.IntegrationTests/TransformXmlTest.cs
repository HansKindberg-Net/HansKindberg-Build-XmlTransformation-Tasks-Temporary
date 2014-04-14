using System.IO;
using HansKindberg.Build.XmlTransformation.Tasks.IntegrationTests.Fakes;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HansKindberg.Build.XmlTransformation.Tasks.IntegrationTests
{
	[TestClass]
	public class TransformXmlTest
	{
		#region Fields

		private static readonly string _expectedDirectory = Path.Combine(Project.Directory, "TransformXml-Tests", "Expected");
		private static readonly string _source = Path.Combine(Project.SolutionDirectory, @"packages\SomePackage\Configuration\Web.Template.config");

		#endregion

		#region Methods

		[TestMethod]
		public void Execute_ShouldWorkProperly()
		{
			var destination = new TaskItem(Path.Combine(Directory.GetCurrentDirectory(), "XmlTransformation", "Web.config"));
			var expectedFile = Path.Combine(_expectedDirectory, "Execute-ShouldWorkProperly.Web.config");
			var expected = File.ReadAllText(expectedFile);

			var transformXml = new TransformXml();
			var buildEngine = new BuildEngineFake();
			transformXml.BuildEngine = buildEngine;
			transformXml.Destination = new TaskItem(destination);
			transformXml.Source = new TaskItem(_source);
			transformXml.Transforms = new ITaskItem[] {new TaskItem("Web.Build.config"), new TaskItem("Web.Release.config")};

			Assert.IsTrue(transformXml.Execute());

			var actual = File.ReadAllText(destination.ItemSpec);

			Assert.AreEqual(expected, actual);
		}

		#endregion
	}
}