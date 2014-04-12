using System.Collections.Generic;
using System.IO;
using System.Linq;
using HansKindberg.Build.XmlTransformation.Tasks.Extensions;
using HansKindberg.Build.XmlTransformation.Tasks.IntegrationTests.Fakes;
using HansKindberg.Build.XmlTransformation.Tasks.IntegrationTests.Helpers;
using Microsoft.Build.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HansKindberg.Build.XmlTransformation.Tasks.IntegrationTests
{
	[TestClass]
	public class CollectPublishTransformationTest
	{
		#region Fields

		private static readonly string _source = Path.Combine(Project.SolutionDirectory, @"packages\SomePackage\Configuration\Web.Template.config");

		#endregion

		#region Methods

		private static IEnumerable<ITaskItem> CreateProblemFiles()
		{
			const string file = "Web.config";
			var intermediateFile = Path.Combine(Project.XmlTransformationBuildIntermediateLocation, file);

			var files = new List<ITaskItem>
			{
				FileHelper.CreateFile(file),
				FileHelper.CreateFile(intermediateFile, intermediateFile, false, file, "Web.Build.config"),
				FileHelper.CreateFile(intermediateFile, intermediateFile, false, file, "Web.Release.config")
			};

			return files.ToArray();
		}

		[TestMethod]
		public void Test()
		{
			var xmlTransformationMaps = new List<ITaskItem>
			{
				XmlTransformationMapHelper.CreateXmlTransformationMap("Web.config", "Web.Build.config", "Web.Publish.config", _source)
			};

			var collectPublishTransformation = new CollectPublishTransformation
			{
				DestinationDirectory = Project.XmlTransformationPublishIntermediateLocation,
				Files = CreateProblemFiles().ToArray(),
				TransformName = "Production",
				XmlFileExtensions = XmlFileExtensionHelper.CreateXmlFileExtensions(new[] {".config", ".xml"}).ToArray(),
				XmlTransformationMaps = xmlTransformationMaps.ToArray()
			};

			var buildEngine = new BuildEngineFake();

			collectPublishTransformation.BuildEngine = buildEngine;

			Assert.IsTrue(collectPublishTransformation.Execute());

			var filesToTransform = collectPublishTransformation.FilesToTransform;

			Assert.AreEqual(2, filesToTransform.Count());

			var expectedDestination = Path.Combine(Project.XmlTransformationPublishIntermediateLocation, "Web.config");

			var firstFileToTransform = filesToTransform.ElementAt(0);

			var firstExpectedIdentity = Path.Combine(Project.XmlTransformationBuildIntermediateLocation, "Web.config");

			Assert.AreEqual(firstExpectedIdentity, firstFileToTransform.ItemSpec);
			Assert.AreEqual(expectedDestination, firstFileToTransform.Destination());
			// ReSharper disable PossibleInvalidOperationException
			Assert.IsFalse(firstFileToTransform.IsAppConfig());
			// ReSharper restore PossibleInvalidOperationException
			Assert.AreEqual("Web.Publish.config", firstFileToTransform.Transform());

			var secondFileToTransform = filesToTransform.ElementAt(1);

			Assert.AreEqual(expectedDestination, secondFileToTransform.ItemSpec);
			Assert.AreEqual(expectedDestination, secondFileToTransform.Destination());
			// ReSharper disable PossibleInvalidOperationException
			Assert.IsFalse(secondFileToTransform.IsAppConfig());
			// ReSharper restore PossibleInvalidOperationException
			Assert.AreEqual("Web.Production.config", secondFileToTransform.Transform());
		}

		#endregion
	}
}