using Microsoft.Xbox.Services;

using UnityEngine.EventSystems;

public interface IStatsManagerEventHandler : IEventSystemHandler
{
    void LocalUserAdded(XboxLiveUser user);

    void LocalUserRemoved(XboxLiveUser user);

    void StatUpdateComplete();
}