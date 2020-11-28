using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SpiderEye.Mac.Native;

namespace SpiderEye.Mac.Interop
{
    internal class NativeClassDefinition
    {
        public IntPtr Handle { get; private set; }

        private readonly List<Delegate> callbacks;
        private readonly IntPtr[] protocols;

        private bool registered;
        private IntPtr ivar;

        private NativeClassDefinition(string name, IntPtr parent, IntPtr[] protocols)
        {
            if (name == null) { throw new ArgumentNullException(nameof(name)); }

            this.protocols = protocols ?? throw new ArgumentNullException(nameof(protocols));
            callbacks = new List<Delegate>();
            Handle = ObjC.AllocateClassPair(parent, name, IntPtr.Zero);
        }

        public static NativeClassDefinition FromObject(string name, params IntPtr[] protocols)
        {
            return FromClass(name, ObjC.GetClass("NSObject"), protocols);
        }

        public static NativeClassDefinition FromClass(string name, IntPtr parent, params IntPtr[] protocols)
        {
            return new NativeClassDefinition(name, parent, protocols);
        }

        public void AddMethod<T>(string name, string signature, T callback)
            where T : Delegate
        {
            if (registered) { throw new InvalidOperationException("Native class is already declared and registered"); }

            // keep reference to callback or it will get garbage collected
            callbacks.Add(callback);

            ObjC.AddMethod(
                Handle,
                ObjC.RegisterName(name),
                callback,
                signature);
        }

        public void FinishDeclaration()
        {
            if (registered) { throw new InvalidOperationException("Native class is already declared and registered"); }
            registered = true;

            // variable to hold reference to .NET object that creates an instance
            const string variableName = "_SEInstance";
            ObjC.AddVariable(Handle, variableName, new IntPtr(IntPtr.Size), (byte)Math.Log(IntPtr.Size, 2), "@");
            ivar = ObjC.GetVariable(Handle, variableName);

            foreach (IntPtr protocol in protocols)
            {
                if (protocol == IntPtr.Zero)
                {
                    // must not add null protocol, can cause runtime exception with conformsToProtocol check
                    continue;
                }

                ObjC.AddProtocol(Handle, protocol);
            }

            ObjC.RegisterClassPair(Handle);
        }

        public NativeClassInstance CreateInstance(object parent)
        {
            if (!registered) { throw new InvalidOperationException("Native class is not yet fully declared and registered"); }

            IntPtr instance = ObjC.Call(Handle, "new");

            var parentHandle = GCHandle.Alloc(parent, GCHandleType.Normal);
            ObjC.SetVariableValue(instance, ivar, GCHandle.ToIntPtr(parentHandle));

            return new NativeClassInstance(instance, parentHandle);
        }

        public T GetParent<T>(IntPtr self)
        {
            IntPtr handle = ObjC.GetVariableValue(self, ivar);
            return (T)GCHandle.FromIntPtr(handle).Target!;
        }
    }
}
