// -----------------------------------------------------------------------
//  <copyright file="XboxLiveUser.cs" company="Microsoft">
//      Copyright (c) Microsoft. All rights reserved.
//      Licensed under the MIT license. See LICENSE file in the project root for full license information.
//  </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Xbox.Services.System
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Runtime.InteropServices;
    using global::System.Text;
    using global::System.Threading.Tasks;

    public partial class XboxLiveUser
    {
        public Task<SignInResult> SignInAsync()
        {
            if (XboxLiveContext.UseMockData)
            {
                this.IsSignedIn = true;
                this.Gamertag = "Veleek";
                this.XboxUserId = "2533274795524562";
                return Task.FromResult(new SignInResult(SignInStatus.Success));
            }

            throw new NotImplementedException();
        }

        public Task<SignInResult> SignInSilentlyAsync()
        {
            if (XboxLiveContext.UseMockData)
            {
                this.IsSignedIn = true;
                this.Gamertag = "2 dev 7727";
                this.XboxUserId = "123456789";
                return Task.FromResult(new SignInResult(SignInStatus.Success));
            }

            throw new NotImplementedException();
        }

        public Task<SignInResult> SwitchAccountAsync()
        {
            if (XboxLiveContext.UseMockData)
            {
                this.IsSignedIn = true;
                this.Gamertag = "2 dev 7727";
                this.XboxUserId = "123456789";
                return Task.FromResult(new SignInResult(SignInStatus.Success));
            }

            throw new NotImplementedException();
        }

        public Task<GetTokenAndSignatureResult> GetTokenAndSignatureAsync(string httpMethod, string url, string headers)
        {
            return this.GetTokenAndSignatureAsync(httpMethod, url, headers, string.Empty);
        }

        public Task<GetTokenAndSignatureResult> GetTokenAndSignatureAsync(string httpMethod, string url, string headers, byte[] body)
        {
            string bodyString = Encoding.UTF8.GetString(body);

            return this.GetTokenAndSignatureAsync(httpMethod, url, headers, bodyString);
        }

        public Task<GetTokenAndSignatureResult> GetTokenAndSignatureAsync(string httpMethod, string url, string headers, string body)
        {
            if (XboxLiveContext.UseMockData)
            {
                string authHeader = "XBL3.0 x=14920277399322024895;eyJlbmMiOiJBMTI4Q0JDK0hTMjU2IiwiYWxnIjoiUlNBLU9BRVAiLCJjdHkiOiJKV1QiLCJ6aXAiOiJERUYiLCJ4NXQiOiIxZlVBejExYmtpWklFaE5KSVZnSDFTdTVzX2cifQ.OQctaMKerhBGroiCxxhqoW1nbsk_aswjlUxRqJKTjHk_5rdS1XfQLDzykdvwTqZzBOiLLjZqbZ5dHJXh8BEDNUBD9qb2euc8-Byg-YVviJY6csiaVwD_gv5_G1lqYahBaEKdUdTxf9O4NjxZwkI4np_LiGaJWHFP5ChPWpdi_E0.gCc3o2_OFaZ9gZj7Y3xCsw.lkDkd7HIT9hTiqA0ukfeeszw-pUZxqSF1aFUXV9mQNyQAxJkmreBGjUEFH_jNs0A44tfPnBygnItJxL8mkQrPN0GkoRZbTH_tpwps2eXoTVHtvoGPp8eZ-8uJn9CFuxIdwRpsncx_g3firPdbYzUlc2oPejslngZOMQU4u0Zm32togplaEhgGrW2sjI5bZAkWNxgDWhkaw_p467gDliI_zb9lFNAGH-XIaN5Mcq8KrsfDZr9vOsVlX9Tt51MG-FtIkgxLUDUuXnbc9PksDXzkP2PU3jB1fU0ELLqRxGUNPoqwataqktYJVyC9Dg8SSLLyMebIIHtNmjuybkA-6jgZFDUCHnMy2LXKPlUJFZgAfvpCT8iQPBuzXMvc6xaHz6CHg9wHglohsV0Mejbs6lmdYPZtAdvJuMUYTLf33pWVCJljBdT00m0sRzG8nZgqceLVM3w_dTTxoUtFi2hgdBdBatuKuQ_udvYNO4IakRosyF8votm1Hanb0QPf6Yn2IUrujEuMko722MsMojiy4WkvyWoIa21iWXcPJaRR7lo2YhYHkvKRDCzT76znDplkjCaMzNgseQ6Gn1tbLzcj9iGHSgB6kVsHBjy_BKptwJKJnOAEQUQCjOE4gtsGqyiVNij9tuEYR4cak3eYMyLpygrTFNhjJkQqMQfCGjlCXeB22PK8rUXHWzQddyBLkOwx2ryNjRl7mIVxDjm4JtPbN4PRHILuGTceEU9RhkgTWtXBUtsz19-L8xgnUT9sp8w0_mRcShsdlfafOYws5Kt2h1zNyqaLAa-3UTLCvFMb0bfliRVZoK1g-y-kA1NPrSYsMvVRc-FgxXFvK5fPXqmmQNqUXB6uKMjxocypZmD_ceZvUnVR_W6EpYOzFgdY8v_5UHPbwjItzl1FQLhDwAfkJHTQJaxiYZMlPKSA9X_Wwkcu2yg4QvkNStwyPLqgqy6EzzOEG-loh0CH27bF7L81s0LIz_phdeqs5TUqHlwEX1Q4PgOrJfkohzVTuH4kUiT_rOwfofleAw-zwFsjVismzlLS_ipCrtIe-5fm_DmStjbhwtkpx4LEkc3tQAh0uOSeWVCbjxXY0D0llmhGyR4KdraIHfFdFHCbKN5HsdlZA215DlXUzPObSG2vIFIV38e-cBgMwEtFI_JHxVYL9B6wYBJcuTH4P3cR_ps-0xXPZDwj0hjcZwtK1G7p2m9QMNQOVsWkjvaUMmgAF5ZcBW_XREZyi6UnNiPWAIeN8N7RHnDIe9eVSbrpGij-AdcC04nOOCRHWLQdEEieopnG64TptErxGl36oMYwvSkQVHZkVIKsz8KQfBe9mdfSurCskGTwGmXSOae1nk5_YxVjQfyEmpSapdEyOWYoSgUHUyt36ddH2NLVtVkRhyo3ivbLmCptREA5A4RA7t4C2u8rR8JEZu-xqoXpWmIq7CDToBfPGD2kGU-_Ub7o6uNmSH-QBDonLP7Xbkxr7mQzZlppfCuoDThp_Xy-Dbz9tdjyrpCVCA4Ly5Vt-Szzz5l4atzMMaftWvkXYF-sc6YxWfi5PtlNuJfe50QEFpyps88lUUKJJCjsbV9HeQuNVleCFfo-9mgGbHxxMnu_LIQq4a-qJw2Sw7_pTEknAWGz9W-ju_r2m1yz-F_RvH0aZJmDfjp7SgPMuhPpApYCkJy5SD9NrfFH_1-zNWo5uDqaz2unF6YoYuTnsiMgMt-3GFUn4BIfWQaTn99T9zq8vxlXVsOQ5LJvqmWFRV5d0-98cm6KYFs4x7cHJ_9cW71xbQWSvtILuT7H5pQ1SsypG-9DUuLj0YbGVfpr_Wub9aD-XIr3ZPUkIQ289UDl_m6hvSw166X3ACyJ0E8W_CbXtiWOS8D54pn22pEZ1TDPPyF-F1dYKBchQr55cunYknE6K-a6GGk5rQ6kr_5tDjDvRa0yAaFnzJv8mWpXInasNpOH2nTG1w54WzAgEepcihO_hjSBUCaaCXX7cWf48SeSYReM6ZiCPLHJ2wNCg_e4_zsBeAUf9ca36d4wFU.4oAe-Ao6TXFk0GJVYg23VPdEx9PQTAacAdxGCDHArpE";
                string[] authHeaderParts = authHeader.Substring(9).Split(';');
                return Task.FromResult(new GetTokenAndSignatureResult
                {
                    XboxUserId = "2533274795524562",
                    XboxUserHash = authHeaderParts[0],
                    Token = authHeaderParts[1],
                });
            }

            throw new NotImplementedException();
        }
        
    }
}