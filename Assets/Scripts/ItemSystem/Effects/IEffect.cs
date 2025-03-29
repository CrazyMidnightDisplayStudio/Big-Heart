namespace ItemSystem
{
    public interface IEffect
    {
        public void OnEquip();
        public void OnUnEquip();
        public void OnDateStart();
        public void OnDateEnd();
        
        public void PeriodicEffect();
    }
}