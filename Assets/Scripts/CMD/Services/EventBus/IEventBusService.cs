using System;

namespace CMD.Services
{
    public interface IEventBusService
    {
        IDisposable Subscribe<T>(Action<T> handler, Func<T, bool> filter = null) where T : struct;
        void Publish<T>(T evt);
    }
}
