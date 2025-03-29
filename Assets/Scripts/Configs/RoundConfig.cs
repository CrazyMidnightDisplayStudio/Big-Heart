using Configs;
using UnityEngine;

namespace Rounds
{
    [CreateAssetMenu(menuName = "Configs/RoundsConfig", fileName = "RoundsConfig")]
    public class RoundConfig : BaseConfig
    {
        public float negativeProgressInSecond;
    }
}