using UnityEngine;
using System.Collections;

namespace TestCraft.Core
{
    public interface IBlock
    {
        Vector3 Position { get; }
        BlockType BlockType { set; }
    }

    public class Block : IBlock
    {
        public Block(Vector3 pos)
        {
            Position = pos;
            BlockType = BlockType.Dirt;
        }

        public Vector3 Position { get; private set; }
        public BlockType BlockType { set; private get; }
    }
}