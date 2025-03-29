using DefaultNamespace;
using Services;
using UnityEngine;

namespace Rounds
{
    public class BaseGameManager : MonoBehaviour
    {
        public GameState GameState { get; protected set; }
        
        protected virtual void Start()
        {
            Debug.Log($"Сцена {GetType().Name} загрузилась");
            GameState = GameState.Running;
        }

        public virtual void Pause()
        {
            GameState = GameState.Paused;
        }

        public virtual void Resume()
        {
            GameState = GameState.Running;
        }
    }
}