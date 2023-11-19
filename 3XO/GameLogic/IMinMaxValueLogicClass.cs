
namespace GameLogic
{
    using TicTacToe.GameLogic;

    public interface IMinMaxValueLogicClass
    {
        Player MinPlayer { get; }
        Player MaxPlayer { get; }

        double GetValue(IBoardBase binTreeNode, int depth = 0);
    }
}