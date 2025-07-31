using UnityEngine;

namespace Base
{
    public interface IView
    {
    }

    public interface IViewBinder<in TDefinition> : IView where TDefinition : BaseEntityDefinition
    {
        void Bind(TDefinition definition);
    }
}