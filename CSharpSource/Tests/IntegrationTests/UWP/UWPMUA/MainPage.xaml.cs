// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 

namespace UWPIntegration
{
    using System;
    using System.Linq;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    using Microsoft.Xbox.Services;
    using Microsoft.Xbox.Services.System;
    using Microsoft.Xbox.Services.Leaderboard;
    using Microsoft.Xbox.Services.Social.Manager;
    using Microsoft.Xbox.Services.Statistics.Manager;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            bool APIExist = Windows.Foundation.Metadata.ApiInformation.IsMethodPresent("Windows.System.UserPicker", "IsSupported");
            bool isMultiUserApplication = APIExist && Windows.System.UserPicker.IsSupported();

            Log("This is a " + (isMultiUserApplication ? "MUA" : "SUA"));

            XboxLiveUser.SignInCompleted += XboxLiveUser_SignInCompleted; ;
            XboxLiveUser.SignOutCompleted += XboxLiveUser_SignOutCompleted;

            Start();
        }

        void ClearLogs()
        {
            LogStackPanel.Children.Clear();
        }

        public void Log(string logLine)
        {
            TextBlock uiElement = new TextBlock();
            uiElement.FontSize = 14;
            uiElement.Text = logLine;
            LogStackPanel.Children.Add(uiElement);
        }

        private async void Start()
        {
            Log("Start");

            var allUser = await Windows.System.User.FindAllAsync();
            var validSysUser = allUser.Where(user => (user.Type != Windows.System.UserType.LocalGuest || user.Type != Windows.System.UserType.RemoteGuest)).ToList();
            Log($"Found {validSysUser.Count} system user.");

            validSysUser.ForEach(async user =>
            {
                var id = user.NonRoamableId;

                try
                {
                    Log($"Creating XboxLiveUser id: {id}");
                    XboxLiveUser xboxLiveUser = new XboxLiveUser(user);

                    Log($"Signing in silently, id: {id}");
                    var signInResult = await xboxLiveUser.SignInSilentlyAsync();

                    Log($"Sign in silently result: {signInResult.Status}, id: {id}");

                    if(signInResult.Status == SignInStatus.UserInteractionRequired)
                    {
                        Log($"Signing in with UI, id: {id}");
                        signInResult = await xboxLiveUser.SignInAsync();
                        Log($"Sign in result: {signInResult.Status}, id: {id}");
                    }

                }
                catch (Exception e)
                {
                    Log($"sign in failed, id:{id}, Exception: " + e.ToString());
                }
            });
        }

        private async void XboxLiveUser_SignOutCompleted(object sender, SignOutCompletedEventArgs args)
        {
            var UIDispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher;
            await UIDispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Log($"User signed out, id:{args.User.WindowsSystemUser.NonRoamableId}");
            });
            
        }

        private async void  XboxLiveUser_SignInCompleted(object sender, SignInCompletedEventArgs args)
        {
            var UIDispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher;
            await UIDispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Log($"User signed in callback, id:{args.User.WindowsSystemUser.NonRoamableId}");
            });
        }
    }
}