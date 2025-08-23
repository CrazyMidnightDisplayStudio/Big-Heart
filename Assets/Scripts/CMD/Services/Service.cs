using System;
using UnityEngine;

namespace CMD.Services
{
    public abstract class Service : IDisposable
    {
        public string Name { get; }

        protected Service(string name)
        {
            Name = name;
            Debug.Log($"<color=green>{Name} ready</color>");
        }

        protected virtual void Init()
        {
        }

        protected virtual void Start()
        {
        }

        public virtual void Dispose()
        {
        }
    }
}
