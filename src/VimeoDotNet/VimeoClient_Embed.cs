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
        /// Update text track asynchronously
        /// </summary>
        /// <param name="videoId">VideoId</param>
        /// <param name="embedPresetId">EmbedPresetId</param>
        /// <returns></returns>
        public async Task UpdateEmbedPresetAsync(long videoId, long embedPresetId)
        {
            try
            {
                IApiRequest request = GenerateUpdateEmbedPresetRequest(videoId, embedPresetId);
                IRestResponse response = await request.ExecuteRequestAsync();
                UpdateRateLimit(response);
                CheckStatusCodeError(response, "Error updating embed preset for video.", HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                if (ex is VimeoApiException)
                {
                    throw;
                }
                throw new VimeoApiException("Error updating embed preset for video.", ex);
            }
        }

        private IApiRequest GenerateUpdateEmbedPresetRequest(long clipId, long embedPresetId)
        {
            ThrowIfUnauthorized();

            IApiRequest request = ApiRequestFactory.GetApiRequest(AccessToken);
            request.Method = Method.PUT;
            request.Path = Endpoints.EmbedPreset;
            request.UrlSegments.Add("clipId", clipId.ToString());
            request.UrlSegments.Add("embedPresetId", embedPresetId.ToString());

            return request;
        }
    }
}