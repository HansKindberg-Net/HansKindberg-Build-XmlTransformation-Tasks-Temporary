using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using HansKindberg.Build.XmlTransformation.Tasks.IntegrationTests.Fakes;
using HansKindberg.Build.XmlTransformation.Tasks.IntegrationTests.Helpers;
using HansKindberg.Build.XmlTransformation.Tasks.IoC;
using Microsoft.Build.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HansKindberg.Build.XmlTransformation.Tasks.IntegrationTests
{
	[TestClass]
	public class CollectXmlTransformationTest
	{
		#region Methods

		private static CollectXmlTransformation CreateCollectXmlTransformation(TransformMode transformMode)
		{
			var collectXmlTransformationMock = new Mock<CollectXmlTransformation>(new object[] {ServiceLocator.Instance.GetService<IFileSystem>(), ServiceLocator.Instance.GetService<IXmlTransformationDecoratorFactory>()}) {CallBase = true};

			collectXmlTransformationMock.Setup(collectXmlTransformation => collectXmlTransformation.TransformMode).Returns(transformMode);

			return collectXmlTransformationMock.Object;
		}

		[TestMethod]
		public void Execute_IfAXmlTransformationMapSourceIsInvalid_ShouldNotAddTheFileToFilesToTransformAndShouldLogInformationAboutIt()
		{
			const string file = "Web.config";
			string invalidSource = Path.Combine(Project.SolutionDirectory, @"packages\SomePackage\Configuration\Web.DoesNotExist.config");

			var xmlTransformationMaps = new List<ITaskItem>
			{
				XmlTransformationMapHelper.CreateXmlTransformationMap(file, "Web.Build.config", "Web.Publish.config", invalidSource)
			};

			var collectXmlTransformation = CreateCollectXmlTransformation(TransformMode.Build);

			collectXmlTransformation.DestinationDirectory = Project.XmlTransformationBuildIntermediateLocation;
			collectXmlTransformation.Files = FileHelper.CreateFiles(new[] {file}).ToArray();
			collectXmlTransformation.TransformName = "Debug";
			collectXmlTransformation.XmlFileExtensions = XmlFileExtensionHelper.CreateDefaultXmlFileExtensions().ToArray();
			collectXmlTransformation.XmlTransformationMaps = xmlTransformationMaps.ToArray();

			var buildEngine = new BuildEngineFake();

			collectXmlTransformation.BuildEngine = buildEngine;

			string expectedLogMessage = string.Format(CultureInfo.InvariantCulture, "The source \"{0}\" for file \"{1}\" does not exist. See the \"XmlTransformationMap\"-itemgroup with identity \"{1}\".", invalidSource, file);

			Assert.IsTrue(collectXmlTransformation.Execute());

			var filesToTransform = collectXmlTransformation.FilesToTransform;

			Assert.AreEqual(0, filesToTransform.Count());

			Assert.IsTrue(buildEngine.LogMessages.Contains(expectedLogMessage));
		}

		#endregion
	}
}