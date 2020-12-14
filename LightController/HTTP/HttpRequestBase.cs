// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpRequestBase.cs" company="LightController Project (http://github.com/sr55/LightController)">
//   This file is part of the LightController source code - It may be used under the terms of the MIT License.
// </copyright>
// <summary>
//   Defines the HttpRequestBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LightController.HTTP
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class HttpRequestBase
    {
        public static JsonSerializerOptions JsonOptions = new JsonSerializerOptions
                                                          {
                                                              IgnoreNullValues = true,
                                                              WriteIndented = true,
                                                              PropertyNameCaseInsensitive = true
                                                          };

        public async Task<ServerResponse> MakePOSTRequest(string url, string json, Dictionary<string, string> headers)
        {
            if (string.IsNullOrEmpty(json))
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

                requestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");

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

        public async Task<ServerResponse> MakeGETRequest(string url, Dictionary<string, string> headers)
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