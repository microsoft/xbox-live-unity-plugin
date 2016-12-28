## Welcome!

The Xbox Live Unity Plugin provides a way for developers to quickly and easily integrate Xbox Live functionality into their [Unity](https://unity3d.com/) based game. 
To get access to the Xbox Live service, please apply to the [ID@Xbox program](http://www.xbox.com/en-us/Developers/id).

## Overview
The Unity Plugin is broken into the following parts

* __Assets__ contains the Unity project content.
  * The actual Xbox Live plugin assets in the plugin package are located in the __Xbox Live__ folder inside.
* __Build__ contains scripts to generate the .unitypackage and handle other project setup tasks.
* __External__ contains additional resources used by the plugin
  * [__xbox-live-api-csharp__](https://github.com/Microsoft/xbox-live-api-csharp) is a submodule where the Xbox Live SDK comes from.  You can choose to build the SDK from this sub-module or pull existing binaries down from NuGet.
  * [__xbox-live-plugin-shared__](https://github.com/Microsoft/xbox-live-plugin-shared) is a submodule where we get the Xbox Live Association Wizard from.
* __ProjectSettings__ contains standard Unity project settings files.

## Getting Started
If you're just looking to integrate Xbox Live functionality into your Unity game, you can download the [Unity Plugin in the Unity Asset Store](https://https://www.assetstore.unity3d.com/#!/content/TODO).

You can also build the Unity plugin yourself:

1. Open up a powershell window.
2. Clone the project, and be sure to include and sync all the required submodules.

    ```
    git clone https://github.com/Microsoft/xbox-live-unity-plugin --recursive
    cd xbox-live-unity-plugin
    ```
3. Run the Setup script to get all of the pre-requisites configured.

    ```
    .\Build\Setup.ps1
    ```
4. Open up the project in Unity.

    ```
    Unity.exe -projectPath .\
    ```

## Contribute Back!

Is there a feature missing that you'd like to see, or found a bug that you have a fix for? Or do you have an idea or just interest in helping out in building the plugin? Let us know and we'd love to work with you. For a good starting point on where we are headed and feature ideas, take a look at our [requested features and bugs](https://github.com/Microsoft/xbox-live-unity-plugin/issues).  

Big or small we'd like to take your contributions back to help improve the Xbox Live Unity plugin for everyone. 

## Having Trouble?

We'd love to get your review score, whether good or bad, but even more than that, we want to fix your problem. If you submit your issue as a Review, we won't be able to respond to your problem and ask any follow-up questions that may be necessary. The most efficient way to do that is to open a an issue in our [issue tracker](https://github.com/Microsoft/xbox-live-unity-plugin/issues).  

### Quick Links

*   [Issue Tracker](https://github.com/Microsoft/xbox-live-unity-plugin/issues)
*   [ID@Xbox](http://www.xbox.com/en-us/Developers/id)

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

