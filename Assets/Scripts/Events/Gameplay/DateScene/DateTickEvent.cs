namespace Events.Gameplay
{
    public readonly struct DateTickEvent
    {
        public readonly float TimeLeftSec;
        public DateTickEvent(float timeLeftSec) => TimeLeftSec = timeLeftSec;
    }
}