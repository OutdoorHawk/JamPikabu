using Code.Infrastructure.States.GameStateHandler;

namespace Code.Gameplay.Cheats.Cheats.Abstract
{
    public interface ICheatAction
    {
        string CheatLabel { get; }
        OrderType Order { get; }
    }

    public interface ICheatActionBasic : ICheatAction
    {
        void Execute();
    }

    public interface ICheatActionInputString : ICheatAction
    {
        void Execute(string input);
    }
}