using Microsoft.Extensions.Configuration;
using System.IO;

namespace Tracer.API.Helper.AppSetting
{
    public class SettingsConfig
    {
        /// <summary>
        /// The application settings
        /// </summary>
        private static SettingsConfig _appSettings;

        /// <summary>
        /// Gets or sets the application setting value.
        /// </summary>
        /// <value>
        /// The application setting value.
        /// </value>
        public string appSettingValue { get; set; }

        /// <summary>
        /// Applications the setting.
        /// </summary>
        /// <param name="Key">The key.</param>
        /// <returns></returns>
        public static string AppSetting(string Key)
        {
            _appSettings = GetCurrentSettings(Key);
            return _appSettings.appSettingValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsConfig"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="Key">The key.</param>
        public SettingsConfig(IConfiguration config, string Key)
        {
            this.appSettingValue = config.GetSection(Key).Value;
        }

        // Get a valued stored in the appsettings.
        // Pass in a key like TestArea:TestKey to get TestValue
        public static SettingsConfig GetCurrentSettings(string Key)
        {
            var builder = new ConfigurationBuilder()
                           .SetBasePath(Directory.GetCurrentDirectory())
                           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                           .AddEnvironmentVariables();

            IConfigurationRoot configuration = builder.Build();

            var settings = new SettingsConfig(configuration.GetSection("ApplicationSettings"), Key);

            return settings;

        }

        public class CommonSettings
        {
            public static string ApplicationRootPath { get; set; }
            public static string LogsFolderPath { get { return Path.Combine(ApplicationRootPath, "wwwroot", "Logs"); } }
        }

    }
}
