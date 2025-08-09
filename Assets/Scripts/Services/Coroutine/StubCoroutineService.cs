using System;
using System.Collections;
using System.Collections.Generic;
namespace Services
{
    public class StubCoroutineService : ICoroutineService
    {

        public uint RunCoroutine(Action action, float delay = 0) => 0;
        public uint RunCoroutine(IEnumerator routine) => 0;
        public uint RunRepeatingCoroutine(Action action, float interval, float duration = 0, Func<bool> stop = null) =>
            0;
        public void Stop(uint id)
        {
        }
        public void Stop(IEnumerable<uint> ids)
        {
        }
        public void StopAll()
        {
        }
    }
}
