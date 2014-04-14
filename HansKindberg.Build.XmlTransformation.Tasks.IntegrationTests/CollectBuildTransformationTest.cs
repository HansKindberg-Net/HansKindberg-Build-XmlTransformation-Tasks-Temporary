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
	public class CollectBuildTransformationTest
	{
		#region Fields

		private static readonly string _source = Path.Combine(Project.SolutionDirectory, @"packages\SomePackage\Configuration\Web.Template.config");

		#endregion

		#region Methods

		[TestMethod]
		public void Execute_ShouldWorkProperly()
		{
			var xmlTransformationMaps = new List<ITaskItem>
			{
				XmlTransformationMapHelper.CreateXmlTransformationMap("Web.config", "Web.Build.config", "Web.Publish.config", _source)
			};

			var collectBuildTransformation = new CollectBuildTransformation
			{
				DestinationDirectory = Project.XmlTransformationBuildIntermediateLocation,
				Files = FileHelper.CreateFiles(new[] {"Web.config"}).ToArray(),
				TransformName = "Debug",
				XmlFileExtensions = XmlFileExtensionHelper.CreateXmlFileExtensions(new[] {".config", ".xml"}).ToArray(),
				XmlTransformationMaps = xmlTransformationMaps.ToArray()
			};

			var buildEngine = new BuildEngineFake();

			collectBuildTransformation.BuildEngine = buildEngine;

			Assert.IsTrue(collectBuildTransformation.Execute());

			var filesToTransform = collectBuildTransformation.FilesToTransform;

			Assert.AreEqual(2, filesToTransform.Count());

			var expectedDestination = Path.Combine(Project.XmlTransformationBuildIntermediateLocation, "Web.config");

			var firstFileToTransform = filesToTransform.ElementAt(0);

			Assert.AreEqual(_source, firstFileToTransform.ItemSpec);
			Assert.AreEqual(expectedDestination, firstFileToTransform.Destination());
			// ReSharper disable PossibleInvalidOperationException
			Assert.IsFalse(firstFileToTransform.IsAppConfig());
			// ReSharper restore PossibleInvalidOperationException
			Assert.AreEqual("Web.Build.config", firstFileToTransform.Transform());

			var secondFileToTransform = filesToTransform.ElementAt(1);

			Assert.AreEqual(expectedDestination, secondFileToTransform.ItemSpec);
			Assert.AreEqual(expectedDestination, secondFileToTransform.Destination());
			// ReSharper disable PossibleInvalidOperationException
			Assert.IsFalse(secondFileToTransform.IsAppConfig());
			// ReSharper restore PossibleInvalidOperationException
			Assert.AreEqual("Web.Debug.config", secondFileToTransform.Transform());
		}

		#endregion
	}
}