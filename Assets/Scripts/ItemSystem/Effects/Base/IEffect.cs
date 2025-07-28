namespace ItemSystem.Effects
{
    public interface IEffect
    {
        public void OnEquip();
        public void OnUnEquip();
        public void OnDateStart();
        public void OnDateEnd();
    }
    
    public interface IPeriodicEffect : IEffect
    {
        float IntervalSec { get; }
        float DurationSec { get; }
        void Tick();
    }
}