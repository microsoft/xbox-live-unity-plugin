// -----------------------------------------------------------------------
//  <copyright file="RandomExtensions.cs" company="Microsoft">
//      Copyright (c) Microsoft. All rights reserved.
//      Licensed under the MIT license. See LICENSE file in the project root for full license information.
//  </copyright>
// -----------------------------------------------------------------------

namespace Assets.Xbox_Live.Scripts.MockData
{
    using System;

    public static class RandomExtensions
    {
        public static string NextGamertag(this Random random)
        {
            return "User_" + random.Next();
        }
    }
}