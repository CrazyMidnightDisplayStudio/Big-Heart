using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public sealed class CoroutineService : BaseServiceSingleton<CoroutineService>
    {
        readonly Dictionary<uint, Coroutine> _running = new();
        uint _nextId = 1; // 0 зарезервирован как «невалидный»

        public override void Init()
        {
            base.Init();
            Debug.Log("CoroutineService initialized");
        }

        /*──────────────────────────────────── public API ───────────────────────────────────*/

        public uint RunCoroutine(Action action, float delay = 0f) =>
            RunCoroutine(WrapAction(action, delay));

        public uint RunCoroutine(IEnumerator routine)
        {
            uint id = NextId();
            Coroutine c = StartCoroutine(Wrapped(routine, id));
            _running[id] = c;
            return id;
        }

        /// <summary>Повторяем <paramref name="action"/> каждые <paramref name="interval"/> сек.,
        /// пока <paramref name="stopCondition"/> не вернёт true.</summary>
        public uint RunRepeatingCoroutine(Action action, float interval, Func<bool> stopCondition)
        {
            uint id = NextId();
            Coroutine c = StartCoroutine(Repeating(action, interval, stopCondition, id)); // ← id
            _running[id] = c;
            return id;
        }

        /*—  остановка  —*/
        public void Stop(uint id)
        {
            if (_running.TryGetValue(id, out var c))
            {
                StopCoroutine(c);
                _running.Remove(id);
            }
        }

        public void Stop(IEnumerable<uint> ids)
        {
            foreach (var id in ids) Stop(id);
        }

        public void StopAllRunningCoroutines()
        {
            foreach (var c in _running.Values) StopCoroutine(c);
            _running.Clear();
        }

        /*─────────────────────────────────── internal ─────────────────────────────────────*/

        uint NextId()
        {
            if (_nextId == 0) {
                _nextId = 1; // wrap-around после uint.MaxValue
                Debug.LogError("CoroutineService: coroutine counter OVERFLOWED!!!");
            }
            return _nextId++;
        }

        IEnumerator Wrapped(IEnumerator routine, uint id)
        {
            yield return StartCoroutine(routine);
            _running.Remove(id); // auto-cleanup
        }

        IEnumerator WrapAction(Action action, float delay)
        {
            if (delay > 0) yield return new WaitForSeconds(delay);
            action?.Invoke();
        }

        IEnumerator Repeating(Action action, float interval, Func<bool> stop, uint id)
        {
            while (!stop())
            {
                action?.Invoke();
                yield return new WaitForSeconds(interval);
            }

            _running.Remove(id); // снимаем id по завершении
        }
    }
}