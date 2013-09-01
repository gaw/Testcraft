using UnityEngine;
using System.Collections;

public interface IWorld 
{
    void DeleteBlock(Vector3 position);
    void SetBlock(Vector3 position, BlockTypes cType);
    Block[] GetChunkBlocks(Vector3 position);
}
