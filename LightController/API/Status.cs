// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Brightness.cs" company="LightController Project (http://github.com/sr55/LightController)">
//   This file is part of the LightController source code - It may be used under the terms of the MIT License.
// </copyright>
// <summary>
//   Defines the Brightness type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LightController.API
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Threading.Tasks;

    using LightController.API.CommandObjects;
    using LightController.API.Model;
    using LightController.Helpers;

    public class Status
    {
        private readonly string baseUrl;

        private string statusUrl = "/xled/v1/status";
        private string deviceNameUrl = "/xled/v1/device_name";

        public Status(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }
        
        public StatusResponse GetStatus(LoginResponse token)
        {
            ConsoleOutput.WriteLine("- Fetching Status", ConsoleColor.Yellow);
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("X-Auth-Token", token.Authentication_token);

            var task = Task.Run(async () => await HttpHelper.MakeGETRequest(this.baseUrl + this.statusUrl, headers));
            task.Wait();

            if (task.Result != null && task.Result.WasSuccessful)
            {
                StatusResponse response = JsonSerializer.Deserialize<StatusResponse>(task.Result.JsonResponse, HttpHelper.JsonOptions);
                if (response != null)
                {
                    ConsoleOutput.WriteLine(string.Format("  Status Code: ({0})", response.Code), ConsoleColor.Cyan);
                    return response;
                }
            }
            else
            {
                ConsoleOutput.WriteLine(string.Format("  Get Status Failed ({0})", task.Result?.JsonResponse), ConsoleColor.Red);
            }

            return null;
        }

        public DeviceNameResponse GetDeviceName(LoginResponse token)
        {
            ConsoleOutput.WriteLine("- Fetching Device Name", ConsoleColor.Yellow);
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("X-Auth-Token", token.Authentication_token);

            var task = Task.Run(async () => await HttpHelper.MakeGETRequest(this.baseUrl + this.statusUrl, headers));
            task.Wait();

            if (task.Result != null && task.Result.WasSuccessful)
            {
                DeviceNameResponse response = JsonSerializer.Deserialize<DeviceNameResponse>(task.Result.JsonResponse, HttpHelper.JsonOptions);
                if (response != null)
                {
                    ConsoleOutput.WriteLine(string.Format("  Device Name: ({0})", response.Name), ConsoleColor.Cyan);
                    return response;
                }
            }
            else
            {
                ConsoleOutput.WriteLine(string.Format("  Get Device Name Failed ({0})", task.Result?.JsonResponse), ConsoleColor.Red);
            }

            return null;
        }
    }
}
