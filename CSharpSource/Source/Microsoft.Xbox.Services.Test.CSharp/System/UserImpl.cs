// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.System
{
    using global::System;
    using global::System.Text;
    using global::System.Threading.Tasks;

    internal class UserImpl : IUserImpl
    {
        private const string authHeader = "XBL3.0 x=3640350854913039893;eyJlbmMiOiJBMTI4Q0JDK0hTMjU2IiwiYWxnIjoiUlNBLU9BRVAiLCJjdHkiOiJKV1QiLCJ6aXAiOiJERUYiLCJ4NXQiOiIxZlVBejExYmtpWklFaE5KSVZnSDFTdTVzX2cifQ.GMSFQcTP-Uy_hnwIfGQzfZPSsC1cZ4JV_ziiQohD4dGp5AknXEQJcZsqGNLUhibL-nZcyf2LuTEQsGYZTikWN9g06nUzEpOfFjV34UVxSBrJFjyZekOX-GyzE0T8RKQ-bMIwWOYt3kZugjADbp5f4j3-OvCECHAWhRujtK4MmXE.903ZoiRb6_pb3jjRrP52hA.C28j5jAx-Awr71EoR8va-mp2NFk3cjB4GyjLgMw864RcnYJz7AYoeY6rdMVeqKbreEHGb3RdpvnImHTzmLaQueSai_ow4qO743MqIbvDqEQZist_VPnO-kNQfJpXZ5cWBkD7ezuf5pvB9RIRxArViDLfCfuUn-AZJxBzVk0CUKEpP66TDu-30XVGPrGkCwGORg1nuulAxhsoqyh2ysVr-2ii3E5z4PPxksRC9gijv25Wpim6zgFPtnbPaWhUau7vymGcRhRGMC-ySJJ95J-LKKVZ1vkvvJQQ8emRYrWkKzJegzcHbwg3YKW2jljSfwcACY1nq8m4GApEuZ3qr3Av6Fr1E3VUV70vMorrwqMwwmHevDPmsWUYI3rnSLFoVXssrQfRpdNQ9yA_O-_-FjKbBKiSG5wiPgHC7svGeZdhAmM75XsY-WZ3aW6yKTvO_Nimiv1B6Tf9R18C2SkWk-xSVD21l8lPza4EB-t2_ZZsLgD_QnHn9DoaYSJNTc-lqg9Ik4jdrS7KlnSTzS9b5mkEkvrMLEgcQOeR2xiDv9tAil-ENLEHax3VWetUF5oKBG3SAUQsGiei-KMccJHVEPsuHnjcwMvpeK_k6b4W16UFePARa5E_y9Y9wkLbczzO3H6BidAPLXi_OYRZULGYyAYDVL9j9EKlhXtIbQ4klUGpKgrDN7DAz0zy955HoHeygSrjUtJ6lCibt6y3roc5IGeejeGf2I1yH9aX3_t_Pa3Y-ONS54b6_x95XO9uc6HtAqahaebicySafvlI4osrPGSt97FJ11eNH3DZY8uaCSM48JXrdmRz-33GxU9CA0KDTM0v4FbNj9HIeaXM12AikkFdsAZhwJfn9_-gVtqp_yTD1ros1xuwUBEfSQ6x_hii2G9_5M8UTlGgiGp-cJtJzuG5MS_gVInJRYt4BaWU0wdNK7mrLq6V0YzqPddP2lZe2MSi3DJxZJV0ZoZUdeJSRBvX-GfrHtIlpOwOBcMYV3fpLG1DzQ3QvBFtUpqVaXub-LRDPYHY3xMxwZ09bUYVW1pS4DFEe2Qmfll8xO-Tdn_GuC7O-sE6NpPzZXItK_fJVwAH7NBaZ3JwPT4-SGDTk18s22DD2jls5X0A7nEmsnLD7gW96Ewrhuoh11uBJRTkVE7kBIiSLtqCIUVTITRkxsAMxkTrNvFBaTxj6w-bzsW1yPRVfu0c-5Lg-Gv0MkFrja2hyuLjWCkibWEisUE1aDFbS0dSOHJxels5Ng-bvt8o2A_lnTx5OQaBRojMyUnLQ3kigk_sNFg7Evt-7pUYsgK4M2ZHXOG62OnCrYc5H7J4wpBrT6o-zCMeH983n-VB3N_4kceJh3gJWj4I14-QhdIaBuc8tMnRnfjtiAFA01-clDWJF5w7g7QLKluaeApFCfLBfIsWU1EtFR0yKSJ_1k3orfFBfXqvbkqCa1VRXaZwm38tEhZA4dr0cxgxee-DDMn8iNxJ83FagH47sLW8_NIo3svY8PNDawApHGVhahj1KLxAHLFvQS5O-NtFtdE0IK6FgBFhsU5c0yyWCjJFHoYhzqZJkv6zqAs9KJ77uqZwJHv0CabLiPn65vQbBJja_sUHRhxEEBIp51UDY7q17dpbK7khBkhN0h4-KHGCxGl9S7JT2I83bUhiWfJmaz11gzWY6ZArxzo4CTDjXbrCmd_S4LxqN1vkkcvublSq9wmvF25JiKe-o9fev06Ho8OXe3ufuQqKGttDQu6sRfRq1OlPs5UvqRITharu4fvIG7_6SRfSTQrAHHo7XbD-cRp7wGJjpEUaBlZpNLjTZbdUtoyO_srD85t2SmZdi7BIEK8BUrxkwkF9BDopYCnIn3yWg8tbhNEH9tJU_q2I3-XAq0rPkGLA5DVSb5OICZ3GWt2_NwJ31mVnyGsYNtvaK6D4uNuTLPoRHLL2xloBVPaiBtT0zIWICG4cMsWlpVNLmvrK8zd3JAUikE_Vvr0275iZ2XAj.f5y047gzbHv2tb1BXWfrTiPYEGGMBCc5dtkw5-E0cC4";

        public UserImpl() : this("2814662072777140", "2 Dev 183714711")
        {
        }

        public UserImpl(string xuid, string gamertag)
        {
            this.XboxUserId = xuid;
            this.Gamertag = gamertag;
        }

        public bool IsSignedIn { get; set; }
        public XboxLiveUser User { get; set; }

        public string XboxUserId { get; set; }
        public string Gamertag { get; set; }
        public string AgeGroup { get; set; }
        public string Privileges { get; set; }
        public string WebAccountId { get; set; }
        public AuthConfig AuthConfig { get; set; }
        public IntPtr XboxLiveUserPtr { get; }

        public Task<SignInResult> SignInImpl(bool showUI, bool forceRefresh)
        {
            return Task.FromResult(new SignInResult(SignInStatus.Success));
        }

        public Task<TokenAndSignatureResult> InternalGetTokenAndSignatureAsync(string httpMethod, string url, string headers, byte[] body, bool promptForCredentialsIfNeeded, bool forceRefresh)
        {
            string[] authHeaderParts = authHeader.Substring(9).Split(';');
            return Task.FromResult(new TokenAndSignatureResult
            {
                Gamertag = this.Gamertag,
                XboxUserId = this.XboxUserId,
                XboxUserHash = authHeaderParts[0],
                Token = authHeaderParts[1],
                Signature = "==",
            });
        }
    }
}