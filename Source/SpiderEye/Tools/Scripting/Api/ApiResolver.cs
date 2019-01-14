using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SpiderEye.Tools.Json;
using SpiderEye.Tools.Scripting.Models;

namespace SpiderEye.Tools.Scripting.Api
{
    internal class ApiResolver
    {
        private static readonly Dictionary<string, ApiMethodInfo> ApiMethods = new Dictionary<string, ApiMethodInfo>();

        public static ApiResultModel ResolveCall(string id, string parameters)
        {
            if (ApiMethods.TryGetValue(id, out ApiMethodInfo info))
            {
                try
                {
                    object parametersObject = null;
                    if (info.HasParameter && !string.IsNullOrWhiteSpace(parameters))
                    {
                        parametersObject = JsonConverter.Deserialize(parameters, info.ParameterType);
                    }

                    object result = info.Invoke(parametersObject);
                    return new ApiResultModel
                    {
                        Success = true,
                        Value = info.HasReturnValue ? JsonConverter.Serialize(result) : null,
                    };
                }
                catch (Exception ex) { return ApiResultModel.FromError(ex.Message); }
            }
            else { return ApiResultModel.FromError($"Unknown API call \"{id ?? "<null>"}\""); }
        }

        public static void InitApi()
        {
            var assembly = Assembly.GetExecutingAssembly();
            foreach (var type in assembly.GetTypes())
            {
                var attribute = type.GetCustomAttribute<ApiAttribute>(true);
                if (attribute != null)
                {
                    string objectName = GetApiNamespace(type);
                    foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                    {
                        var info = new ApiMethodInfo(method);
                        string fullName = $"{objectName}.{info.Name}";
                        ApiMethods.Add(fullName, info);
                    }
                }
            }
        }

        private static string GetApiNamespace(Type type)
        {
            string baseName = type.Name;
            if (type.IsNested)
            {
                var parents = new List<string>() { baseName };
                while (type.IsNested)
                {
                    type = type.DeclaringType;
                    parents.Add(type.Name);
                }

                return string.Join(".", parents.AsEnumerable().Reverse());
            }
            else { return baseName; }
        }

        private class ApiMethodInfo
        {
            public string Name { get; }
            public Type ParameterType { get; }
            public Type ReturnType { get; }
            public bool HasReturnValue { get; }
            public bool HasParameter { get; }

            private readonly MethodInfo info;

            public ApiMethodInfo(MethodInfo info)
            {
                this.info = info ?? throw new ArgumentNullException(nameof(info));

                Name = JsTools.NormalizeToJsName(info.Name);
                ParameterType = info.GetParameters().FirstOrDefault()?.ParameterType;
                HasParameter = ParameterType != null;
                ReturnType = info.ReturnType;
                HasReturnValue = ReturnType != typeof(void);
            }

            public object Invoke(object parameter)
            {
                if (HasParameter) { return info.Invoke(null, new object[] { parameter }); }
                else { return info.Invoke(null, null); }
            }
        }
    }
}
