# Xbox Live Unity Scripts

The Xbox Live Unity plugin provides a bunch of [Unity Scripts](https://docs.unity3d.com/Manual/CreatingAndUsingScripts.html) to wrap Xbox Live APIs and make them easier to use from within Unity.  Most of the scripts are directly attached to [various prefabs](./prefabs.md) but nearly all of it could easily be pulled out and used directly.  A few more important scripts whos utility may not be immediately obvious are described below.  

## `XboxLive` script

Most Xbox Live functionality in Unity is managed by the `XboxLive` script.  This object is automatically instantiated in your scene when you use any Xbox Live functionality and marked as `DontDestryOnLoad` so that it lives for the entire duration of the game.

Any of your scripts that need to make calls to Xbox Live APIs should use the various properties `XboxLive.Instance`.

* **`Context`** provides the main entry point into many Xbox Live services and will be initialized after a user has been authenticated using `SignInAsync()`.  See the [Xbox Live API documentation](https://docs.microsoft.com/gaming/xbox-live/api-ref/live-api-reference-nav) for details.
* **`User`** provides a reference to the currently authenticated user which can be used when making calls to various services.

## `UnityTaskExtensions` script

Although Unity doesn't yet support .NET `Task` (along with async/await), the SDK is shared among different platforms including UWP which does.  In order to properly support tasks, we include `Unity.Compat.dll` and `Unity.Tasks.dll` libraries which include barebones `Task` implementations for Unity (from the [Parse SDK for .NET](https://github.com/ParsePlatform/Parse-SDK-dotNET).  The `UnityTaskExtensions` script provides some additional helper functions to make it easier to use tasks inside along side [Unity Coroutines](https://docs.unity3d.com/Manual/Coroutines.html).  

The `.AsCoroutine()` extension method converts a `Task` into an `IEnumerable` which can be yielded from a coroutine.
