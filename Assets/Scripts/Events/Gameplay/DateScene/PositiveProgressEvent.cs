namespace Events.Gameplay
{
    public struct PositiveProgressEvent
    {
        public readonly float Amount;
        public PositiveProgressEvent(float amount) => Amount = amount;
    }
}