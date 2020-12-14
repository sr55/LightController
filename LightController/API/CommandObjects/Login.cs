// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Login.cs" company="LightController Project (http://github.com/sr55/LightController)">
//   This file is part of the LightController source code - It may be used under the terms of the MIT License.
// </copyright>
// <summary>
//   Defines the Login type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LightController.API.CommandObjects
{
    public class Login
    {
        public Login()
        {
            this.Challenge = "AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGxwdHh8=";
        }

        public string Challenge { get; set; }
    }
}
