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

    void Update () {
	
	}
}
