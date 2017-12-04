using System;
using System.IO;
using Android.Content;
using Java.Util;

namespace FoosLiveAndroid.Util
{
    public static class PropertiesManager
    {
        private const string ConfigFileName = "app.properties";
        private static Properties _properties;

        /// <summary>
        /// Initialise _properties attribute 
        /// </summary>
        /// <param name="context">Context.</param>
        public static void Initialise(Context context)
        {
            using (var streamReader = new StreamReader(context.Assets.Open(ConfigFileName)))
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

        public static int GetIntProperty(String key)
        {
            Int32.TryParse(_properties?.GetProperty(key), out var tempVal);
            return tempVal;
        } 

        public static float GetFloatProperty(String key)
        {
            float.TryParse(_properties?.GetProperty(key), out var tempVal);
            return tempVal;
        } 

        public static double GetDoubleProperty(String key)
        {
            double.TryParse(_properties?.GetProperty(key), out var tempVal);
            return tempVal;
        } 
    }       
}
