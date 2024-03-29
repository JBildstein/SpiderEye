﻿using System;
using System.Runtime.InteropServices;
using System.Threading;
using SpiderEye.Mac.Native;

namespace SpiderEye.Mac
{
    internal sealed class CocoaSynchronizationContext : SynchronizationContext
    {
        public bool IsMainThread
        {
            get { return Environment.CurrentManagedThreadId == mainThreadId; }
        }

        private readonly int mainThreadId;

        public CocoaSynchronizationContext()
            : this(Environment.CurrentManagedThreadId)
        {
        }

        private CocoaSynchronizationContext(int mainThreadId)
        {
            this.mainThreadId = mainThreadId;
        }

        public override SynchronizationContext CreateCopy()
        {
            return new CocoaSynchronizationContext(mainThreadId);
        }

        public override void Post(SendOrPostCallback d, object? state)
        {
            if (d == null) { throw new ArgumentNullException(nameof(d)); }

            var data = new InvokeState(d, state);
            var handle = GCHandle.Alloc(data, GCHandleType.Normal);
            Dispatch.AsyncFunction(Dispatch.MainQueue, GCHandle.ToIntPtr(handle), InvokeCallback);
        }

        public override void Send(SendOrPostCallback d, object? state)
        {
            if (d == null) { throw new ArgumentNullException(nameof(d)); }

            if (IsMainThread) { d(state); }
            else
            {
                var data = new InvokeState(d, state);
                var handle = GCHandle.Alloc(data, GCHandleType.Normal);
                Dispatch.SyncFunction(Dispatch.MainQueue, GCHandle.ToIntPtr(handle), InvokeCallback);
            }
        }

        private static void InvokeCallback(IntPtr data)
        {
            var handle = GCHandle.FromIntPtr(data);
            var state = (InvokeState)handle.Target!;

            try { state.Callback(state.State); }
            finally { handle.Free(); }
        }

        private sealed class InvokeState
        {
            public readonly SendOrPostCallback Callback;
            public readonly object? State;

            public InvokeState(SendOrPostCallback callback, object? state)
            {
                Callback = callback;
                State = state;
            }
        }
    }
}
