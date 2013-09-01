using TestCraft.Core;
using UnityEngine;
using System.Collections;

public class GUIhelper : MonoBehaviour {

    public Texture2D Inventory;
    public Texture2D Focus;
    private const int ScreenOffset = 10;
    private PlayerInput _player;

	void Start ()
	{
	    _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
	}

    private void OnGUI()
    {
        var slotsRect = new Rect(Screen.width/2 - Inventory.width /2, Screen.height - Inventory.height - ScreenOffset, Inventory.width, Inventory.height);
        GUI.DrawTexture(slotsRect, Inventory);

        var num = 0;
        switch (_player.SelectedBlockType)
        {
                case BlockType.Wood:
                num = 0;
                break;
                case BlockType.Dirt:
                num = 1;
                break;
                case BlockType.IronOre:
                num = 2;
                break;
                case BlockType.Stone:
                num = 3;
                break;
        }

        var focusPosX = Screen.width / 2 - Inventory.width / 2 + Focus.width * num;
        var focusPosY = Screen.height - Inventory.height - ScreenOffset;
        GUI.DrawTexture(new Rect(focusPosX, focusPosY, Focus.width, Focus.height ), Focus);
    }
}
