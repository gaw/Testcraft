using System;
using TestCraft.Core;
using UnityEngine;
using System.Collections;

public class ChunkController : MonoBehaviour
{
    public Transform Player;
    public IWorld World;
    public Vector3 MapPosition;

    private ObjectPool _objectPool;
    
    private float distanceLoad = 50;

    private Vector3 _center;

    private ChunkState _state = ChunkState.Empty;

    private Block[,,] _blocks;


    public void Start()
    {
        var chunksMapObject = GameObject.Find("ChunksMap");
        _objectPool = chunksMapObject.GetComponent<ObjectPool>();

        var chunksMap = gameObject.transform.parent.GetComponent<ChunksMap>();
        _blocks = new Block[chunksMap.ChunkSizeX, chunksMap.ChunkSizeY, chunksMap.ChunkSizeZ];
        _center = new Vector3((int)transform.position.x + chunksMap.ChunkSizeX / 2,
                             (int)transform.position.y + chunksMap.ChunkSizeY / 2,
                             (int)transform.position.z + chunksMap.ChunkSizeZ / 2);

        //var o = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //o.transform.parent = transform;
        //o.transform.position = Center;

        //Debug.Log("Chunk Created");
    }


    public void Update () {
        if (_state == ChunkState.Empty && DistanceToPlayer(transform.position) <= distanceLoad)
        {
            //Debug.Log("Chunk Loading");
            _state = ChunkState.Loading;
            StartCoroutine(LoadBlocks());
        }
        else if (_state == ChunkState.Finished && DistanceToPlayer(transform.position) > (distanceLoad))
        {
            DisableChunk();
        }
        else if (_state == ChunkState.Disabled && DistanceToPlayer(transform.position) <= (distanceLoad))
        {
            EnableChunk();
        }	
	}


    public void AddBlock(Vector3 pos, BlockType blockType)
    {
        var block = new Block(pos) {BlockType = blockType};
        SetBlock(pos, block);

        var o = _objectPool.GetObjects(1)[0];
        o.transform.parent = transform;
        o.transform.position = pos;
        o.SetActive(true);
    }


    public void RemoveBlock(Vector3 pos)
    {
        int amountChild = transform.GetChildCount(); 
        for (int i = 0; i < amountChild; i++)
        {
            var c = transform.GetChild(i);
            if (c.position == pos)
                Destroy(c.gameObject);  // todo ������ � ���
        }       
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
        return _blocks[(int)pos.x - (int)transform.position.x, (int)pos.y - (int)transform.position.y, (int)pos.z - (int)transform.position.z];
    }


    private float DistanceToPlayer(Vector3 position)
    {
        return Vector3.Distance(Player.transform.position, _center);
    }

    private IEnumerator LoadBlocks()
    {
        //Debug.Log(string.Format("Load chunk: {0}", name));

        var startDate = DateTime.Now;

        var blocks = World.GetChunkBlocks(MapPosition);

        if (blocks.Length > 0)
        {
            Debug.Log(string.Format("{0} blocks", blocks.Length));

            var startDate2 = DateTime.Now;
            
            const int countPerFrame = 100;

            for (var index = 0; index < blocks.Length; index += countPerFrame)
            {
                var count = blocks.Length - index < countPerFrame
                                ? blocks.Length - index
                                : countPerFrame;

                var objects = _objectPool.GetObjects(count);

                for (var i = 0; i < count; i++)
                {
                    var o = objects[i];
                    var block = blocks[index + i];
                    o.transform.parent = transform;
                    o.transform.position = block.Position;

                    SetBlock(block.Position, block);
                }

                yield return null;
            }

            Debug.Log(string.Format("Loading blocks {3}: {0}, {1}. {2} blocks", startDate2 - startDate, DateTime.Now - startDate2, blocks.Length, name));
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
