using System;
using UnityEngine;

namespace Services
{
    public abstract class Service : IDisposable
    {
        protected readonly ILogger Log; // UnityEngine.ILogger
        public string Name { get; }

        protected Service(string name, ILogger logger = null)
        {
            Name = name;
            Log = logger ?? Debug.unityLogger;
            Debug.Log($"<color=green>{Name} ready</color>");
        }

        public virtual void Init()
        {
        }

        public virtual void Start()
        {
        }

        public virtual void Dispose()
        {
        }
    }
}
