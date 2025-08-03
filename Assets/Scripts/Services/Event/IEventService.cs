using System;
using JetBrains.Annotations;

namespace Services
{
    public interface IEventService
    {
        IDisposable Subscribe<T>(Action<T> handler, [CanBeNull] Func<T, bool> filter = null) where T : struct;
        void Publish<T>(T @event) where T : struct;
    }
}