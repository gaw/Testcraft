using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using TestCraft.Core;

public class ChunksMap : MonoBehaviour
{
    public Transform Player;

    public int CountX = 1;
    public int CountY = 1;
    public int CountZ = 1;

    public int ChunkSizeX = 32;
    public int ChunkSizeY = 32;
    public int ChunkSizeZ = 32;

    private World _world;
    private ResourceManager rs_mgr;

    private void Start()
    {
        _world = new World(ChunkSizeX*CountX, ChunkSizeZ*CountZ, 10, 10, 10, 1);

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

        var rs = GameObject.FindGameObjectWithTag("RS_MGR");
        if (rs != null)
        {
            rs_mgr = rs.GetComponent<ResourceManager>();
        }
        else
        {
            print("RESOURCE MANAGER NOT FOUND");
        }
    }


    public void AddBlock(Vector3 pos, BlockType blockType, params string[] attributes)
    {
        var cc = GetChunkControllerByBlock(pos);

        var nbArray = cc.GetNearBlocks(pos);

        if (blockType == BlockType.Dirt)
        {
            // Находим верхний блок, если такой есть
            var hiBlock = nbArray.FirstOrDefault(b => b.BlockType == BlockType.Dirt && b.Position == new Vector3(pos.x, pos.y + 1, pos.z));
            if (hiBlock != null)
            {
                cc.AddBlock(pos, blockType);
            }
            else
            {
                cc.AddBlock(pos, blockType, BlockAttributes.WithGrass);
                var lowBlock = nbArray.FirstOrDefault(b => b.BlockType == BlockType.Dirt && b.Position == new Vector3(pos.x, pos.y - 1, pos.z));
                if (lowBlock != null)
                {
                    lowBlock.Attributes.Remove(BlockAttributes.WithGrass);
                    cc.GetBlockObject(lowBlock.Position).renderer.material.mainTexture =
                        rs_mgr.GetTexture(BlockType.Dirt);
                }
            }
        }
        else
        {
            cc.AddBlock(pos, blockType);
        }
        
    }


    public void RemoveBlock(Vector3 pos)
    {
        var cc = GetChunkControllerByBlock(pos);
        cc.RemoveBlock(pos);
    }


    public Block GetBlock(Vector3 pos)
    {
        var cc = GetChunkControllerByBlock(pos);
        return cc.GetBlock(pos);
    }


    public GameObject GetBlockObject(Vector3 pos)
    {
        var cc = GetChunkControllerByBlock(pos);
        return cc.GetBlockObject(pos);
    }



    public Block[] GetNearBlocks(Vector3 pos)
    {
        var cc = GetChunkControllerByBlock(pos);
        return cc.GetNearBlocks(pos);
    }


    private ChunkController GetChunkControllerByBlock(Vector3 pos)
    {
        var chunk = GetChunkByBlock(pos);
        if (chunk == null) return null;
        return chunk.GetComponent<ChunkController>();
    }


    private Transform GetChunkByBlock(Vector3 pos)
    {
        var x = pos.x < 0 ? ((int)pos.x - ChunkSizeX + 1) / ChunkSizeX : (int)pos.x / ChunkSizeX;
        var y = pos.y < 0 ? ((int)pos.y - ChunkSizeY + 1) / ChunkSizeY : (int)pos.y / ChunkSizeY;
        var z = pos.z < 0 ? ((int)pos.z - ChunkSizeZ + 1) / ChunkSizeZ : (int)pos.z / ChunkSizeZ;
        var chunkPos = new Vector3(x, y, z);

        var chunkName = ChunkController.GetChunkName(chunkPos);
        //Debug.Log(chunkName);

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
