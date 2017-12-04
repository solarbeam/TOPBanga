using System;
using System.IO;
using Android.Content;
using Android.Util;
using Java.Util;

namespace FoosLiveAndroid.Util
{
    public static class PropertiesManager
    {
        private const string configFileName = "app.properties";
        private static Properties _properties;

        public static void Initialise(Context context)
        {
            using (var streamReader = new StreamReader(context.Assets.Open(configFileName)))
            {
                var rawResource = streamReader.BaseStream;
                _properties = new Properties();
                _properties.Load(rawResource);

                rawResource.Dispose();
            }
        }

        /// <summary>
        /// Search value using a key from config file
        /// </summary>
        /// <returns>The configuration value OR null if value not found or manager not initialised</returns>
        /// <param name="key">Configuration value key</param>
        public static string GetProperty(String key)
        {
            return _properties?.GetProperty(key);
        }
    }       
}
