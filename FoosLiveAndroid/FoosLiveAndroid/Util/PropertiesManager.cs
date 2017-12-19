using System;
using System.IO;
using Android.Content;
using Java.Util;

namespace FoosLiveAndroid.Util
{
    public static class PropertiesManager
    {
        static readonly string Tag = typeof(PropertiesManager).Name;
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
        /// Searches value using a key from config file
        /// </summary>
        /// <returns>The configuration value OR null if value not found or manager not initialised</returns>
        /// <param name="key">Configuration value key</param>
        public static string GetProperty(string key)
        {
            if (_properties == null)
                throw new Exception("PropertiesManager called before initialisation");

            var property = _properties.GetProperty(key);

            if (property == null) 
                throw new Exception($"Property not found in configuration file. Key {key}");
            
            return property;
        }

        /// <summary>
        /// Searches value using a key from config file and parses it to int
        /// </summary>
        /// <returns>Int property</returns>
        /// <param name="key">Configuration value key</param>
        public static int GetIntProperty(string key)
        {
            var property = _properties.GetProperty(key);
            return int.Parse(property);
        } 
        /// <summary>
        /// Searches value using a key from config file and parses it to float
        /// </summary>
        /// <returns>Float property</returns>
        /// <param name="key">Configuration value key</param>
        public static float GetFloatProperty(string key)
        {
            var property = _properties.GetProperty(key);
            return float.Parse(property);
        } 

        /// <summary>
        /// Searches value using a key from config file and parses it to double
        /// </summary>
        /// <returns>Double property</returns>
        /// <param name="key">Configuration value key</param>
        public static double GetDoubleProperty(string key)
        {
            var property = _properties.GetProperty(key);
            return double.Parse(property);
        } 
    }       
}
