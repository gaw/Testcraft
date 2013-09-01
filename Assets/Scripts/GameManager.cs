using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	
	private IWorld _world;
	
	public Transform Block;
    public Texture Texture;
	
	public Transform Player;
	private Vector3 _position;

	void Start ()
	{
	    _world = new World(10, 2, 10);
		_position = Player.position;
	}
	
	void LateUpdate () 
	{
		if(_position != Player.position)
		{
			_position = Player.position;
			UpdateBlocks();
		}
	}
	
	
	Dictionary<string, Transform> _blocks = new Dictionary<string, Transform>();
	List<Transform> _blocksPool = new List<Transform>();
	
	
	bool nya = false;
	
	private void UpdateBlocks()
	{
        //var blocks = _world.GetNearBlocks(_position);
        //Vector3[] newBlocks = Array.ConvertAll(blocks, a => a.Position);

        //Vector3[] blocks = _world.GetBlocks(_position);

        //if (blocks.Length > 0)
        //    Debug.Log(blocks.Length);

        ////if (nya) return;
        ////if(newBlocks.Length > 0) nya = true;
		
        //foreach(var block in blocks)
        //{
        //    var hash = Utils.VectorToHash(block);
        //    if(!_blocks.ContainsKey(hash))
        //    {
        //        var o = CreateBlock();
        //        o.position = block;
        //        _blocks.Add(hash, o);
        //    }
        //}

        //return;

        //foreach(var block in _blocks)
        //{
        //    if(!Array.Exists(blocks, a => Utils.VectorToHash(a) == block.Key))
        //    {
        //        block.Value.gameObject.SetActive(false);
        //    }
        //}

        //return;

        //foreach(var block in _blocks)
        //{
        //    if (!block.Value.gameObject.activeSelf)
        //    {
        //        _blocksPool.Add(block.Value);
        //        _blocks.Remove(block.Key);
        //    }
        //}

	}


    private Material _material;
	
	private Transform CreateBlock()
	{
		if(_blocksPool.Count > 0)
		{
			var o = _blocksPool[_blocksPool.Count - 1];
			_blocksPool.RemoveAt(_blocksPool.Count - 1);
			o.gameObject.SetActive(true);
			return o;
		}
		else
		{
			var o = Instantiate(Block) as Transform;

            if (_material == null)
            {
                _material = o.renderer.material;
                _material.mainTexture = Texture;
            }
		    o.renderer.material = _material;
		    return o;
		}
	}
}
