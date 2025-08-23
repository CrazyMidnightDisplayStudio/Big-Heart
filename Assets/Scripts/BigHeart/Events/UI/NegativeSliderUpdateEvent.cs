namespace Events.UI
{
    public struct NegativeSliderUpdateEvent
    {
        public readonly float Value;
        
        public NegativeSliderUpdateEvent(float value) => Value = value;
    }
}