// -----------------------------------------------------------------------
//  <copyright file="XboxLivePostProcessing.cs" company="Microsoft">
//      Copyright (c) Microsoft. All rights reserved.
//      Licensed under the MIT license. See LICENSE file in the project root for full license information.
//  </copyright>
// -----------------------------------------------------------------------

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

    [InitializeOnLoad]
    public class XboxLivePostProcessing
    {
        static XboxLivePostProcessing()
        {
            ProjectFilesGenerator.ProjectFileGeneration += AddXboxServicesConfig;
        }

        /// <summary>
        /// Adds the XboxServices.config file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileContent"></param>
        /// <returns></returns>
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
                Debug.Log("Post Processing " + solutionFolder + " for " + target + " build.");

                string projectFolder = Path.Combine(solutionFolder, Application.productName);

                CopyXboxServiceConfig(projectFolder);
                UpdateProjectFile(projectFolder);
                UpdateAppxManifest(projectFolder);
            }
        }

        private static void CopyXboxServiceConfig(string projectFolder)
        {
            string srcFilePath = Path.Combine(Application.dataPath, "../" + XboxLiveAppConfiguration.FileName);
            string dstFilePath = Path.Combine(projectFolder, XboxLiveAppConfiguration.FileName);
            File.Copy(srcFilePath, dstFilePath, true);
        }

        private static void UpdateProjectFile(string projectFolder)
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
                string publisherCertificateFileName = Application.productName + "_StoreKey.pfx";

                // Replace the existing test cert
                certificateElement.Attribute("Include").Value = publisherCertificateFileName;

                // And update the PackageCertificateKeyFile
                project.XPathSelectElement("msb:Project/msb:PropertyGroup/msb:PackageCertificateKeyFile", ns).Value = publisherCertificateFileName;
            }

            // Add the XboxService.config file
            identityItemGroup.Add(
                new XElement(msb + "Content",
                    new XAttribute("Include", "XboxServices.config"),
                    new XElement(msb + "CopyToOutputDirectory", "PreserveNewest")));

            project.Save(projectFile);
        }

        private static void UpdateAppxManifest(string projectFolder)
        {
            string manifestFile = Path.Combine(projectFolder, "Package.appxmanifest");
            XDocument manifest = XDocument.Load(manifestFile);

            XmlNamespaceManager ns = new XmlNamespaceManager(new NameTable());
            XNamespace m = "http://schemas.microsoft.com/appx/manifest/foundation/windows10";
            ns.AddNamespace("m", m.NamespaceName);
            ns.AddNamespace("uap", "http://schemas.microsoft.com/appx/manifest/uap/windows10");

            // TODO. Set these to not hardcoded values.
            manifest.XPathSelectElement("m:Package/m:Identity", ns).Attribute("Name").Value = "XboxDeveloperExperienceTe.NoXsapiUWP2";
            manifest.XPathSelectElement("m:Package/m:Identity", ns).Attribute("Publisher").Value = "CN=CE00B9BA-76FD-4DEB-8467-08AFAE3E9B9C";
            manifest.XPathSelectElement("m:Package/m:Properties/m:DisplayName", ns).Value = "NoXsapiUWP2";
            manifest.XPathSelectElement("m:Package/m:Properties/m:PublisherDisplayName", ns).Value = "Xbox Developer Experience Team";
            manifest.XPathSelectElement("m:Package/m:Applications/m:Application/uap:VisualElements", ns).Attribute("DisplayName").Value = "NoXsapiUWP2";
            manifest.XPathSelectElement("m:Package", ns).Add(
                new XElement(m + "Capabilities",
                    new XElement(m + "Cabability",
                        new XAttribute("Name", "internetClient"))));

            manifest.Save(manifestFile);
        }
    }
}