// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Statistics.Manager
{
    using global::System;

    public class StatValue
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public StatValueType Type { get; set; }

        internal StatValue(string name, object value, StatValueType type)
        {
            this.Name = name;
            this.Value = value;
            this.Type = type;
        }

        public int AsInteger()
        {
            return Convert.ToInt32(this.Value.ToString());
        }

        public string AsString()
        {
            return this.Value.ToString();
        }

        public double AsNumber()
        {
            return Convert.ToDouble(this.Value.ToString());
        }

        internal void SetStat(object value, StatValueType type)
        {
            this.Value = value;
            this.Type = type;
        }
    }
}