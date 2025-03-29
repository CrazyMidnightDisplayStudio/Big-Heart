using Services;

namespace Rounds
{
    public class MainMenuGameManager : BaseGameManager
    {
        protected override void Start()
        {
            Initialization.Initialize();
        }
    }
}