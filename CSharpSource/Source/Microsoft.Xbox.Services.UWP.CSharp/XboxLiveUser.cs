// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services
{
    using global::System.Threading.Tasks;
    using Microsoft.Xbox.Services.System;

    public partial class XboxLiveUser
    {
        public XboxLiveUser() : this(null)
        {
        }

        public XboxLiveUser(Windows.System.User systemUser)
        {
            var user = new UserImpl(systemUser);

            // The UserImpl monitors the underlying system for sign out events
            // and notifies us that a user has been signed out.  We can then
            // pass that event on the application with a concrete reference.
            user.SignInCompleted += (sender, args) =>
            {
                OnSignInCompleted(this);
            };
            user.SignOutCompleted += (sender, args) =>
            {
                OnSignOutCompleted(this);
            };

            this.userImpl = user;
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

        internal static void CleanupEventHandler()
        {
            foreach (var eh in signInDelegates)
            {
                InternalSignInCompleted -= eh;
            }
            signInDelegates.Clear();

            foreach (var eh in signOutDelegates)
            {
                InternalSignOutCompleted -= eh;
            }
            signOutDelegates.Clear();
        }
    }
}