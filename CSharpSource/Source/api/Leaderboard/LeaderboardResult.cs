// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Leaderboard
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Runtime.InteropServices;

    public partial class LeaderboardResult
    {
        public IList<LeaderboardRow> Rows { get; internal set; }

        public IList<LeaderboardColumn> Columns { get; internal set; }

        public uint TotalRowCount { get; internal set; }

        public bool HasNext
        {
            get
            {
                return pImpl.GetHasNext();
            }
        }

        ILeaderboardResultImpl pImpl;
        internal LeaderboardResult(IntPtr leaderboardResultPtr)
        {
            pImpl = new LeaderboardResultImpl(leaderboardResultPtr);

            LEADERBOARD_RESULT cResult = MarshalingHelpers.PtrToStructure<LEADERBOARD_RESULT>(leaderboardResultPtr);

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

        public LeaderboardQuery GetNextQuery()
        {
            return pImpl.GetNextQueryImpl();
        }

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
        
        [StructLayout(LayoutKind.Sequential)]
        internal struct LEADERBOARD_RESULT
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
            pImpl = new MockLeaderboardResultImpl();
        }
    }
}