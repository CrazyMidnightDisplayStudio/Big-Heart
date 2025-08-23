using System;

namespace CMD.Services
{
    public sealed partial class EventBusService
    {
        private abstract class Subscription : IDisposable
        {
            public abstract void Invoke(object e);
            public abstract void Dispose();
        }

        private sealed class Subscription<T> : Subscription where T : struct
        {
            private readonly Action<T> _handler;
            private readonly Func<T, bool>? _filter;
            private bool _disposed;

            public Subscription(Action<T> h, Func<T, bool>? f)
            {
                _handler = h;
                _filter = f;
            }

            public override void Dispose() => _disposed = true;

            public override void Invoke(object e)
            {
                if (_disposed || e is not T t) return;
                if (_filter?.Invoke(t) ?? true)
                {
                    _handler(t);
                }
            }

            // для Dispatch<T>(T) — без лишних проверок boxing
            public void Invoke(T e)
            {
                if (!_disposed && (_filter?.Invoke(e) ?? true))
                {
                    _handler(e);
                }
            }
        }

    }
}
