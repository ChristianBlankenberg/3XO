namespace GameLogicTests
{
    using System;
    using TicTacToe.GameLogic;

    public class BinTreeNode : IBoardBase
    {
        public int Value { get; set; }
        public BinTreeNode Left { get; set; }
        public BinTreeNode Right { get; set; }

        public BinTreeNode(int value)
        {
            this.Value = value;
            this.Left = null;
            this.Right = null;
        }

        public BinTreeNode(BinTreeNode left, BinTreeNode right)
        {
            this.Value = 0;
            this.Left = left;
            this.Right = right;
        }

        public double GetValue() => this.Value;

        public bool IsTerminal()
        {
            return this.Left == null && this.Right == null;
        }

        public int NrOfVariants() => 2;

        private int actVariante = -1;
        public void SetVariant(int nr) => this.actVariante = nr;

        public IBoardBase GetActVariant() => this.actVariante == 0 ? this.Left : this.actVariante == 1 ? this.Right : null;

        public void ReSetVariant() => this.actVariante = -1;
    }

    /*
    public class AlphaBetaPruneClass
    {
        private int alpha;
        private int beta;

        private readonly MinMaxDescriptionAlphaBetaPrune minMaxP1;
        private readonly MinMaxDescriptionAlphaBetaPrune minMaxP2;


        public AlphaBetaPruneClass(MinMaxDescriptionAlphaBetaPrune minMaxP1, MinMaxDescriptionAlphaBetaPrune minMaxP2)
        {
            this.minMaxP1 = minMaxP1;
            this.minMaxP2 = minMaxP2;
        }

        public int GetValue(BinTreeNode binTreeNode)
        {
            this.alpha = int.MinValue;
            this.beta = int.MaxValue;

            var valueAndAlphaBeta = this.GetValueAlphaBetaPrune(binTreeNode, 0, alpha, beta, this.minMaxP1);
            return valueAndAlphaBeta.value;
        }

        private (int value, int alphaBeta) GetValueAlphaBetaPrune(BinTreeNode binTreeNode, int depth, int alpha, int beta, MinMaxDescriptionAlphaBetaPrune minMaxDescriptionAlphaBetaPrune)
        {
            //int alpha = alphaParam;
            //int beta = betaParam;
            int alphaBetaReturn = 0;

            if (binTreeNode.IsTerminal())
            {
                return (binTreeNode.Value, binTreeNode.Value);
            }
            else
            {
                int value = minMaxDescriptionAlphaBetaPrune.StartValue;
                var allFieldIdxs = binTreeNode.AllFieldIdxs();

                foreach (var fieldIdx in allFieldIdxs)
                {
                    BinTreeNode nextTreeNode = binTreeNode.Get(fieldIdx);

                    if (minMaxDescriptionAlphaBetaPrune == this.minMaxP1)
                    {
                        // Max
                        var valueAndAlphaBeta = this.GetValueAlphaBetaPrune(nextTreeNode, depth + 1, alpha, beta, this.minMaxP2);
                        value = minMaxDescriptionAlphaBetaPrune.MinMaxFunc(value, valueAndAlphaBeta.value);                                                
                        alpha = Math.Max(alpha, value);
                        alphaBetaReturn = alpha;
                    }
                    else if (minMaxDescriptionAlphaBetaPrune == this.minMaxP2)
                    {
                        // Min
                        var valueAndAlphaBeta = this.GetValueAlphaBetaPrune(nextTreeNode, depth + 1, alpha, beta, this.minMaxP1);
                        value = minMaxDescriptionAlphaBetaPrune.MinMaxFunc(value, valueAndAlphaBeta.value);
                        beta = Math.Min(beta, value);
                        alphaBetaReturn = beta;
                    }

                    if (alpha >= beta)
                    {
                        break;
                    }
                }
                
                return (value, alphaBetaReturn);
            }
        }
    }
    */
}
