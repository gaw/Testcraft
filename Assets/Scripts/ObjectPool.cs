using System.Collections.Generic;
using TestCraft.Core;
using UnityEngine;
using System.Collections;

public class ObjectPool : MonoBehaviour
{
    private List<GameObject> Pool;

    public GameObject Prefab;
    public int MinimalCount = 5000;

    private int MaxCreatePerUpdate = 5;

    private ResourceManager rs_mgr;

    private void Awake()
    {

    }

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

        var tex = rs_mgr != null
              ? rs_mgr.GetTexture(BlockType.Dirt, new string[] { "with_grass" })
              : (Texture2D)
                Resources.LoadAssetAtPath("Assets/Textures/dirt_with_grass.jpg", typeof(Texture2D));
        Prefab.renderer.material.mainTexture = tex;

        if (Prefab == null)
        {
            //Prefab = Resources.LoadAssetAtPath("Assets/Prefabs/Block_PFB.prefab", typeof (GameObject)) as GameObject;
            tex = rs_mgr != null
                          ? rs_mgr.GetTexture(BlockType.Dirt, new string[] {"with_grass"})
                          : (Texture2D)
                            Resources.LoadAssetAtPath("Assets/Textures/dirt_with_grass.jpg", typeof (Texture2D));
            Prefab.renderer.material.mainTexture = tex;
        }

        Pool = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if(Pool.Count < MinimalCount) FillPool();
	}


    void FillPool()
    {
        var count = MinimalCount - Pool.Count;
        if (count > MaxCreatePerUpdate) count = MaxCreatePerUpdate;
        FillPool(count);
    }


    void FillPool(int count)
    {
        for (var i = 0; i < count; i++)
        {
            var o = Instantiate(Prefab) as GameObject;
            o.SetActive(false);
            //o.transform.parent = transform;
            Pool.Add(o);
        }

        //Debug.Log("ObjectPool: " + Pool.Count);
    }
    
    
    public GameObject[] GetObjects(int count)
    {
        if (Pool.Count > count)
        {
            var res = Pool.GetRange(Pool.Count - count, count);
            Pool.RemoveRange(Pool.Count - count, count);
            return res.ToArray();
        }

        if (Pool.Count < count)
        {
            FillPool(count - Pool.Count);
        }

        var result = Pool.ToArray();
        Pool.Clear();
        return result;
    }
}
