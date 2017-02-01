// -----------------------------------------------------------------------
//  <copyright file="TestScript.cs" company="Microsoft">
//      Copyright (c) Microsoft. All rights reserved.
//      Internal use only.
//  </copyright>
// -----------------------------------------------------------------------

using System.Net;

using UnityEngine;

public class TestScript : MonoBehaviour
{
    public void Test()
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.bing.com");

        request.Headers["Accept"] = "application/json";
        request.Headers["Accept-Language"] = "*";
        request.Headers["Accept"] = "*/*";
    }
}