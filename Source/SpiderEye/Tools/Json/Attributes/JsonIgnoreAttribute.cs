using System;

namespace SpiderEye.Tools.Json
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    internal class JsonIgnoreAttribute : Attribute
    {
    }
}
