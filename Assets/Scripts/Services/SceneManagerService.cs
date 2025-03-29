using UnityEngine;
using UnityEngine.SceneManagement;

namespace Services
{
    public class SceneManagerService : BaseServiceSingleton<SceneManagerService>
    {
        public override void Init()
        {
            base.Init();
            Debug.Log("SceneManagerService initialized");
        }
        
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}