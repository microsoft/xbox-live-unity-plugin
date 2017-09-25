
namespace Microsoft.Xbox.Services.System
{
    using global::System.Collections.Generic;
    using global::System.Linq;
    using Windows.Security.Authentication.Web.Core;

    internal class TokenRequestResult
    {
        public WebProviderError ResponseError { get; set; }

        public WebTokenRequestStatus ResponseStatus { get; set; }

        public IDictionary<string, string> Properties { get; set; }

        public WebProviderError ProviderError { get; set; }

        public string Token { get; set; }

        public string WebAccountId { get; set; }

        public TokenRequestResult(WebTokenRequestResult result)
        {
            if (result != null)
            {
                this.ResponseStatus = result.ResponseStatus;
                this.ResponseError = result.ResponseError;

                if (result.ResponseData != null && result.ResponseData.Count > 0)
                { 
                    var responseData = result.ResponseData.FirstOrDefault();
                
                    this.Properties = responseData.Properties;
                    this.ProviderError = responseData.ProviderError;
                    this.Token = responseData.Token;
                    this.WebAccountId = responseData.WebAccount?.Id;
                }
            }
        }
    }
}
