// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Leaderboard
{
    using global::System.Collections.Generic;

    public partial class LeaderboardResult : ILeaderboardResult
    {        
        public IList<LeaderboardRow> Rows { get; internal set; }

        public IList<LeaderboardColumn> Columns { get; internal set; }

        public uint TotalRowCount { get; internal set; }
             
// todo remove for ID sdk
//#if !XBOX_LIVE_CREATORS_SDK
//        public class GetNextResult
//        {
//            public LeaderboardResult NextResult;
//        }

//        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
//        private delegate void GetNextCompletionRoutine(GetNextResult_c result, IntPtr context);

//        [StructLayout(LayoutKind.Sequential)]
//        internal struct GetNextResultPayload_c
//        {
//            [MarshalAs(UnmanagedType.SysInt)]
//            public IntPtr NextResult;
//        }

//        [StructLayout(LayoutKind.Sequential)]
//        private struct GetNextResult_c
//        {
//            public XboxLiveResult result;
//            public GetNextResultPayload_c payload;
//        }

//        private delegate XsapiResult LeaderboardResultGetNext(IntPtr leaderboardResult, UInt32 maxItems, GetNextCompletionRoutine completionRoutine, IntPtr completionRoutineContext, Int64 taskGroupId);
//        public Task<LeaderboardResult> GetNextAsync(uint maxItems)
//        {
//            var tcs = new TaskCompletionSource<LeaderboardResult>();

//            Task.Run(() =>
//            {
//                int contextKey = XboxLiveCallbackContext<LeaderboardResult, LeaderboardResult>.CreateContext(
//                    this,
//                    tcs,
//                    null,
//                    null);

//                XboxLive.Instance.Invoke<XsapiResult, LeaderboardResultGetNext>(
//                    m_leaderboardResultPtr, 
//                    maxItems, 
//                    (GetNextCompletionRoutine)GetNextComplete, 
//                    (IntPtr)contextKey, 
//                    XboxLive.DefaultTaskGroupId
//                    );
//            });
            
//            return tcs.Task;
//        }

//        private static void GetNextComplete(GetNextResult_c result, IntPtr context)
//        {
//            int contextKey = context.ToInt32();
//            XboxLiveCallbackContext<LeaderboardResult, LeaderboardResult> contextObject;
//            if (XboxLiveCallbackContext<LeaderboardResult, LeaderboardResult>.TryRemove(contextKey, out contextObject))
//            {
//                if (result.result.errorCode == 0)
//                {
//                    LeaderboardResult nextResult = new LeaderboardResult(result.payload.NextResult);

//                    contextObject.TaskCompletionSource.SetResult(nextResult);
//                }
//                else
//                {
//                    contextObject.TaskCompletionSource.SetException(new Exception(result.result.errorMessage));
//                }
//                contextObject.Dispose();
//            }
//        }
//#endif
        
        // Used for mock services
        internal LeaderboardResult(IList<LeaderboardRow> rows, IList<LeaderboardColumn> cols, uint totalRowCount)
        {
            Rows = rows;
            Columns = cols;
            TotalRowCount = totalRowCount;
        }
    }
}