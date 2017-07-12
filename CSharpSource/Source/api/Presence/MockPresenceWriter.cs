using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Presence
{
    public class MockPresenceWriter : IPresenceWriter
    {
        public void AddUser(XboxLiveUser xboxLiveUser)
        {
            // Intentionally Empty
        }

        public void HandleTimeTrigger(object obj)
        {
            // Intentionally Empty
        }

        public void StartWriter()
        {
            // Intentionally Empty
        }

        public void StopWriter()
        {
            // Intentionally Empty
        }
    }
}
