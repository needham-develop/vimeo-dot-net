using RestSharp;
using System;
using System.Net;
using System.Threading.Tasks;
using VimeoDotNet.Constants;
using VimeoDotNet.Exceptions;
using VimeoDotNet.Models;
using VimeoDotNet.Net;

namespace VimeoDotNet
{
    public partial class VimeoClient
    {
        /// <summary>
        /// Get Privacy domain
        /// </summary>
        /// <param name="videoId">VideoId</param>
        /// <returns></returns>
        public async Task<Paginated<PrivacyDomain>> GetPrivacyDomainsAsync(long videoId)
        {
            try
            {
                IApiRequest request = GenerateGetPrivacyDomainsRequest(videoId);
                IRestResponse<Paginated<PrivacyDomain>> response = await request.ExecuteRequestAsync<Paginated<PrivacyDomain>>();
                UpdateRateLimit(response);
                CheckStatusCodeError(response, "Error getting Privacy domain for video.", HttpStatusCode.NotFound);

                return response.Data;
            }
            catch (Exception ex)
            {
                if (ex is VimeoApiException)
                {
                    throw;
                }
                throw new VimeoApiException("Error getting Privacy domain for video.", ex);
            }
        }

        /// <summary>
        /// Add Privacy domain
        /// </summary>
        /// <param name="videoId">VideoId</param>
        /// <param name="domain">domain</param>
        /// <returns></returns>
        public async Task AddPrivacyDomainAsync(long videoId, string domain)
        {
            try
            {
                IApiRequest request = GenerateAddPrivacyDomainRequest(videoId, domain);
                IRestResponse response = await request.ExecuteRequestAsync();
                UpdateRateLimit(response);
                CheckStatusCodeError(response, "Error adding Privacy domain for video.", HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                if (ex is VimeoApiException)
                {
                    throw;
                }
                throw new VimeoApiException("Error adding Privacy domain for video.", ex);
            }
        }

        /// <summary>
        /// Delete Privacy domain
        /// </summary>
        /// <param name="videoId">VideoId</param>
        /// <param name="domain">domain</param>
        /// <returns></returns>
        public async Task DeletePrivacyDomainAsync(long videoId, string domain)
        {
            try
            {
                IApiRequest request = GenerateDeletePrivacyDomainRequest(videoId, domain);
                IRestResponse response = await request.ExecuteRequestAsync();
                UpdateRateLimit(response);
                CheckStatusCodeError(response, "Error deleting Privacy domain for video.", HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                if (ex is VimeoApiException)
                {
                    throw;
                }
                throw new VimeoApiException("Error deleteing Privacy domain for video.", ex);
            }
        }

        private IApiRequest GenerateGetPrivacyDomainsRequest(long clipId)
        {
            ThrowIfUnauthorized();

            IApiRequest request = ApiRequestFactory.GetApiRequest(AccessToken);
            request.Method = Method.GET;
            request.Path = Endpoints.GetPrivacyDomains;
            request.UrlSegments.Add("clipId", clipId.ToString());

            return request;
        }

        private IApiRequest GenerateAddPrivacyDomainRequest(long clipId, string domain)
        {
            ThrowIfUnauthorized();

            IApiRequest request = ApiRequestFactory.GetApiRequest(AccessToken);
            request.Method = Method.PUT;
            request.Path = Endpoints.UpdatePrivacyDomains;
            request.UrlSegments.Add("clipId", clipId.ToString());
            request.UrlSegments.Add("domain", domain);

            return request;
        }

        private IApiRequest GenerateDeletePrivacyDomainRequest(long clipId, string domain)
        {
            ThrowIfUnauthorized();

            IApiRequest request = ApiRequestFactory.GetApiRequest(AccessToken);
            request.Method = Method.DELETE;
            request.Path = Endpoints.UpdatePrivacyDomains;
            request.UrlSegments.Add("clipId", clipId.ToString());
            request.UrlSegments.Add("domain", domain);

            return request;
        }
    }
}