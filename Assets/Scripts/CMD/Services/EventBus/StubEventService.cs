using System;

namespace CMD.Services
{
    public class StubEventService : IEventBusService
    {
        private sealed class D : IDisposable
        {
            public void Dispose() { }
        }

        public IDisposable Subscribe<T>(Action<T> h, Func<T, bool> filter = null) where T : struct => new D();
        public void Publish<T>(T evt)
        { /* no-op */
        }
    }
}
