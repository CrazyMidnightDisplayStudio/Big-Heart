using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Services
{
    public sealed class CoroutineService : Service, ICoroutineService
    {
        /*────────────────── internal runner ──────────────────*/
        private sealed class CoroutineRunner : MonoBehaviour
        {
        }

        private readonly Dictionary<uint, Coroutine> _running = new();
        private readonly CoroutineRunner _runner;
        private uint _nextId = 1;

        public CoroutineService() : base("CoroutineService")
        {
            var go = new GameObject("[CoroutineService]");
            UnityEngine.Object.DontDestroyOnLoad(go);
            _runner = go.AddComponent<CoroutineRunner>();
        }

        /*────────────────────── API ───────────────────────────*/

        public uint RunCoroutine(Action action, float delay = 0f) =>
            RunCoroutine(WrapAction(action, delay));

        public uint RunCoroutine(IEnumerator routine)
        {
            uint id = NextId();
            var c = _runner.StartCoroutine(Wrapped(routine, id));
            _running[id] = c;
            return id;
        }

        public uint RunRepeatingCoroutine(Action action, float interval, float duration = 0, [CanBeNull] Func<bool> stop = null)
        {
            if (interval <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(interval), "interval must be > 0");
            }

            uint id = NextId();
            var c = _runner.StartCoroutine(Repeating(action, interval, id, duration, stop));
            _running[id] = c;
            return id;
        }

        public void Stop(uint id)
        {
            if (_running.TryGetValue(id, out var c))
            {
                _runner.StopCoroutine(c);
                _running.Remove(id);
            }
        }

        public void Stop(IEnumerable<uint> ids)
        {
            foreach (var id in ids) Stop(id);
        }

        public void StopAll()
        {
            foreach (var c in _running.Values) _runner.StopCoroutine(c);
            _running.Clear();
        }

        /*────────────────── cleanup (optional) ────────────────*/
        public override void Dispose()
        {
            StopAll();
            if (_runner) UnityEngine.Object.Destroy(_runner.gameObject);
        }

        /*───────────────── helpers / wrappers ─────────────────*/

        uint NextId()
        {
            if (_nextId == 0) // wrap-around после uint.MaxValue
            {
                _nextId = 1;
                Debug.LogError("CoroutineService: counter overflowed!");
            }

            return _nextId++;
        }

        IEnumerator Wrapped(IEnumerator routine, uint id)
        {
            yield return _runner.StartCoroutine(routine);
            _running.Remove(id); // авто-очистка
        }

        IEnumerator WrapAction(Action action, float delay)
        {
            if (delay > 0f) yield return new WaitForSeconds(delay);
            action?.Invoke();
        }

        IEnumerator Repeating(Action action, float interval, uint id, float duration = 0, [CanBeNull] Func<bool> stop = null)
        {
            float elapsed = 0f;
            while (stop == null || !stop() && (duration <= 0f || elapsed < duration))
            {
                action?.Invoke();
                elapsed += interval;
                yield return new WaitForSeconds(interval);
            }

            _running.Remove(id);
        }
    }
}