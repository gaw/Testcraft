using TestCraft.Core;
using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour
{
    private ChunksMap gm_mgr;
    public Texture2D Crosshair;
    private BlockType _selectedBlockType;

    private void OnGUI()
    {
        if(Crosshair != null)
        GUI.DrawTexture(new Rect(Screen.width/2 - 12, Screen.height/2-12, 25,25), Crosshair);
    }

    private void Start()
    {
        var mgr = GameObject.FindGameObjectWithTag("GM_MGR");
        if (mgr == null)
        {
            print("mgr is null");
        }
        else
        {
            gm_mgr = mgr.GetComponent<ChunksMap>();
        }

        _selectedBlockType = BlockType.Dirt;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            _selectedBlockType = BlockType.Dirt;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _selectedBlockType = BlockType.Stone;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _selectedBlockType = BlockType.IronOre;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _selectedBlockType = BlockType.Wood;
        }
    

        var ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 5))
        {
            if (Input.GetMouseButtonDown(0))
            {
                gm_mgr.RemoveBlock(hit.collider.transform.position);
                
            }
            if (Input.GetMouseButtonDown(1) && hit.distance >= 1.5f)
            {
                gm_mgr.AddBlock(hit.transform.position + hit.normal, _selectedBlockType);
            }
        }    
    }
}
