using System;
using System.Collections.Generic;
using TestCraft.Core;
using UnityEngine;
using System.Collections;

public class ObjectPool : MonoBehaviour
{
    private GameObject[] Pool;
    private int _objectCount;

    public GameObject Prefab;
    public int MinimalCount = 5000;

    private int MaxCreatePerUpdate = 20;

    private ResourceManager rs_mgr;

    private Mesh _mesh;
    private Material _material;

    void Start () 
    {
        var rs = GameObject.FindGameObjectWithTag("RS_MGR");
        if (rs != null)
        {
            rs_mgr = rs.GetComponent<ResourceManager>();
        }
        else
        {
            print("RESOURCE MANAGER NOT FOUND");
        }

        _mesh = Prefab.GetComponent<MeshFilter>().sharedMesh;
        _material = Prefab.renderer.material;
        _material.mainTexture = rs_mgr.GetTexture(BlockType.Dirt, new string[] {"with_grass"});
            //(Texture2D)Resources.LoadAssetAtPath("Assets/Textures/dirt_with_grass.jpg", typeof(Texture2D));

        Pool = new GameObject[10000];
        _objectCount = 0;

        var start = DateTime.Now;
        FillPool(10000);
        Debug.Log(DateTime.Now - start);
	}
	
	// Update is called once per frame
	public void Update () 
    {
	    if(_objectCount < MinimalCount) FillPool();
	}


    void FillPool()
    {
        var count = MinimalCount - _objectCount;
        if (count > MaxCreatePerUpdate) count = MaxCreatePerUpdate;
        FillPool(count);
    }


    void FillPool(int count)
    {
        for (var i = 0; i < count; i++)
        {
            var o = new GameObject();
            var filter = o.AddComponent<MeshFilter>();
            filter.sharedMesh = _mesh;

            o.AddComponent<MeshRenderer>().material = _material;
            //o.AddComponent<BoxCollider>();

            //var o = Instantiate(Prefab) as GameObject;
            o.SetActive(false);
            //o.transform.parent = transform;

            Pool[_objectCount++] = o;
        }

        //Debug.Log("ObjectPool: " + _objectCount);
    }
    
    
    public GameObject[] GetObjects(int count)
    {
        var res = new GameObject[count];

        if (_objectCount > count)
        {
            Array.Copy(Pool, _objectCount - count, res, 0, count);
            _objectCount -= count;
            return res;
        }

        if (_objectCount < count)
        {
            FillPool(count - _objectCount);
        }

        Array.Copy(Pool, res, count);
        _objectCount = 0;
        return res;
    }
}
