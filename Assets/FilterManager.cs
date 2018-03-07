using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.Xbox.Services.Client
{
    public class FilterManager : MonoBehaviour
    {

        public void SelectFilter(int filterId)
        {
            var filterList = this.transform.parent.GetComponentsInChildren<FilterSelect>();
            foreach (var filter in filterList)
            {
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