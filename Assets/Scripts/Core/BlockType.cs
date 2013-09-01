using UnityEngine;
using System.Collections;

namespace TestCraft.Core
{
    public enum BlockType
    {
        Dirt = 2,
        Stone = 4,
        IronOre = 3,
        Wood = 1
    }

    public static class BlockExtensions
    {
        public static string GetTypeName(this BlockType type)
        {
            var result = type.ToString().ToUpper();

            switch (type)
            {
                case BlockType.Dirt:
                    result = "DIRT";
                    break;

                case BlockType.IronOre:
                    result = "IRONORE";
                    break;

                case BlockType.Stone:
                    result = "STONE";
                    break;

                case BlockType.Wood:
                    result = "WOOD";
                    break;
            }

            return result;
        }
    }
}
