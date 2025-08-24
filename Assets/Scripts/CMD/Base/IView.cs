namespace CMD.Base
{
    public interface IView<in TEntityRuntime> where TEntityRuntime : BaseEntityRuntime
    {
        void Bind(TEntityRuntime entity);
    }
}
