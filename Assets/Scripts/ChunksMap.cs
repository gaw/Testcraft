using UnityEngine;
using System.Collections;

public class ChunksMap : MonoBehaviour
{
    public Transform Player;

    public int CountX = 1;
    public int CountY = 1;
    public int CountZ = 1;

    public int ChunkSizeX = 32;
    public int ChunkSizeY = 32;
    public int ChunkSizeZ = 32;

    private IWorld _world;

    private void Start()
    {
        _world = new World(ChunkSizeX, ChunkSizeY, ChunkSizeZ);

        for (var i = 0; i < CountX; i++)
            for (var j = -2; j < CountY; j++)
                for (var k = 0; k < CountZ; k++)
                {
                    var position = new Vector3(ChunkSizeX*i, ChunkSizeY*j, ChunkSizeZ*k);

                    var chunk = new GameObject();
                    chunk.name = ChunkController.GetChunkName(new Vector3(i, j, k));
                    chunk.transform.parent = transform;
                    chunk.transform.localPosition = position;

                    var controller = chunk.AddComponent<ChunkController>();
                    controller.Player = Player;
                    controller.World = _world;
                    controller.MapPosition = new Vector3(i, j, k);
                    
                }
    }


    public void AddBlock(Vector3 pos, BlockTypes blockType)
    {
        var chunk = GetChunkByBlock(pos);
        if (chunk == null) return;
        
        var cc = chunk.GetComponent<ChunkController>();
        cc.AddBlock(pos, blockType);
    }


    public void RemoveBlock(Vector3 pos)
    {
        var chunk = GetChunkByBlock(pos);
        if (chunk == null) return;

        var cc = chunk.GetComponent<ChunkController>();
        cc.RemoveBlock(pos);
    }


    private Transform GetChunkByBlock(Vector3 pos)
    {
        var chunkPos = new Vector3(((int) pos.x)/ChunkSizeX, ((int) pos.y)/ChunkSizeY, ((int) pos.z)/ChunkSizeZ);

        var chunkName = ChunkController.GetChunkName(chunkPos);
        var chunk = transform.FindChild(chunkName);
        if (chunk == null)
        {
            Debug.Log("Не найдена область: " + chunkName);
        }

        return chunk;
    }

    void Update () {
	
	}
}
