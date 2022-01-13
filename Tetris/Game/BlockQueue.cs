using System;
using Tetris.Game.Blocks;

namespace Tetris.Game
{
    public class BlockQueue
    {
        private readonly Block[] _blocks = new Block[]
        {
            new IBlock(),
            new JBlock(),
            new LBlock(),
            new OBlock(),
            new SBlock(),
            new TBlock(),
            new ZBlock()
        };

        private readonly Random random = new Random();

        public Block NextBlock { get; private set; }

        public BlockQueue()
        {
            NextBlock = Randomize();
        }

        public Block GetBlock()
        {
            Block block = NextBlock;

            do { NextBlock = Randomize(); }
            while (NextBlock.Id == block.Id);

            return block;
        }

        private Block Randomize()
        {
            return _blocks[random.Next(_blocks.Length)];
        }
    }
}
