// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Assets.Xbox_Live.Editor
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.XPath;

    using Microsoft.Xbox.Services;

    using SyntaxTree.VisualStudio.Unity.Bridge;

    using UnityEditor;
    using UnityEditor.Callbacks;

    using UnityEngine;

    /// <summary>
    /// Handles post processing the generated Visaul Studio projects in order to deal with DevCenter
    /// association and including Xbox Live configuration files.
    /// </summary>
    [InitializeOnLoad]
    public class XboxLivePostProcessing
    {
        static XboxLivePostProcessing()
        {
            ProjectFilesGenerator.ProjectFileGeneration += AddXboxServicesConfig;
        }

        /// <summary>
        /// Adds the XboxServices.config file to the generated project file.
        /// </summary>
        /// <param name="fileName">The name of the file being processed.</param>
        /// <param name="fileContent">The content of the file being processed.</param>
        /// <returns>The project file with an additional content element included.</returns>
        /// <remarks>
        /// This only modifies the Unity debug projects and will not have any
        /// effect on projects built as part of the Unity UWP build process.
        /// </remarks>
        private static string AddXboxServicesConfig(string fileName, string fileContent)
        {
            if (!fileName.EndsWith(".Editor.csproj"))
            {
                const string configFileElement =
                    "    <Content Include=\"" + XboxLiveAppConfiguration.FileName + "\">\r\n" +
                    "       <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>\r\n" +
                    "    </Content>\r\n";

                // Hacky way to do this for now.  Should make it a bit more stable.
                int lastItemGroup = fileContent.LastIndexOf("  </ItemGroup>", StringComparison.OrdinalIgnoreCase);
                fileContent = fileContent.Insert(lastItemGroup, configFileElement);
            }

            return fileContent;
        }

        [PostProcessBuild(1)]
        public static void OnPostprocessBuild(BuildTarget target, string solutionFolder)
        {
            // When we're building for UWP we need to do some extra stuff.
            if (target == BuildTarget.WSAPlayer)
            {
                XboxLiveConfigurationEditor editorWindow = EditorWindow.GetWindow<XboxLiveConfigurationEditor>("Xbox Live");

                if (editorWindow.configuration == null || editorWindow.configuration.TitleId == 0)
                {
                    Debug.LogWarning("Unable to find any Xbox Live configuration files so Xbox Live will not be enabled on UWP project.\r\nOpen \"Xbox Live > Configuration\" and click \"Run Xbox Live Association Wizard\" to configure for Xbox Live access.");
                    return;
                }

                string projectFolder = Path.Combine(solutionFolder, Application.productName);
                UpdateProjectFile(projectFolder, editorWindow.configuration);
                UpdateAppxManifest(projectFolder, editorWindow.configuration);
            }
        }

        private static void CopyConfigurationFile(string filename, string projectFolder)
        {
            File.Copy(
                Path.Combine(Application.dataPath, "../" + filename),
                Path.Combine(projectFolder, filename),
                true);
        }

        private static void UpdateProjectFile(string projectFolder, XboxLiveAppConfiguration configuration)
        {
            string projectFile = Path.Combine(projectFolder, Application.productName + ".csproj");
            XDocument project = XDocument.Load(projectFile);

            XNamespace msb = "http://schemas.microsoft.com/developer/msbuild/2003";
            XmlNamespaceManager ns = new XmlNamespaceManager(new NameTable());
            ns.AddNamespace("msb", msb.NamespaceName);

            XElement identityItemGroup = project.XPathSelectElement("msb:Project/msb:ItemGroup[msb:AppxManifest]", ns);
            XElement certificateElement = identityItemGroup.XPathSelectElement("./msb:None[@Include='WSATestCertificate.pfx']", ns);

            if (certificateElement != null)
            {
                string publisherCertificateFileName = configuration.PackageIdentityName + "_StoreKey.pfx";
                CopyConfigurationFile(publisherCertificateFileName, projectFolder);

                // Replace the existing test cert with our generated cert.
                certificateElement.Attribute("Include").Value = publisherCertificateFileName;

                // And update the PackageCertificateKeyFile
                project.XPathSelectElement("msb:Project/msb:PropertyGroup/msb:PackageCertificateKeyFile", ns).Value = publisherCertificateFileName;
            }

            CopyConfigurationFile(XboxLiveAppConfiguration.FileName, projectFolder);

            // Add the XboxService.config file if it doesn't exist 
            if (identityItemGroup.XPathSelectElement("msb:Content[@Include='XboxServices.config']", ns) == null)
            {
                identityItemGroup.Add(
                    new XElement(msb + "Content",
                        new XAttribute("Include", "XboxServices.config"),
                        new XElement(msb + "CopyToOutputDirectory", "PreserveNewest")));
            }

            project.Save(projectFile);
        }

        private static void UpdateAppxManifest(string projectFolder, XboxLiveAppConfiguration configuration)
        {
            string manifestFile = Path.Combine(projectFolder, "Package.appxmanifest");
            XDocument manifest = XDocument.Load(manifestFile);

            XmlNamespaceManager ns = new XmlNamespaceManager(new NameTable());
            XNamespace m = "http://schemas.microsoft.com/appx/manifest/foundation/windows10";
            ns.AddNamespace("m", m.NamespaceName);
            ns.AddNamespace("uap", "http://schemas.microsoft.com/appx/manifest/uap/windows10");

            // TODO. Set these to not hardcoded values.
            manifest.XPathSelectElement("m:Package/m:Identity", ns).Attribute("Name").Value = configuration.PackageIdentityName;
            manifest.XPathSelectElement("m:Package/m:Identity", ns).Attribute("Publisher").Value = configuration.PublisherId;
            manifest.XPathSelectElement("m:Package/m:Properties/m:DisplayName", ns).Value = configuration.DisplayName;
            manifest.XPathSelectElement("m:Package/m:Properties/m:PublisherDisplayName", ns).Value = configuration.PublisherDisplayName;
            manifest.XPathSelectElement("m:Package/m:Applications/m:Application/uap:VisualElements", ns).Attribute("DisplayName").Value = configuration.DisplayName;

            if (manifest.XPathSelectElement("m:Package/m:Capabilities", ns) == null)
            {
                manifest.XPathSelectElement("m:Package", ns).Add(new XElement(m + "Capabilities"));
            }

            if (manifest.XPathSelectElement("m:Package/m:Capabilities/m:Capability[@Name='internetClient']", ns) == null)
            {
                manifest.XPathSelectElement("m:Package/m:Capabilities", ns).Add(new XElement(m + "Capability", new XAttribute("Name", "internetClient")));
            }

            manifest.Save(manifestFile);
        }
    }
}