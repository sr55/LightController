// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginResponse.cs" company="LightController Project (http://github.com/sr55/LightController)">
//   This file is part of the LightController source code - It may be used under the terms of the MIT License.
// </copyright>
// <summary>
//   Defines the LoginResponse type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LightController.API.Model
{
    using System.Text.Json.Serialization;

    public class LoginResponse
    {
        public string Authentication_token { get; set; }

        public int Authentication_token_expires_in { get; set; }

        [JsonPropertyName("challenge-response")]
        public string ChallengeResponse { get; set; }

        public int Code { get; set; }
    }
}
