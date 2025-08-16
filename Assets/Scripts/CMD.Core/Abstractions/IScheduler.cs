using System;
namespace CMD.Core
{
    public interface IScheduler
    {
        IDisposable Schedule(float delay, Action tick); // one shot
        IDisposable ScheduleRepeating(float delay, float interval, int repeatCount, Action tick);
    }
}
