// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Authentication.cs" company="LightController Project (http://github.com/sr55/LightController)">
//   This file is part of the LightController source code - It may be used under the terms of the MIT License.
// </copyright>
// <summary>
//   Defines the Authentication type.
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

    public class Authentication
    {
        private readonly string baseUrl;

        private string loginUrl = "/xled/v1/login";
        private string verifyLogin = "/xled/v1/verify";

        public Authentication(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public LoginResponse Login()
        {
            ConsoleOutput.WriteLine("- Logging into Lights", ConsoleColor.Yellow);
            Login login = new Login();
            string jsonString = JsonSerializer.Serialize(login, HttpHelper.JsonOptions);

            var task = Task.Run(async () => await HttpHelper.MakePOSTRequest(this.baseUrl + this.loginUrl, jsonString, null));
            task.Wait();

            if (task.Result != null && task.Result.WasSuccessful)
            {
                LoginResponse response = JsonSerializer.Deserialize<LoginResponse>(task.Result.JsonResponse, HttpHelper.JsonOptions);
                if (response != null && response.Code == 1000)
                {
                    ConsoleOutput.WriteLine("  Logged In!", ConsoleColor.Green);
                }
                else
                {
                    ConsoleOutput.WriteLine(string.Format("  Login Failed ({0})", response.Code), ConsoleColor.Red);
                }

                return response;
            }

            ConsoleOutput.WriteLine("  POST Request Failed", ConsoleColor.Red);

            return null;
        }

        public bool Verify(LoginResponse token)
        {
            ConsoleOutput.WriteLine("- Verifying Login", ConsoleColor.Yellow);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("X-Auth-Token", token.Authentication_token);

            Verify login = new Verify(token.ChallengeResponse);
            string jsonString = JsonSerializer.Serialize(login, HttpHelper.JsonOptions);

            var task = Task.Run(async () => await HttpHelper.MakePOSTRequest(this.baseUrl + this.verifyLogin, jsonString, headers));
            task.Wait();

            if (task.Result != null && task.Result.WasSuccessful)
            {
                VerifyResponse response = JsonSerializer.Deserialize<VerifyResponse>(task.Result.JsonResponse, HttpHelper.JsonOptions);
                if (response != null && response.Code == 1000)
                {
                    ConsoleOutput.WriteLine("- Login Verified", ConsoleColor.Green);
                    return true;
                }
                else
                {
                    ConsoleOutput.WriteLine(string.Format("  Login Verification Failed ({0})", response?.Code ?? -1), ConsoleColor.Red);
                    return false;
                }
            }

            ConsoleOutput.WriteLine("  POST Request Failed", ConsoleColor.Red);
            return false;
        }
    }
}
