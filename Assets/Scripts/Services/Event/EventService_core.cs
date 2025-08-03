using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Services
{
    public sealed partial class EventService : Service, IEventService
    {
        private readonly Dictionary<Type, List<Subscription>> _map = new();
        private readonly ConcurrentQueue<object> _threadQueue = new();
        private readonly Stopwatch _sw = new();

        private GameObject _runner;

        private sealed class Runner : MonoBehaviour
        {
            public EventService owner;
            void Update() => owner.Tick();
        }

        public EventService() : base("EventService")
        {
            _runner = new GameObject("[EventServiceRunner]");
            UnityEngine.Object.DontDestroyOnLoad(_runner);
            _runner.hideFlags = HideFlags.HideInHierarchy;
            _runner.AddComponent<Runner>().owner = this;
        }

        #region I_EVENT_SERVICE

        public IDisposable Subscribe<T>(Action<T> h, Func<T, bool>? filter = null)
            where T : struct
        {
            var sub = new Subscription<T>(h, filter);
            (_map.TryGetValue(typeof(T), out var list) ? list : _map[typeof(T)] = new()).Add(sub);
            return sub;
        }

        public void Publish<T>(T evt) where T : struct
        {
            if (Thread.CurrentThread.ManagedThreadId == 1)
            {
                Dispatch(evt);
            }
            else
            {
                _threadQueue.Enqueue(evt);
            }
        }

        #endregion

        private void Tick() // Runner->tick
        {
            while (_threadQueue.TryDequeue(out var raw))
            {
                DispatchObject(raw); // уже на главном потоке
            }
        }

        #region HELPERS

        private void DispatchObject(object raw)
        {
            var type = raw.GetType();
            if (!_map.TryGetValue(type, out var list)) return;

            _sw.Restart();
            foreach (var s in list.ToArray()) // snapshot на случай отписки
            {
                s.Invoke(raw);
            }

            SLA(type.Name);
        }

        private void Dispatch<T>(T evt) where T : struct
        {
            if (!_map.TryGetValue(typeof(T), out var list)) return;

            _sw.Restart();
            foreach (var s in list.ToArray())
            {
                ((Subscription<T>)s).Invoke(evt);
            }

            SLA(typeof(T).Name);
        }

        private void SLA(string eventName)
        {
            if (_sw.ElapsedMilliseconds > 10)
            {
                Log.LogWarning(Name, $"Event {eventName} took {_sw.ElapsedMilliseconds} ms");
            }
        }

        public override void Dispose()
        {
            if (_runner)
            {
                Object.Destroy(_runner);
            }

            _map.Clear();
        }

        #endregion
    }
}