using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TestCraft.Core
{
    public interface IBlock
    {
        Vector3 Position { get; }
        BlockType BlockType { set; }

        List<string> Attributes { get; set; }

    }

    public class Block : IBlock
    {
        public Block(Vector3 pos)
        {
            Attributes = new List<string>();
            Position = pos;
            BlockType = BlockType.Dirt;
        }

        public Vector3 Position { get; private set; }
        public BlockType BlockType { set; get; }
        public List<string> Attributes { get; set; }
    }

    public static class BlockAttributes
    {
        public const string WithGrass = "with_grass";
    }
}