namespace Gameplay
{
    interface IDateState
    {
        void Enter(); // разовая логика при входе
        void Exit(); // очистка перед сменой фазы
    }
}