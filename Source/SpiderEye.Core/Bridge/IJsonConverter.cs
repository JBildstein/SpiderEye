using System;

namespace SpiderEye.Bridge
{
    /// <summary>
    /// Provides methods for converting objects to and from JSON.
    /// </summary>
    internal interface IJsonConverter
    {
        /// <summary>
        /// Serializes the given object to JSON.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <returns>The given object as JSON.</returns>
        string Serialize(object value);

        /// <summary>
        /// Deserializes the given JSON into an object.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="json">The JSON to deserialize.</param>
        /// <returns>The deserialized object.</returns>
        T Deserialize<T>(string json);

        /// <summary>
        /// Deserializes the given JSON into an object.
        /// </summary>
        /// <param name="json">The JSON to deserialize.</param>
        /// <param name="type">The type of the object.</param>
        /// <returns>The deserialized object.</returns>
        object Deserialize(string json, Type type);
    }
}
