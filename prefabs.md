# Xbox Live Prefabs

There are a number of different [Unity Prefabs](https://docs.unity3d.com/Manual/Prefabs.html) provided with the Xbox Live Unity plugin that can help you quickly integrate Xbox Live functionality into your game while at the same time providing a good example about how to call the Xbox Live APIs yourself.

## UserProfile

The `UserProfile` prefab is the most important Xbox Live prefab.  This prefab allows the user to login to Xbox Live and after the user is logged it, it will show their Gamertag, Gamer Picture, and Gamerscore.  Usually you would show this prefab on the initial menu screen, or automatically trigger it when the game launches.  In order to use any of the other Xbox Live prefabs, you must include a UserProfile prefab or manually invoke the sign-in API.  See the `UserProfile.cs` script for details on how 

## Leaderboard

The `Leaderboard` prefab provides a quick easy way to render a Leaderboard in your game.  After dropping the prefab into your project set the following properties to configure it:

* **`LeaderboardName`** - the name of the leaderboard or the stat to render.
* **`SocialGroup`** - *(Optional)* the social group of the leaderboard.  If `None` is provided, a non-social leaderboard (i.e. a global leaderboard) will be rendered.
* **`DisplayName`** - the value to use as the header text for the leaderboard.
* **`EntryCount`** - The maximum number of entries to display on a single page of the leaderboard. **Default:** 10

### LeaderboardEntry

This is a helper prefab that's used to render a single row of a Leaderboard.  You you can modify this prefab to customize the look and feel of leaderboard rows.

## Stats

There are a few different prefabs that work well together to interact with the Stats system.  

### Stat Prefabs

In most cases it makes sense to maintain your own "user stats" object to hold the current values of all of the significant stats for a users but there are a few scenarios where it might be easier to expose one or more stats as objects within the Unity scene to allow easy binding.

We provide `IntegerStat`, `DoubleStat`, and `StringStat` prefabs for this purpose, each of which has it's own corresponding script (e.g. `IntegerStat.cs`).  These have a few common properties that are shared across all three.

* **`Name`** - the name of the stat you want it to map to
* **`DisplayName`** - a friendly name for the stat that will be used if it's displayed anywhere in the game (i.e. with a `StatPanel`).
* **`Value`** - sets the initial value of the stat.  This will *overwrite* any existing value on the service unless you specify `UseInitialValueFromService`.
* **`UseInitialValueFromService`** - indicates that the stat should wait until all existing stat values are retrieved from the service before modifying the value.

In addition to a `SetValue` method, some of the components have extra helper methods to modify stat values.  For example, `IntegerStat.Increment()` can be used to easily increment a stat value and with a `Stat` Unity object, you can bind a call to this method to [UnityEvent](https://docs.unity3d.com/ScriptReference/Events.UnityEvent.html) such as the `OnClick` event exposed by the [Button component](https://docs.unity3d.com/Manual/UIInteractionComponents.html).

### StatPanel Prefab

The `StatPanel` prefab provides an easy way to render the value of a `Stat`.  You can bind this to a `Stat` object in Unity and any changes will be rendered automatically.  