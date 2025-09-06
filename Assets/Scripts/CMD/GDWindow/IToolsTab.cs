namespace CMD.GD
{
    public interface IToolsTab
    {
        void EnsureInit(); // ленивая инициализация, без доступа к runtime в конструкторе
        void SetGroup(string g); // просто сохранить имя группы при необходимости (для логов/настроек)
    }
}
