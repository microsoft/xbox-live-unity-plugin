using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.Xbox.Services.Client
{
    public enum ExceptionSource {
        GameSaveManager,
        Leaderboard,
        PlayerAuthentication,
        PlayerProfile,
        SignInManager,
        StatManager,
        Stats, 
        Social,
        SocialManager,
        ThemeHelper,
        UnityComponents
    }

    public enum ExceptionType {
        SignInFailed,
        SignInSilentlyFailed,
        MinimumPlayerNumberReached,
        MaximumPlayerNumberReached,
        NoPlayersAreSignedIn,
        PlayerIsAlreadySignedIn,
        PlayerRequestedIsNotSignedIn,
        CreateSocialUserGroupFailed,
        LoadSocialUserGroupFailed,
        AddLocalUserFailed,
        StatIsNotConfigured,
        LoadGamerPicFailed,
        LoadSpriteFailed,
        GameSaveProviderNotInitialized,
        BlobNotFound,
        LoadingDataFailed,
        UnexpectedException
    }

    public class XboxLiveException {

        public ExceptionSource Source { get; set; }

        public ExceptionType Type { get; set; }

        public Exception InnerException { get; set; }
    }

    public class ExceptionManager : Singleton<ExceptionManager>
    {
        private Dictionary<ExceptionSource, List<UnityAction<XboxLiveException>>> ErrorCallbacks { get; set; }

        public void Awake()
        {
            this.ErrorCallbacks = new Dictionary<ExceptionSource, List<UnityAction<XboxLiveException>>>();
        }

        public void Start()
        {
            // Super simple check to determine if configuration is non-empty.  This is not a thorough check to determine if the configuration is valid.
            // A user can easly bypass this check which will just cause them to fail at runtime if they try to use any functionality.
            if (!XboxLiveServicesSettings.EnsureXboxLiveServiceConfiguration() && Application.isPlaying)
            {
                const string message = "Xbox Live is not configured, but the game is attempting to use Xbox Live functionality.  You must update the configuration in 'Xbox Live > Configuration' before building the game to enable Xbox Live.";
                if (Application.isEditor && XboxLiveServicesSettings.Instance.DebugLogsOn)
                {
                    Debug.LogWarning(message);
                }
                else
                {
                    if (XboxLiveServicesSettings.Instance.DebugLogsOn)
                    {
                        Debug.LogError(message);
                    }
                }
            }
        }

        public void OnException(ExceptionSource sourceComponent, UnityAction<XboxLiveException> callback)
        {
            if (!this.ErrorCallbacks.ContainsKey(sourceComponent))
            {
                this.ErrorCallbacks.Add(sourceComponent, new List<UnityAction<XboxLiveException>>());
            }

            this.ErrorCallbacks[sourceComponent].Add(callback);
        }
    
        public void OnAnyException(UnityAction<XboxLiveException> callback)
        {
            foreach (ExceptionSource component in Enum.GetValues(typeof(ExceptionSource)))
            {
                if (!this.ErrorCallbacks.ContainsKey(component))
                {
                    this.ErrorCallbacks.Add(component, new List<UnityAction<XboxLiveException>>());
                }

                this.ErrorCallbacks[component].Add(callback);
            }
        }

        public void RemoveCallbackFromComponent(ExceptionSource sourceComponent, UnityAction<XboxLiveException> callback) {
            if (this.ErrorCallbacks.ContainsKey(sourceComponent))
            {
                if (this.ErrorCallbacks[sourceComponent].Contains(callback))
                {
                    this.ErrorCallbacks[sourceComponent].Remove(callback);
                }
            }
        }

        public void RemoveCallbackFromAllComponents(UnityAction<XboxLiveException> callback)
        {
            foreach (ExceptionSource sourceComponent in Enum.GetValues(typeof(ExceptionSource)))
            {
                if (this.ErrorCallbacks.ContainsKey(sourceComponent))
                {
                    if (this.ErrorCallbacks[sourceComponent].Contains(callback))
                    {
                        this.ErrorCallbacks[sourceComponent].Remove(callback);
                    }
                }
            }
        }

        public void ThrowException(ExceptionSource sourceComponent, ExceptionType exceptionType, Exception ex) {
            if (this.ErrorCallbacks.ContainsKey(sourceComponent) && this.ErrorCallbacks[sourceComponent].Count > 0)
            {
                var errorToThrow = new XboxLiveException()
                {
                    Source = sourceComponent,
                    Type = exceptionType,
                    InnerException = ex
                };

                foreach (var callback in this.ErrorCallbacks[sourceComponent]) {
                    callback(errorToThrow);
                }
            }
        }
    }
}