namespace BigHeart.Services
{
    public sealed class GameContextService
    {
        public int CurrentRound { get; private set; } = 1;
        public void NextRound() => CurrentRound++;
    }
}
