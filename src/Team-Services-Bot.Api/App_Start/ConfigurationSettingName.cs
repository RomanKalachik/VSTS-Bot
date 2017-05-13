﻿// ———————————————————————————————
// <copyright file="ConfigurationSettingName.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Defines the name of configuration settings.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.App_Start
{
    /// <summary>
    /// Defines the name of configuration settings.
    /// </summary>
    public static class ConfigurationSettingName
    {
        /// <summary>
        /// The Microsoft app id setting name
        /// </summary>
        public const string MicrosoftApplicationId = "MicrosoftAppId";

        /// <summary>
        /// The Microsoft application password setting name.
        /// </summary>
        public const string MicrosoftApplicationPassword = "MicrosoftAppPassword";

        /// <summary>
        /// The emulator listening URL setting name.
        /// </summary>
        public const string EmulatorListeningUrl = "EmulatorListeningUrl";

        /// <summary>
        /// The application id setting name.
        /// </summary>
        public const string ApplicationId = "AppId";

        /// <summary>
        /// The app secret setting name.
        /// </summary>
        public const string ApplicationSecret = "AppSecret";

        /// <summary>
        /// The authorize URL setting name.
        /// </summary>
        public const string AuthorizeUrl = "AuthorizeUrl";
    }
}