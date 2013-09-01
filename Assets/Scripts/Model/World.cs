using System;
using System.ComponentModel;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace TestCraft.Core
{

    public class World
    {
        private int[,] _heightMap;


        public World(int x, int z, float freq, float amp, float pers, int octave)
        {
            _heightMap = new int[x,z];
            var pn = new PerlinNoise(x, z);

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < z; j++)
                {
                    float value = pn.GetRandomHeight(i, j, 1, freq, amp, pers, octave);
                    value = 0.5f * (1 + value);
                    _heightMap[i,j] = (int)value * 4 + 1;
                }
            }
        }


        public Block GetBlock(int x, int y, int z)
        {
            //if(y == 0 || (y == 1 && Random.Range(0, 5) == 0))
            //    return new Block(new Vector3(x, y, z));

            if(y <3)
            //    return new Block(new Vector3(x, y, z));
            if (y >= 0 && y < _heightMap[x, z])
            {
                return new Block(new Vector3(x, y, z));
            }

            return null;
        }
    }
}
