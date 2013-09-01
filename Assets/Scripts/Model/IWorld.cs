using UnityEngine;
using System.Collections;

public interface IWorld 
{
    Vector3[] GetBlocks(Vector3 position);
    IBlock[] GetNearBlocks(Vector3 position);

    void DeleteBlock(Vector3 position);
    void SetBlock(Vector3 position, BlockTypes cType);
    IBlock GetBlock(Vector3 position);

    Block[] GetChunkBlocks(Vector3 position);
}
