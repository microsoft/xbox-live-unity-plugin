## Welcome!

The Xbox Live Unity Plugin provides a way for developers in the Xbox Live Creators Program to quickly and easily integrate Xbox Live functionality into their [Unity](https://unity3d.com/) based game.  For more information about the Xbox Live Creators Program go to [http://aka.ms/xblcp](http://aka.ms/xblcp).

For ID@Xbox developers, this Xbox Live Unity Plugin does not yet support all the features you will need.  Instead, please follow the documentation at [http://aka.ms/xbldocs](http://aka.ms/xbldocs)

## Overview
The Unity Plugin is broken into the following parts

* __Assets__ contains the Unity project content.
  * __Xbox Live__ contains the actual plugin assets that are included in the published `.unitypackage`.
    * __Editor__ contains scripts that provide the basic Unity configuration UI and processes the projects during build.
    * __Examples__ contains a set of simple scene files that show how to use the various prefabs and connect them together.
    * __Images__ is a small set of images that are used by the prefabs.
    * __Libs__ is where the Xbox Live libraries will be stored.  This will only contain `.meta` files when you initially clone the repository.  You must [build the SDK](#Getting_Started) to pull those files in.
    * __Prefabs__ contains various Unity prefab objects that implement Xbox Live functionality.  See [the prefabs documentation](prefabs.md) for more information.
    * __Scripts__ contains all of the code files that actually call the Xbox Live APIs from the prefabs.  This is a great place to look for examples about how to properly call the Xbox Live APIs. See [the scripts documentation](scripts.md) for more detail.

      Inside this folder, you'll also find a folder called __GameSave__ where you'll see the Game Save (Connected Storage) plugin and its scripts.
    * __Tools/AssociationWizard__ contains the Xbox Live Association Wizard, used to pull down application configuration from DevCenter for use within Unity.

* __Build__ contains scripts to generate the .unitypackage and handle other project setup tasks.
* __CSharpSource__ contains source for the Xbox Live API that is used by the plugin

* __ProjectSettings__ contains standard Unity project settings files.

## Getting Started
If you're just looking to integrate Xbox Live functionality into your Unity game, you can [download the latest UnityPackage from Releases](http://github.com/Microsoft/xbox-live-unity-plugin/releases/latest).  If you want to make changes or you need to debug something, you can do so directly from this source.

### Prerequisites

* [Windows 10](https://microsoft.com/windows) Anniversary Update
* [Unity 5.5](https://unity3d.com)
  * Currently 5.5 is the minimum supported version, but we are looking into supporting some earlier versions in the future.
  * You need to include the following components when installing.
    * [Microsoft Visual Studio Tools for Unity](https://marketplace.visualstudio.com/items?itemName=SebastienLebreton.VisualStudio2015ToolsforUnity)
    * Windows Store .NET Scripting Backend
* [Visual Studio 2015](https://www.visualstudio.com/)
  * Any version of Visual Studio should work for this including Community Edition.
  * Make sure to select everything under **Universal Windows App Development Tools** when installing.  You can modify the installation to include these features for an existing installation as well.
* [Powershell](https://microsoft.com/powershell)
  * If you've never used powershell before make sure that you have [enabled powershell scripts to run](https://technet.microsoft.com/en-us/library/ee176961.aspx).
* [Xbox Live Platform Extensions SDK](http://aka.ms/xblextsdk) 

### Building

1. Open up a powershell window.
2. Clone the project, and be sure to include and sync all the required submodules.

    ```powershell
    git clone https://github.com/Microsoft/xbox-live-unity-plugin --recursive
    cd xbox-live-unity-plugin
    ```
3. Run the Setup powershell script to get all of the pre-requisites built and configured.

    ```powershell
    .\Build\Setup.ps1
    ```

Note: Ensure Unity was installed with Windows Store .NET Scripting Backend, and you have the Microsoft Visual Studio Tools for Unity installed.

4. If you want to make any modifications to the scripts or prefabs in the package, open up the project (the `xbox-live-unity-plugin` folder) in Unity and make your changes.  

5. Generate the `XboxLive.unitypackage` that you can import into any other project:

    ```
    .\Build\BuildPackage.ps1
    ```

## Using the Xbox Live Unity Asset 

See the docs at https://docs.microsoft.com/en-us/windows/uwp/xbox-live/get-started-with-creators/develop-creators-title-with-unity



## Using the Game Save Plugin

When the ```.\Build\BuildPackage.ps1``` script is run, the `XboxLive.unitypackage` is generated. When that unity package is imported into a unity project, the `GameSave` folder within the `Xbox Live\Assets\Scripts\` folder will contain the `GameSave.unitypackage` which contains scripts for integrating with Connected Storage.

For an example of how to use Connected Storage, you'll need to first add Sign In to your game. Afterwards, drag the `GameSaveUI.cs` script and drop it on any object in your scene. That will kick-off a simple UI to test your integration to Connected Storage.

Please make sure to check out the best practice and documentation of how to use Connected Storage at: [Connected Storage Documenation](https://developer.microsoft.com/en-us/games/xbox/docs/xboxlive/storage-platform/connected-storage/connected-storage-technical-overview).

## Contribute Back!

Is there a feature missing that you'd like to see, or found a bug that you have a fix for? Or do you have an idea or just interest in helping out in building the plugin? Let us know and we'd love to work with you. For a good starting point on where we are headed and feature ideas, take a look at our [requested features and bugs](https://github.com/Microsoft/xbox-live-unity-plugin/issues). See the [contribution guidelines](CONTRIBUTING.MD) for details.

Big or small we'd like to take your contributions back to help improve the Xbox Live Unity plugin for everyone.

## Having Trouble?

We'd love to get your review score, whether good or bad, but even more than that, we want to fix your problem. If you submit your issue as a Review, we won't be able to respond to your problem and ask any follow-up questions that may be necessary. The most efficient way to do that is to open a an issue in our [issue tracker](https://github.com/Microsoft/xbox-live-unity-plugin/issues).  

Any questions you might have can be answered on the [MSDN Forums](https://social.msdn.microsoft.com/Forums/en-US/home?forum=xboxlivedev).  You can also ask programming related questions to [Stack Overflow](http://stackoverflow.com/questions/tagged/xbox-live) using the "xbox-live" tag.  The Xbox Live team will be engaged with the community and be continually improving our APIs, tools, and documentation based on the feedback received there.  

For developers in the Xbox Live Creators Program, you can submit a new idea or vote on existing idea at our [Xbox Live Creators Program User Voice](https://aka.ms/xblcpuv)

### Xbox Live GitHub projects
*   [Xbox Live Service API for C++](https://github.com/Microsoft/xbox-live-api)
*   [Xbox Live Samples](https://github.com/Microsoft/xbox-live-samples)
*   [Xbox Live Unity Plugin](https://github.com/Microsoft/xbox-live-unity-plugin)
*   [Xbox Live Resiliency Fiddler Plugin](https://github.com/Microsoft/xbox-live-resiliency-fiddler-plugin)
*   [Xbox Live Trace Analyzer](https://github.com/Microsoft/xbox-live-trace-analyzer)
*   [Xbox Live Powershell Cmdlets](https://github.com/Microsoft/xbox-live-powershell-module)

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
