using System;

namespace SpiderEye
{
    internal class MethodParameterInfo
    {
        public MethodParameterSource Source { get; }
        public string Name { get; }
        public Type Type { get; }

        public MethodParameterInfo(MethodParameterSource source, string name, Type type)
        {
            Source = source;
            Name = name;
            Type = type;
        }
    }
}
