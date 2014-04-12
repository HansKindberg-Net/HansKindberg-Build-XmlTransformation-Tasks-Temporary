using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyCompany("Hans Kindberg - open source")]
[assembly: AssemblyConfiguration(
#if DEBUG
	"Debug"
#else
	"Release"
#endif
	)]
/*
	The third position in the version (AssemblyVersion) number is used for the status of the release:
	0 for alpha (status)
	1 for beta (status)
	2 for release candidate
	3 for (final) release

	For instance:
	1.2.0.1 instead of 1.2-a1
	1.2.1.2 instead of 1.2-b2 (beta with some bug fixes)
	1.2.2.3 instead of 1.2-rc3 (release candidate)
	1.2.3.0 instead of 1.2-r (commercial distribution)
	1.2.3.5 instead of 1.2-r5 (commercial distribution with many bug fixes)

	http://en.wikipedia.org/wiki/Software_versioning#Designating_development_stage

	To get nuget pack to work for prereleases and AssemblyInformationalVersion you must follow the guidlines at http://semver.org/.
	Examples:
	1.0.0-alpha < 1.0.0-alpha.1 < 1.0.0-beta.2 < 1.0.0-beta.11 < 1.0.0-rc.1 < 1.0.0-rc.1+build.1 < 1.0.0 < 1.0.0+0.3.7 < 1.3.7+build < 1.3.7+build.2.b8f12d7 < 1.3.7+build.11.e0f985a
	My examples:
	1.0.0-alpha-1 = 1.0.0.1
	1.0.0-alpha-2 = 1.0.0.2
	1.0.1-beta-1 = 1.0.1.1
	1.0.1-beta-2 = 1.0.1.2
	1.0.2-rc-1 = 1.0.2.1
	1.0.2-rc-2 = 1.0.2.2
	1.0.3 = 1.0.3.1 or 1.0.3.2 etc. (when its the real release the version number will tell the exact version, no literal information added, in other words I dont use 1.0.3-r-1, 1.0.3-r-2 etc.)

	If we sometime want to increment just the fileversion we do it by adding a filerevision at the end:
	Version 1.0.0.1 has file version 1.0.0.11, if we want to fix without incrementing the version the file version becomes 1.0.0.12
*/

[assembly: AssemblyInformationalVersion("1.0.0-alpha-0")]
#pragma warning disable 436

[assembly: AssemblyProduct(AssemblyInfo.AssemblyName)]
[assembly: AssemblyTitle(AssemblyInfo.AssemblyName)]
#pragma warning restore 436

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: ComVisible(false)]