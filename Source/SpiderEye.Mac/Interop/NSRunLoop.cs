using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SpiderEye.Mac.Native;

namespace SpiderEye.Mac.Interop
{
    internal sealed class NSRunLoop
    {
        private readonly IntPtr loop;
        private readonly IntPtr mode;
        private readonly IntPtr date;
        private readonly IntPtr method;

        private volatile bool isCompleted;

        public NSRunLoop()
        {
            loop = Foundation.Call("NSRunLoop", "currentRunLoop");
            mode = ObjC.Call(loop, "currentMode");
            date = Foundation.Call("NSDate", "distantFuture");
            method = ObjC.RegisterName("runMode:beforeDate:");
        }

        public static void WaitForTask(Task task)
        {
            var loop = new NSRunLoop();
            while (!task.IsCompleted) { loop.Iterate(); }
        }

        public static T WaitForTask<T>(Task<T> task)
        {
            WaitForTask((Task)task);
            return task.Result;
        }

        public void WaitForCompletion()
        {
            while (!isCompleted) { Iterate(); }
        }

        public void Complete()
        {
            isCompleted = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Iterate()
        {
            ObjC.SendMessage(loop, method, mode, date);
        }
    }
}
