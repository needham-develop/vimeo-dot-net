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
        /// Get pictures asynchronously
        /// </summary>
        /// <param name="videoId">VideoId</param>
        /// <returns>Return pictures</returns>
        ///
        public async Task<Paginated<Picture>> GetPicturesAsync(long videoId)
        {
            try
            {
                IApiRequest request = GeneratePicturesRequest(videoId);
                IRestResponse<Paginated<Picture>> response = await request.ExecuteRequestAsync<Paginated<Picture>>();
                UpdateRateLimit(response);
                CheckStatusCodeError(response, "Error retrieving pictures for video.", HttpStatusCode.NotFound);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                return response.Data;
            }
            catch (Exception ex)
            {
                if (ex is VimeoApiException)
                {
                    throw;
                }
                throw new VimeoApiException("Error retrieving pictures for video.", ex);
            }
        }

        /// <summary>
        /// Get picture asynchronously
        /// </summary>
        /// <param name="videoId">VideoId</param>
        /// <param name="pictureId">TrackId</param>
        /// <returns>Return picture</returns>
        public async Task<Picture> GetPictureAsync(long videoId, long pictureId)
        {
            try
            {
                IApiRequest request = GeneratePicturesRequest(videoId, pictureId);
                IRestResponse<Picture> response = await request.ExecuteRequestAsync<Picture>();
                UpdateRateLimit(response);
                CheckStatusCodeError(response, "Error retrieving picture for video.", HttpStatusCode.NotFound);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                return response.Data;
            }
            catch (Exception ex)
            {
                if (ex is VimeoApiException)
                {
                    throw;
                }
                throw new VimeoApiException("Error retrieving picture for video.", ex);
            }
        }

        /// <summary>
        /// Upload new picture file asynchronously
        /// </summary>
        /// <param name="fileContent">File content</param>
        /// <param name="videoId">VideoId</param>
        /// <returns>New picture</returns>
        public async Task<Picture> UploadPictureFileAsync(IBinaryContent fileContent, long videoId)
        {
            if (!fileContent.Data.CanRead)
            {
                throw new ArgumentException("fileContent should be readable");
            }
            if (fileContent.Data.CanSeek && fileContent.Data.Position > 0)
            {
                fileContent.Data.Position = 0;
            }

            Picture ticket = await GetUploadPictureTicketAsync(videoId, null, null);

            IApiRequest request = ApiRequestFactory.GetApiRequest();
            request.Method = Method.PUT;
            request.ExcludeAuthorizationHeader = true;
            request.Path = ticket.link;
            request.Headers.Add(Request.HeaderContentType, fileContent.ContentType);
            request.Headers.Add(Request.HeaderContentLength, fileContent.Data.Length.ToString());
            request.BinaryContent = await fileContent.ReadAllAsync();

            IRestResponse response = await request.ExecuteRequestAsync();
            CheckStatusCodeError(null, response, "Error uploading picture file.", HttpStatusCode.BadRequest);

            return ticket;
        }

        /// <summary>
        /// Update picture asynchronously
        /// </summary>
        /// <param name="videoId">VideoId</param>
        /// <param name="pictureId">TrackId</param>
        /// <param name="active">Picture</param>
        /// <returns>Updated picture</returns>
        public async Task<Picture> UpdatePictureAsync(long videoId, long pictureId, bool? active)
        {
            try
            {
                IApiRequest request = GenerateUpdatePictureRequest(videoId, pictureId, active);
                IRestResponse<Picture> response = await request.ExecuteRequestAsync<Picture>();
                UpdateRateLimit(response);
                CheckStatusCodeError(response, "Error updating picture for video.", HttpStatusCode.NotFound);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                return response.Data;
            }
            catch (Exception ex)
            {
                if (ex is VimeoApiException)
                {
                    throw;
                }
                throw new VimeoApiException("Error updating picture for video.", ex);
            }
        }

        /// <summary>
        /// Delete picture asynchronously
        /// </summary>
        /// <param name="videoId">VideoId</param>
        /// <param name="pictureId">TrackId</param>
        /// <returns></returns>
        public async Task DeletePictureAsync(long videoId, long pictureId)
        {
            try
            {
                IApiRequest request = GenerateDeletePictureRequest(videoId, pictureId);
                IRestResponse<Picture> response = await request.ExecuteRequestAsync<Picture>();
                UpdateRateLimit(response);
                CheckStatusCodeError(response, "Error updating picture for video.", HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                if (ex is VimeoApiException)
                {
                    throw;
                }
                throw new VimeoApiException("Error updating picture for video.", ex);
            }
        }

        private async Task<Picture> GetUploadPictureTicketAsync(long clipId, float? time, bool? active)
        {
            try
            {
                IApiRequest request = GenerateUploadPictureTicketRequest(clipId, time, active);

                IRestResponse<Picture> response = await request.ExecuteRequestAsync<Picture>();
                UpdateRateLimit(response);
                CheckStatusCodeError(null, response, "Error generating upload picture ticket.");

                return response.Data;
            }
            catch (Exception ex)
            {
                if (ex is VimeoApiException)
                {
                    throw;
                }
                throw new VimeoUploadException("Error generating upload picture ticket.", null, ex);
            }
        }

        private IApiRequest GenerateUploadPictureTicketRequest(long clipId, float? time, bool? active)
        {
            ThrowIfUnauthorized();

            IApiRequest request = ApiRequestFactory.GetApiRequest(AccessToken);
            request.Method = Method.POST;
            request.Path = Endpoints.Pictures;
            request.UrlSegments.Add("clipId", clipId.ToString());

            if (time.HasValue)
            {
                request.Query.Add("time", time.Value.ToString());
            }

            if (active.HasValue)
            {
                request.Query.Add("active", active.Value.ToString().ToLower());
            }

            return request;
        }

        private IApiRequest GenerateUpdatePictureRequest(long clipId, long pictureId, bool? active)
        {
            ThrowIfUnauthorized();

            IApiRequest request = ApiRequestFactory.GetApiRequest(AccessToken);
            request.Method = Method.PATCH;
            request.Path = Endpoints.Picture;
            request.UrlSegments.Add("clipId", clipId.ToString());
            request.UrlSegments.Add("pictureId", pictureId.ToString());

            if (active.HasValue)
            {
                request.Query.Add("active", active.Value.ToString().ToLower());
            }

            return request;
        }

        private IApiRequest GeneratePicturesRequest(long clipId, long? pictureId = null)
        {
            ThrowIfUnauthorized();

            IApiRequest request = ApiRequestFactory.GetApiRequest(AccessToken);
            string endpoint = pictureId.HasValue ? Endpoints.Picture : Endpoints.Pictures;
            request.Method = Method.GET;
            request.Path = endpoint;

            request.UrlSegments.Add("clipId", clipId.ToString());
            if (pictureId.HasValue)
            {
                request.UrlSegments.Add("pictureId", pictureId.ToString());
            }
            return request;
        }

        private IApiRequest GenerateDeletePictureRequest(long clipId, long pictureId)
        {
            ThrowIfUnauthorized();

            IApiRequest request = ApiRequestFactory.GetApiRequest(AccessToken);
            string endpoint = Endpoints.Picture;
            request.Method = Method.DELETE;
            request.Path = endpoint;

            request.UrlSegments.Add("clipId", clipId.ToString());
            request.UrlSegments.Add("pictureId", pictureId.ToString());

            return request;
        }
    }
}