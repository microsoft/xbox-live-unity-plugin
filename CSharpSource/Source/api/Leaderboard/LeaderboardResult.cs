// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Leaderboard
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Runtime.InteropServices;
    using global::System.Threading.Tasks;
    using Microsoft.Xbox.Services.System;

    public class LeaderboardResult
    {
        IntPtr m_leaderboardResultPtr;
        internal LeaderboardResult(IntPtr leaderboardResultPtr)
        {
            m_leaderboardResultPtr = leaderboardResultPtr;
            LeaderboardResult_c cResult = Marshal.PtrToStructure<LeaderboardResult_c>(leaderboardResultPtr);

            TotalRowCount = cResult.TotalRowCount;

            Columns = new List<LeaderboardColumn>();
            if (cResult.ColumnsSize > 0)
            {
                IntPtr[] cColumns = new IntPtr[cResult.ColumnsSize];
                Marshal.Copy(cResult.Columns, cColumns, 0, cResult.ColumnsSize);
                for (int i = 0; i < cResult.ColumnsSize; i++)
                {
                    Columns.Add(new LeaderboardColumn(cColumns[i]));
                }
            }

            Rows = new List<LeaderboardRow>();
            if (cResult.RowsSize > 0)
            {
                IntPtr[] cRows = new IntPtr[cResult.RowsSize];
                Marshal.Copy(cResult.Rows, cRows, 0, cResult.RowsSize);
                for (int i = 0; i < cResult.RowsSize; i++)
                {
                    Rows.Add(new LeaderboardRow(cRows[i]));
                }
            }
        }
        
        public IList<LeaderboardRow> Rows { get; internal set; }

        public IList<LeaderboardColumn> Columns { get; internal set; }

        public uint TotalRowCount { get; internal set; }
        
        private delegate bool LeaderboardResultHasNext(IntPtr leaderboard);
        public bool HasNext
        {
            get
            {
                return XboxLive.Instance.Invoke<bool, LeaderboardResultHasNext>(m_leaderboardResultPtr);
            }
        }

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

        private delegate Int32 LeaderboardResultGetNextQuery(IntPtr leaderboard, IntPtr nextQuery, IntPtr errMessage);
        public LeaderboardQuery GetNextQuery()
        {
            // Allocates memory for returned objects
            IntPtr cErrMessage = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());
            IntPtr cNextQuery = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());

            // Invokes the c method
            int errCode = XboxLive.Instance.Invoke<Int32, LeaderboardResultGetNextQuery>(m_leaderboardResultPtr, cNextQuery, cErrMessage);

            // Handles error
            string errMessage = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(cErrMessage));
            Marshal.FreeHGlobal(cErrMessage);

            if (errCode > 0)
            {
                // todo do something
            }

            // Does local work
            LeaderboardQuery nextQuery = new LeaderboardQuery(Marshal.ReadIntPtr(cNextQuery));
            Marshal.FreeHGlobal(cNextQuery);

            return nextQuery;
        }

        // todo move
        [StructLayout(LayoutKind.Sequential)]
        internal struct LeaderboardResult_c
        {
            [MarshalAs(UnmanagedType.U4)]
            public UInt32 TotalRowCount;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr Columns;

            [MarshalAs(UnmanagedType.U4)]
            public Int32 ColumnsSize;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr Rows;

            [MarshalAs(UnmanagedType.U4)]
            public Int32 RowsSize;
        }

        // Used for mock services
        internal LeaderboardResult(IList<LeaderboardRow> rows, IList<LeaderboardColumn> cols, uint totalRowCount)
        {
            Rows = rows;
            Columns = cols;
            TotalRowCount = totalRowCount;
        }
    }
}