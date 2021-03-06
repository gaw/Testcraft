using System;
using System.Collections.Generic;
using TestCraft.Core;
using UnityEngine;
using System.Collections;

public class ChunkController : MonoBehaviour
{
    public Transform Player;
    public World World;
    public Vector3 MapPosition;

    private ChunksMap _chunksMap;
    private ObjectPool _objectPool;
    private ResourceManager rs_mgr;

    private const float DistanceLoad = 50;

    private Vector3 _center;

    private ChunkState _state = ChunkState.Empty;

    private Block[,,] _blocks;


    public void Start()
    {
        var chunksMapObject = GameObject.Find("ChunksMap");
        _objectPool = chunksMapObject.GetComponent<ObjectPool>();
        _chunksMap = chunksMapObject.GetComponent<ChunksMap>();

        _blocks = new Block[_chunksMap.ChunkSizeX, _chunksMap.ChunkSizeY, _chunksMap.ChunkSizeZ];
        _center = new Vector3((int)transform.position.x + _chunksMap.ChunkSizeX / 2,
                             (int)transform.position.y + _chunksMap.ChunkSizeY / 2,
                             (int)transform.position.z + _chunksMap.ChunkSizeZ / 2);

        //var o = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //o.transform.parent = transform;
        //o.transform.position = Center;

        //Debug.Log("Chunk Created");

        // �������� �������� ��������
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


    public void Update () {
        if (_state == ChunkState.Empty && DistanceToPlayer(transform.position) <= DistanceLoad)
        {
            //Debug.Log("Chunk Loading");
            _state = ChunkState.Loading;
            StartCoroutine(LoadBlocks());
        }
        else if (_state == ChunkState.Finished && DistanceToPlayer(transform.position) > (DistanceLoad))
        {
            DisableChunk();
        }
        else if (_state == ChunkState.Disabled && DistanceToPlayer(transform.position) <= (DistanceLoad))
        {
            EnableChunk();
        }	
	}


    public void AddBlock(Vector3 pos, BlockType blockType, params string[] attributes)
    {
        var block = new Block(pos) {BlockType = blockType, Attributes = new List<string>(attributes)};
        SetBlock(pos, block);

        CreateBlockObject(block);

        RefreshNearBlocks(pos);
    }


    public void RemoveBlock(Vector3 pos)
    {
        SetBlock(pos, null);
        RefreshNearBlocks(pos);

        var block = GetBlockObject(pos);
        if (block == null) return;

        Destroy(block.gameObject);  // todo ������ � ���
    }


    public void RefreshBlock(Vector3 pos)
    {
        var block = GetBlock(pos);
        if (block == null)
            return;

        var blockObject = GetBlockObject(pos);
        var nearBlocks = GetNear8Blocks(pos);

        if (blockObject == null && nearBlocks.Length < 6)
        {
            CreateBlockObject(block);
            return;
        }

        if (blockObject != null && nearBlocks.Length == 6)
        {
            Destroy(blockObject);
        }
    }


    public void RefreshNearBlocks(Vector3 pos)
    {
        foreach (var block in GetNear8Blocks(pos))
            _chunksMap.RefreshBlock(block.Position);
    }


    private void CreateBlockObject(Block block)
    {
        var o = _objectPool.GetObjects(1)[0];
        o.renderer.material.mainTexture = rs_mgr.GetTexture(block.BlockType, block.Attributes.ToArray());
        o.transform.parent = transform;
        o.transform.position = block.Position;
        o.AddComponent<BoxCollider>();
        o.SetActive(true);
    }


    private void SetBlock(Vector3 pos, Block block)
    {
        try
        {
            _blocks[(int)pos.x - (int)transform.position.x, (int)pos.y - (int)transform.position.y, (int)pos.z - (int)transform.position.z] = block;
        }
        catch (Exception)
        {
            Debug.Log(pos.ToString() + "  " + transform.position.ToString());
        }
    }


    public Block GetBlock(Vector3 pos)
    {
        var x = (int)pos.x - (int)transform.position.x;
        var y = (int)pos.y - (int)transform.position.y;
        var z = (int)pos.z - (int)transform.position.z;

        if (x < 0 || x >= _chunksMap.ChunkSizeX || 
            y < 0 || y >= _chunksMap.ChunkSizeY || 
            z < 0 || z >= _chunksMap.ChunkSizeZ)
        {
            return _chunksMap.GetBlock(pos);
        }

        return _blocks[x, y, z];
    }


    public GameObject GetBlockObject(Vector3 pos)
    {
        int amountChild = transform.GetChildCount();
        for (int i = 0; i < amountChild; i++)
        {
            var c = transform.GetChild(i);
            if (c.position == pos)
                return c.gameObject;
        }
        return null;
    }


    public Block[] GetNearBlocks(Vector3 pos)
    {
        var blocks = new List<Block>();

        for (var i = -1; i <= 1; i++)
            for (var j = -1; j <= 1; j++)
                for (var k = -1; k <= 1; k++)
                {
                    if (i == 0 && j == 0 && k == 0) continue;
                    var b = GetBlock(new Vector3(pos.x + i, pos.y + j, pos.z + k));
                    if (b != null) blocks.Add(b);
                }

        return blocks.ToArray();
    }


    public Block[] GetNear8Blocks(Vector3 pos)
    {
        var blocks = new List<Block>();

        blocks.Add(GetBlock(new Vector3(pos.x + 1, pos.y, pos.z)));
        blocks.Add(GetBlock(new Vector3(pos.x - 1, pos.y, pos.z)));
        blocks.Add(GetBlock(new Vector3(pos.x, pos.y + 1, pos.z)));
        blocks.Add(GetBlock(new Vector3(pos.x, pos.y - 1, pos.z)));
        blocks.Add(GetBlock(new Vector3(pos.x, pos.y, pos.z + 1)));
        blocks.Add(GetBlock(new Vector3(pos.x, pos.y, pos.z - 1)));

        return blocks.FindAll(a => a != null).ToArray();
    }

    

    private float DistanceToPlayer(Vector3 position)
    {
        return Vector3.Distance(Player.transform.position, _center);
    }

    private IEnumerator LoadBlocks()
    {
        //Debug.Log(string.Format("Load chunk: {0}", name));

        var startDate = DateTime.Now;

        var blocks = new List<Block>();
        var blocksToCreate = new Block[_chunksMap.ChunkSizeX, _chunksMap.ChunkSizeZ];
        for (var i = (int)transform.position.x; i < transform.position.x + _chunksMap.ChunkSizeX; i++)
            for (var j = (int)transform.position.y; j < transform.position.y + _chunksMap.ChunkSizeY; j++)
                for (var k = (int)transform.position.z; k < transform.position.z + _chunksMap.ChunkSizeZ; k++)
                {
                    var b = World.GetBlock(i, j, k);
                    if (b == null) continue;
                    blocks.Add(b);

                    var highestBlock = blocksToCreate[i - (int) transform.position.x, k - (int) transform.position.z];
                    if (highestBlock == null || highestBlock.Position.y < j)
                        blocksToCreate[i - (int) transform.position.x, k - (int) transform.position.z] = b;
                }

            if (blocks.Count > 0)
            {
                foreach (var block in blocks) SetBlock(block.Position, block);

                blocks.Clear();
                for (var i = 0; i < _chunksMap.ChunkSizeX; i++)
                    for (var j = 0; j < _chunksMap.ChunkSizeZ; j++)
                    {
                        if(blocksToCreate[i,j] != null)
                            blocks.Add(blocksToCreate[i,j]);
                    }

                Debug.Log(string.Format("{0} blocks", blocks.Count));

                var startDate2 = DateTime.Now;

                const int countPerFrame = 100;

                for (var index = 0; index < blocks.Count; index += countPerFrame)
                {
                    var count = blocks.Count - index < countPerFrame
                                    ? blocks.Count - index
                                    : countPerFrame;

                    var objects = _objectPool.GetObjects(count);

                    for (var i = 0; i < count; i++)
                    {
                        var o = objects[i];
                        var block = blocks[index + i];
                        o.transform.parent = transform;
                        o.transform.position = block.Position;
                    }

                    yield return null;
                }

                Debug.Log(string.Format("Loading blocks {3}: {0}, {1}. {2} blocks", startDate2 - startDate, DateTime.Now - startDate2, blocks.Count, name));
            }

        _state = ChunkState.Finished;

        EnableChunk();

        //int amountChild = transform.GetChildCount(); // ���������� �����.
        //for (int i = 0; i < amountChild; i++)
        //{
        //    transform.GetChild(i).gameObject.SetActive(true);
        //}
    }


    private void EnableChunk()
    {
        //Debug.Log(string.Format("Enabling: {0}", name));

        int amountChild = this.transform.GetChildCount(); // ���������� �����.
        for (int i = 0; i < amountChild; i++)
        {
            var child = transform.GetChild(i).gameObject;
            var bc = child.GetComponent<BoxCollider>();
            if (bc != null)
            {
                bc.enabled = true;
            }
            else
            {
                child.AddComponent<BoxCollider>();
            }

            transform.GetChild(i).gameObject.SetActive(true);
        }

        _state = ChunkState.Finished;
    }


    private void DisableChunk()
    {
        int amountChild = this.transform.GetChildCount(); // ���������� �����.
        for (int i = 0; i < amountChild; i++)
        {
            var bc = transform.GetChild(i).gameObject.GetComponent<BoxCollider>();
            if (bc != null)
                bc.enabled = false;

            transform.GetChild(i).gameObject.SetActive(false);
        }

        _state = ChunkState.Disabled;
    }


    public static string GetChunkName(Vector3 position)
    {
        return string.Format("Chunk_{0}_{1}_{2}", position.x, position.y, position.z);
    }


    private enum ChunkState
    {
        Empty,
        Loading,
        Finished,
        Disabled
    }
}
