namespace ItemSystem.Effects
{
    public interface IEffect { }                 // маркер

    /* inventory */
    public interface IOnEquipEffect   : IEffect { void OnEquip(); }
    public interface IOnUnEquipEffect : IEffect { void OnUnEquip(); }

    /* date lifecycle */
    public interface IOnDateStart   : IEffect { void OnDateStart(); }
    public interface IOnDatePause   : IEffect { void OnDatePause(); }   // ← NEW
    public interface IOnDateResume  : IEffect { void OnDateResume(); }  // ← NEW
    public interface IOnDateEnd     : IEffect { void OnDateEnd(); }

    /* periodic */
    public interface IPeriodicEffect : IEffect
    {
        float IntervalSec { get; }
        float DurationSec { get; } // 0 → бесконечно
        void Tick();
    }
}
