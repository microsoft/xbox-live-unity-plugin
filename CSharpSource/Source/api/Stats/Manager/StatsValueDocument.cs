// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.Xbox.Services.Shared;

namespace Microsoft.Xbox.Services.Statistics.Manager
{
    public class StatsValueDocument
    {
        internal enum StatValueDocumentState
        {
            NotLoaded,
            Loaded,
            OfflineNotLoaded,
            OfflineLoaded
        }

        private enum StatPendingEventType
        {
            StatChange,
            StatDelete
        }

        private class StatPendingState
        {
            public string Name { get; set; }
            public object Value { get; set; }
            public StatValueType Type { get; set; }
            public StatPendingEventType PendingEventType { get; set; }
        }

        private List<StatPendingState> pendingEventList;

        internal bool IsDirty { get; private set; }
        internal int Revision { get; set; }
        internal StatValueDocumentState State { get; set; }
        internal XboxLiveUser User { get; set; }
        internal Dictionary<string, StatValue> Stats { get; private set; }

        public EventHandler FlushEvent;

        internal StatsValueDocument(Dictionary<string, Models.Stat> statMap, int revision = 0)
        {
            this.IsDirty = false;
            this.pendingEventList = new List<StatPendingState>();
            this.Revision = revision;

            if (statMap != null)
            {
                this.Stats = new Dictionary<string, StatValue>(statMap.Count);

                foreach (var stat in statMap)
                {
                    StatValue statValue;
                    //if (stat.Value.Value is string)
                    //{
                    //    statValue = new StatValue(stat.Key, stat.Value.Value, StatValueType.String);
                    //}
                    //else
                    //{
                    //    statValue = new StatValue(stat.Key, stat.Value.Value, StatValueType.Number);
                    //}
                    //this.Stats.Add(stat.Key, statValue);
                }
            }
            else
            {
                this.Stats = new Dictionary<string, StatValue>();
            }
        }

        internal StatValue GetStat(string statName)
        {
            lock (this.Stats)
            {
                StatValue returnVal;
                this.Stats.TryGetValue(statName, out returnVal);
                return returnVal;
            }
        }

        internal void SetStat(string statName, double statValue)
        {
            lock (this.Stats)
            {
                StatPendingState statPendingState = new StatPendingState()
                {
                    Name = statName,
                    Value = statValue,
                    Type = StatValueType.Number,
                    PendingEventType = StatPendingEventType.StatChange
                };

                this.pendingEventList.Add(statPendingState);
            }
        }

        internal void SetStat(string statName, string statValue)
        {
            lock (this.Stats)
            {
                StatPendingState statPendingState = new StatPendingState()
                {
                    Name = statName,
                    Value = statValue,
                    Type = StatValueType.String,
                    PendingEventType = StatPendingEventType.StatChange
                };

                this.pendingEventList.Add(statPendingState);
            }
        }

        internal void DeleteStat(string statName)
        {
            lock (this.Stats)
            {
                StatPendingState statPendingState = new StatPendingState()
                {
                    Name = statName,
                    PendingEventType = StatPendingEventType.StatDelete
                };

                this.pendingEventList.Add(statPendingState);
            }
        }

        internal void ClearDirtyState()
        {
            lock (this.Stats)
            {
                this.IsDirty = false;
            }
        }

        internal List<string> GetStatNames()
        {
            lock (this.Stats)
            {
                List<string> statNameList = new List<string>(this.Stats.Count);
                foreach (var statPair in this.Stats)
                {
                    statNameList.Add(statPair.Key);
                }

                return statNameList;
            }
        }

        internal void DoWork()
        {
            if (this.State != StatValueDocumentState.NotLoaded)
            {
                lock (this.Stats)
                {
                    foreach (var svdEvent in this.pendingEventList)
                    {
                        switch (svdEvent.PendingEventType)
                        {
                            case StatPendingEventType.StatChange:
                                {
                                    //if (!this.Stats.ContainsKey(svdEvent.Name))
                                    //{
                                    //    var statValue = new StatValue(svdEvent.Name, svdEvent.Value, svdEvent.Type);
                                    //    this.Stats.Add(svdEvent.Name, statValue);
                                    //}
                                    //else
                                    //{
                                    //    this.Stats[svdEvent.Name].SetStat(svdEvent.Value, svdEvent.Type);
                                    //}

                                    this.IsDirty = true;
                                    break;
                                }

                            case StatPendingEventType.StatDelete:
                                {
                                    this.Stats.Remove(svdEvent.Name);
                                    break;
                                }
                        }
                    }

                    this.pendingEventList.Clear();
                }
            }
        }

        internal void MergeStatDocument(StatsValueDocument mergeStatDocument)
        {
            switch (this.State)
            {
                case StatValueDocumentState.NotLoaded:
                    this.Revision = mergeStatDocument.Revision;
                    this.Stats = mergeStatDocument.Stats;
                    break;

                // for offline the stat values local override any service values
                // only add any undefined stats into our list
                case StatValueDocumentState.OfflineNotLoaded:
                case StatValueDocumentState.OfflineLoaded:
                    foreach (var stat in mergeStatDocument.Stats)
                    {
                        if (this.Stats.ContainsKey(stat.Key))
                        {
                            this.Stats.Add(stat.Key, stat.Value);
                        }
                    }
                    break;

                case StatValueDocumentState.Loaded:
                    throw new Exception("MergeStatDocument called with state: StatValueDocumentState.Loaded");

                default:
                    break;
            }

            this.State = StatValueDocumentState.Loaded;
        }
    }
}