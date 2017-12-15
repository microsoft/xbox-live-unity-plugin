// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services
{
    using Microsoft.Xbox.Services.System;

    public partial class XboxLiveUser
    {
        public XboxLiveUser() : this(null)
        {
        }

        public XboxLiveUser(Windows.System.User systemUser)
        {
            var userImpl = new UserImpl(systemUser);
            this.userImpl = userImpl;

            // The UserImpl monitors the underlying system for sign out events
            // and notifies us that a user has been signed out.  We can then
            // pass that event on the application with a concrete reference.
            userImpl.SignInCompleted += (sender, args) =>
            {
                if (SignInCompleted != null)
                {
                    SignInCompleted(null, new SignInCompletedEventArgs(this));
                }
            };
            userImpl.SignOutCompleted += (sender, args) =>
            {
                if (SignOutCompleted != null)
                {
                    SignOutCompleted(null, new SignOutCompletedEventArgs(this));
                }
            };
        }

        internal XboxLiveUser(global::System.IntPtr xboxLiveUserPtr)
        {
            var userImpl = new UserImpl(xboxLiveUserPtr);
            this.userImpl = userImpl;

            // The UserImpl monitors the underlying system for sign out events
            // and notifies us that a user has been signed out.  We can then
            // pass that event on the application with a concrete reference.
            userImpl.SignInCompleted += (sender, args) =>
            {
                if (SignInCompleted != null)
                {
                    SignInCompleted(null, new SignInCompletedEventArgs(this));
                }
            };
            userImpl.SignOutCompleted += (sender, args) =>
            {
                if (SignOutCompleted != null)
                {
                    SignOutCompleted(null, new SignOutCompletedEventArgs(this));
                }
            };
            userImpl.UpdatePropertiesFromXboxLiveUserPtr();
        }
      
        public Windows.System.User WindowsSystemUser
        {
            get
            {
                return this.userImpl.CreationContext;
            }
        }

        internal UserImpl Impl
        {
            get { return (this.userImpl as UserImpl); }
        }
    }
}