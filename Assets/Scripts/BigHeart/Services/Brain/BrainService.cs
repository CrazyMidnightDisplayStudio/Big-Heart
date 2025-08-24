using CMD.Services;

namespace BigHeart.Services
{
    public sealed class BrainService : Service
    {
        public int CurrentRound { get; private set; } = 1;

        public BrainService() : base("Brain")
        {

        }
        public void NextRound() => CurrentRound++;
    }
}
