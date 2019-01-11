using System;
using System.Reflection;

namespace SpiderEye.Tools.Scripting.Api
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    internal class ApiAttribute : Attribute
    {
    }
}
