using UnityEngine;
using System.Collections;

namespace TestCraft.Core
{
    public interface IWorld
    {
        void DeleteBlock(Vector3 position);
        void SetBlock(Vector3 position, BlockType cType);
        Block[] GetChunkBlocks(Vector3 position);
    }
}
