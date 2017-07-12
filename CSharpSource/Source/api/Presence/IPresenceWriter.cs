using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Presence
{
    public interface IPresenceWriter
    {
        void StartWriter();

        void StopWriter();

        void HandleTimeTrigger(object obj);

        void AddUser(XboxLiveUser xboxLiveUser);

    }
}
