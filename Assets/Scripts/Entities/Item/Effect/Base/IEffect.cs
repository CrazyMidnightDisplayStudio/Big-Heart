//@formatter:off
namespace Entities.Item.Effect
{
    public interface IEffect { }
    public interface IOnEquipEffect   : IEffect { void OnEquip(); }
    public interface IOnUnEquipEffect : IEffect { void OnUnEquip(); }
    public interface IOnDateStart     : IEffect { void OnDateStart(); }
    public interface IOnDateEnd       : IEffect { void OnDateEnd(); }

    public interface IPeriodicEffect : IEffect
    {
        float IntervalSec { get; }
        float DurationSec { get; } // 0 => бесконечно
        void Tick();
    }
}
