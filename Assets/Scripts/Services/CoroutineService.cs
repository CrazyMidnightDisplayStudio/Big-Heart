using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public class CoroutineService : BaseServiceSingleton<CoroutineService>
    {
        private readonly Dictionary<string, Coroutine> _runningCoroutines = new Dictionary<string, Coroutine>();
        
        public override void Init()
        {
            base.Init();
            Debug.Log("CoroutineService initialized");
        }

        public string RunCoroutine(Action action, float delay = 0f)
        {
            return RunCoroutine(WrapAction(action, delay));
        }

        public string RunCoroutine(IEnumerator coroutine)
        {
            string coroutineId = Guid.NewGuid().ToString();
            Coroutine runningCoroutine = StartCoroutine(WrappedCoroutine(coroutine, coroutineId));
            _runningCoroutines[coroutineId] = runningCoroutine;
            return coroutineId;
        }

        /// <summary>
        /// Запускает повторяющийся метод с интервалом, пока условие остановки не станет истинным.
        /// </summary>
        public void RunRepeatingCoroutine(Action action, float interval, Func<bool> stopCondition)
        {
            StartCoroutine(RepeatingCoroutine(action, interval, stopCondition));
        }

        public void StopAllRunningCoroutines()
        {
            foreach (var coroutine in _runningCoroutines.Values)
            {
                StopCoroutine(coroutine);
            }

            _runningCoroutines.Clear();
        }

        private IEnumerator WrappedCoroutine(IEnumerator coroutine, string coroutineId)
        {
            yield return StartCoroutine(coroutine);
            _runningCoroutines.Remove(coroutineId);
        }

        private IEnumerator WrapAction(Action action, float delay)
        {
            if (delay > 0)
                yield return new WaitForSeconds(delay);

            action?.Invoke();
        }

        /// <summary>
        /// Выполняет действие с заданным интервалом, пока `stopCondition` не вернёт `true`.
        /// </summary>
        private IEnumerator RepeatingCoroutine(Action action, float interval, Func<bool> stopCondition)
        {
            while (!stopCondition())
            {
                action?.Invoke();
                yield return new WaitForSeconds(interval);
            }
        }
    }
}
