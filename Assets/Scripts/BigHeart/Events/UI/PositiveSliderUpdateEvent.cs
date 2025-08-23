namespace Events.UI
{
    public struct PositiveSliderUpdateEvent
    {
        public readonly float Value;
        
        public PositiveSliderUpdateEvent(float value) => Value = value;
    }
}