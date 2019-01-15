using System.Collections;

namespace SpiderEye.Json.Collections
{
    /// <summary>
    /// Represents a JSON array.
    /// </summary>
    internal interface IJsonArray : IEnumerable
    {
        /// <summary>
        /// Gets the number items in the array.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Adds a new value to the array.
        /// </summary>
        /// <param name="value">The value to add.</param>
        void Add(object value);
    }
}
