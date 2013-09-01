using System.Text;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TestCraft.Core;

public class ResourceManager : MonoBehaviour
{
    private const string TextureFolder = "Textures";

    private readonly List<string> _textureFiles = new List<string>()
        {
            "dirt_with_grass.jpg",
            "dirt.jpg",
            "stone.jpg",
            "wood.jpg",
            "iron_ore.jpg"
        };


    private Dictionary<string, Texture2D> Textures;

    private void Awake()
    {
        // Формируем список текстур
        Textures = new Dictionary<string, Texture2D>();
        foreach (var txt in _textureFiles)
        {
            var texture = LoadTexture(string.Format("Assets/{0}/{1}", TextureFolder, txt));

            if(texture != null)
                Textures.Add(txt, texture);

        }
    }

    private Texture2D LoadTexture(string path)
    {
        return Resources.LoadAssetAtPath(path, typeof (Texture2D)) as Texture2D;
    }

    public Texture2D GetTexture(BlockType type, params string[] attributes)
    {
        var textSearch = type.GetTypeName();
        if (attributes.Length > 0)
        {
            var sb = new StringBuilder();
            foreach (var attr in attributes)
            {
                sb.Append(attr.ToUpper());
            }
            textSearch = textSearch + sb.ToString();
        }
        textSearch = textSearch + ".JPG";
        var tx = Textures.FirstOrDefault(x => x.Key.Replace("_", string.Empty).ToUpper() == textSearch.Replace("_", string.Empty));
        if (tx.Value != null)
            return tx.Value;

        //TODO сообщение об ошибке

        return null;

    }
}
