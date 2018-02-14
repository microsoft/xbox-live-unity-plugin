// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using UnityEngine;

namespace Microsoft.Xbox.Services.Client
{
    /// <summary>
    /// Be aware this will not prevent a non singleton constructor
    ///   such as `T myT = new T();`
    /// To prevent that, add `protected T () {}` to your singleton class.
    /// 
    /// As a note, this is made as MonoBehaviour because we need Coroutines.
    /// </summary>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        private static readonly object syncRoot = new object();
        private static bool applicationIsQuitting;

        public static T Instance
        {
            get
            {
                lock (syncRoot)
                {
                    if (applicationIsQuitting)
                    {
                        if (!typeof(T).Equals(typeof(XboxLiveServicesSettings)) && XboxLiveServicesSettings.instance.DebugLogsOn)
                        {
                            Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                                             "' already destroyed on application quit." +
                                             " Won't create again - returning null.");
                        }
                        return null;
                    }

                    if (instance == null)
                    {
                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            if (!typeof(T).Equals(typeof(XboxLiveServicesSettings)) && XboxLiveServicesSettings.instance.DebugLogsOn)
                            {
                                Debug.LogError("[Singleton] Something went wrong.  There should never be more than 1 singleton!");
                            }
                            return instance;
                        }

                        instance = (T)FindObjectOfType(typeof(T));
                        if (instance == null)
                        {
                            GameObject singleton = new GameObject();
                            instance = singleton.AddComponent<T>();
                            singleton.name = "(singleton) " + typeof(T);

                            DontDestroyOnLoad(singleton);

                            if (!typeof(T).Equals(typeof(XboxLiveServicesSettings)) && XboxLiveServicesSettings.Instance.DebugLogsOn)
                            {
                                Debug.Log("[Singleton] " + typeof(T) + " was created with DontDestroyOnLoad.");
                            }
                        }
                    }

                    return instance;
                }
            }
        }

        /// <summary>
        /// When Unity quits, it destroys objects in a random order.
        /// In principle, a Singleton is only destroyed when application quits.
        /// If any script calls Instance after it have been destroyed, 
        ///   it will create a buggy ghost object that will stay on the Editor scene
        ///   even after stopping playing the Application. Really bad!
        /// So, this was made to be sure we're not creating that buggy ghost object.
        /// </summary>
        public void OnDestroy()
        {
            lock (syncRoot)
            {
                applicationIsQuitting = true;
            }
        }
    }
}