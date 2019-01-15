using System;

namespace SpiderEye.Json
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    internal class JsonIgnoreAttribute : Attribute
    {
    }
}
