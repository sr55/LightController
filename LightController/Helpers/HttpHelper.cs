// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpRequestBase.cs" company="LightController Project (http://github.com/sr55/LightController)">
//   This file is part of the LightController source code - It may be used under the terms of the MIT License.
// </copyright>
// <summary>
//   Defines the HttpRequestBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LightController.Helpers.Model;

namespace LightController.Helpers
{
    public class HttpHelper
    {
        public static JsonSerializerOptions JsonOptions = new JsonSerializerOptions
                                                          {
                                                              IgnoreNullValues = true,
                                                              WriteIndented = true,
                                                              PropertyNameCaseInsensitive = true
                                                          };

        public static async Task<ServerResponse> MakePOSTRequest(string url, object content, Dictionary<string, string> headers, string contentType = "application/json")
        {
            if (content == null)
            {
                throw new InvalidOperationException("No Post Values Found.");
            }

            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        requestMessage.Headers.Add(header.Key, header.Value);
                    }
                }

                if (contentType == "application/json")
                {
                    requestMessage.Content = new StringContent((string)content, Encoding.UTF8, contentType);
                }
                else
                {
                    requestMessage.Content = new ByteArrayContent((byte[])content);
                }

                using (HttpResponseMessage response = await client.SendAsync(requestMessage))
                {
                    if (response != null)
                    {
                        string returnContent = await response.Content.ReadAsStringAsync();
                        ServerResponse serverResponse = new ServerResponse(response.IsSuccessStatusCode, returnContent);

                        return serverResponse;
                    }
                }
            }

            return null;
        }

        public static async Task<ServerResponse> MakeGETRequest(string url, Dictionary<string, string> headers)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        requestMessage.Headers.Add(header.Key, header.Value);
                    }
                }

                using (HttpResponseMessage response = await client.SendAsync(requestMessage))
                {
                    if (response != null)
                    {
                        string returnContent = await response.Content.ReadAsStringAsync();
                        ServerResponse serverResponse = null;
                        serverResponse = response.StatusCode == HttpStatusCode.Unauthorized ? new ServerResponse(false, returnContent) : new ServerResponse(response.IsSuccessStatusCode, returnContent);

                        return serverResponse;
                    }
                }
            }

            return null;
        }
    }
}