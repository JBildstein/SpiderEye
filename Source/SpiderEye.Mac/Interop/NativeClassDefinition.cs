using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SpiderEye.Mac.Native;

namespace SpiderEye.Mac.Interop
{
    internal class NativeClassDefinition
    {
        private readonly IntPtr classPtr;
        private readonly List<Delegate> callbacks;

        private bool registered;
        private IntPtr ivar;

        public NativeClassDefinition(string name, params string[] protocols)
        {
            if (name == null) { throw new ArgumentNullException(nameof(name)); }
            if (protocols == null) { throw new ArgumentNullException(nameof(protocols)); }

            callbacks = new List<Delegate>();
            classPtr = ObjC.AllocateClassPair(ObjC.GetClass("NSObject"), name, IntPtr.Zero);

            foreach (string protocol in protocols)
            {
                ObjC.AddProtocol(classPtr, ObjC.GetProtocol(protocol));
            }
        }

        public void AddMethod<T>(string name, string signature, T callback)
            where T : Delegate
        {
            if (registered) { throw new InvalidOperationException("Native class is already declared and registered"); }

            // keep reference to callback or it will get garbage collected
            callbacks.Add(callback);

            ObjC.AddMethod(
                classPtr,
                ObjC.RegisterName(name),
                callback,
                signature);
        }

        public void FinishDeclaration()
        {
            if (!registered)
            {
                registered = true;

                const string variableName = "_SEInstance";
                ObjC.AddVariable(classPtr, variableName, new IntPtr(IntPtr.Size), (byte)Math.Log(IntPtr.Size, 2), "@");
                ivar = ObjC.GetVariable(classPtr, variableName);

                ObjC.RegisterClassPair(classPtr);
            }
        }

        public NativeClassInstance CreateInstance(object parent)
        {
            if (!registered) { throw new InvalidOperationException("Native class is not yet fully declared and registered"); }

            IntPtr instance = ObjC.Call(classPtr, "new");

            var parentHandle = GCHandle.Alloc(parent, GCHandleType.Normal);
            ObjC.SetVariableValue(instance, ivar, GCHandle.ToIntPtr(parentHandle));

            return new NativeClassInstance(instance, parentHandle);
        }

        public T GetParent<T>(IntPtr self)
        {
            IntPtr handle = ObjC.GetVariableValue(self, ivar);
            return (T)GCHandle.FromIntPtr(handle).Target;
        }
    }
}
