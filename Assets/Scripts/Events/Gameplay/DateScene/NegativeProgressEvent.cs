namespace Events.Gameplay
{
    public readonly struct NegativeProgressEvent
    {
        public readonly float Amount;
        public NegativeProgressEvent(float amount) => Amount = amount;
    }
}