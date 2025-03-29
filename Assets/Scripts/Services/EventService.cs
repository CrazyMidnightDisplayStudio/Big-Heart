using System;
using UnityEngine;
using UnityEngine.Events;

namespace Services
{
    public class EventService : BaseServiceSingleton<EventService>
    {
        public Action<float> OnDatePositiveProgressUpdate;
        public Action<float> OnDateNegativeProgressUpdate;
        public Action<float> OnAddPositiveEffect;
        
        public override void Init()
        {
            base.Init();
            Debug.Log("EventService initialized");
        }
    }
}
