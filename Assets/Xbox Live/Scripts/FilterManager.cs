// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.Xbox.Services.Client
{
    public class FilterManager : MonoBehaviour
    {
        public Theme Theme;

        public void SelectFilter(int filterId)
        {
            var filterList = this.transform.parent.GetComponentsInChildren<FilterSelect>();
            foreach (var filter in filterList)
            {
                filter.Theme = this.Theme;
                if (filter.FilterId == filterId)
                {
                    filter.UpdateStatus(true);
                }
                else
                {
                    filter.UpdateStatus(false);
                }
            }
        }
    }
}