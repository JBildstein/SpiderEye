using System;
using System.Runtime.InteropServices;
using System.Threading;
using SpiderEye.Linux.Native;

namespace SpiderEye.Linux
{
    internal sealed class GtkSynchronizationContext : SynchronizationContext
    {
        public bool IsMainThread
        {
            get { return Thread.CurrentThread.ManagedThreadId == mainThreadId; }
        }

        private readonly int mainThreadId;

        public GtkSynchronizationContext()
            : this(Thread.CurrentThread.ManagedThreadId)
        {
        }

        private GtkSynchronizationContext(int mainThreadId)
        {
            this.mainThreadId = mainThreadId;
        }

        public override SynchronizationContext CreateCopy()
        {
            return new GtkSynchronizationContext(mainThreadId);
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            if (d == null) { throw new ArgumentNullException(nameof(d)); }

            var data = new InvokeState(d, state, false);
            var handle = GCHandle.Alloc(data, GCHandleType.Normal);
            GLib.ContextInvoke(IntPtr.Zero, InvokeCallback, GCHandle.ToIntPtr(handle));
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            if (d == null) { throw new ArgumentNullException(nameof(d)); }

            if (IsMainThread) { d(state); }
            else
            {
                var data = new InvokeState(d, state, true);
                var handle = GCHandle.Alloc(data, GCHandleType.Normal);

                lock (data)
                {
                    GLib.ContextInvoke(IntPtr.Zero, InvokeCallback, GCHandle.ToIntPtr(handle));
                    Monitor.Wait(data);
                }
            }
        }

        private static bool InvokeCallback(IntPtr data)
        {
            var handle = GCHandle.FromIntPtr(data);
            var state = (InvokeState)handle.Target;

            try { state.Callback(state.State); }
            finally
            {
                if (state.Synchronous) { lock (state) { Monitor.Pulse(state); } }
                handle.Free();
            }

            return false;
        }

        private sealed class InvokeState
        {
            public readonly SendOrPostCallback Callback;
            public readonly object State;
            public readonly bool Synchronous;

            public InvokeState(SendOrPostCallback callback, object state, bool synchronous)
            {
                Callback = callback;
                State = state;
                Synchronous = synchronous;
            }
        }
    }
}
