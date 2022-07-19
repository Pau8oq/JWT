using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DesktopClient.Helpers
{
    public class AuthenticationDelegatingHandler: DelegatingHandler
    {
        public static bool IsRefreshing = false;

        public AuthenticationDelegatingHandler(HttpClientHandler handler)
        {
            base.InnerHandler = handler;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (UserHelper.Instance.User != null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", UserHelper.Instance.User.AccesssToken);
            }


            var response = await base.SendAsync(request, cancellationToken);

            if (UserHelper.Instance.User != null)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    var res = await UserHelper.Instance.RefreshTokenAsync();

                    if (!res)
                    {
                        UserHelper.Instance.LoginExpired();
                    }
                    else 
                    {
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", UserHelper.Instance.User.AccesssToken);
                        response = await base.SendAsync(request, cancellationToken);
                    }
                }

                return response;
            }
            else
                return response;

        }
    }
}
