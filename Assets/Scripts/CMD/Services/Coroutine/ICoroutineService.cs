using System;
using System.Collections;
using System.Collections.Generic;

namespace CMD.Services
{
public interface ICoroutineService
{
    uint RunCoroutine(Action action, float delay = 0f);
    uint RunCoroutine(IEnumerator routine);
    uint RunRepeatingCoroutine(Action action, float interval, float duration = 0, Func<bool> stop = null);
    void Stop(uint id);
    void Stop(IEnumerable<uint> ids);
    void StopAll();
}
}
