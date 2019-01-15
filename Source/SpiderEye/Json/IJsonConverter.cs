using System;

namespace SpiderEye.Json
{
    /// <summary>
    /// Provides methods to serialize and deserialize objects to and from JSON.
    /// </summary>
    public interface IJsonConverter
    {
        /// <summary>
        /// Deserializes the given JSON string into an object.
        /// </summary>
        /// <typeparam name="T">The object type to deserialize into.</typeparam>
        /// <param name="json">The JSON data to parse.</param>
        /// <returns>The deserialized object.</returns>
        T Deserialize<T>(string json);

        /// <summary>
        /// Deserializes the given JSON string into an object.
        /// </summary>
        /// <param name="json">The JSON data to parse.</param>
        /// <param name="type">The object type to deserialize into.</param>
        /// <returns>The deserialized object.</returns>
        object Deserialize(string json, Type type);

        /// <summary>
        /// Serializes the given object into a JSON string.
        /// </summary>
        /// <param name="value">The value to serialize.</param>
        /// <returns>The JSON string representing the given object.</returns>
        string Serialize(object value);
    }
}
