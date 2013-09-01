using UnityEngine;
using System.Collections;

public interface IBlock {
    Vector3 Position { get; }
    BlockTypes BlockType { set; }
}

public class Block : IBlock
{
    public Block(Vector3 pos)
    {
        Position = pos;
        BlockType = BlockTypes.Ground;
    }

    public Vector3 Position { get; private set; }
    public BlockTypes BlockType { set; private get; }
}

public enum BlockTypes
{
    Ground
}