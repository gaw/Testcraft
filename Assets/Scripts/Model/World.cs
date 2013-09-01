using System;
using System.ComponentModel;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace TestCraft.Core
{

    public class World : IWorld
    {
        private List<Chunk> _chunks = new List<Chunk>();

        public World(int chunkSizeX, int chunkSizeY, int chunkSizeZ)
        {
            _chunkXSize = chunkSizeX;
            _chunkYSize = chunkSizeY;
            _chunkZSize = chunkSizeZ;
        }

        private List<Vector3> _blocks = new List<Vector3>();
        private Vector3 _position = new Vector3(0, 0, 0);
        private int _radius = 20;

        private int _chunkXSize;
        private int _chunkYSize;
        private int _chunkZSize;
        private int _chunkLoadRadius = 2;


        private Chunk[] _lastChunks = new Chunk[0];
        private Vector3[] _lastBlocks = new Vector3[0];

        public Vector3[] GetBlocks(Vector3 position)
        {
            var chunks = GetChunks(position);
            if (chunks.Length == _lastChunks.Length &&
                Array.TrueForAll(chunks, a => Array.Exists(_lastChunks, b => a == b)))
                return _lastBlocks;

            _lastChunks = chunks;

            var blocks = new List<Vector3>();
            for (var i = 0; i < _chunkXSize; i++)
                for (var j = 0; j < _chunkYSize; j++)
                    for (var k = 0; k < _chunkZSize; k++)
                        foreach (var chunk in chunks)
                        {
                            if (chunk.Blocks[i, j, k] == 1)
                                blocks.Add(new Vector3(i + chunk.X*_chunkXSize, j + chunk.Y*_chunkYSize,
                                                       k + chunk.Z*_chunkZSize));
                        }

            _lastBlocks = blocks.ToArray();
            return _lastBlocks;
        }

        public IBlock[] GetNearBlocks(Vector3 position)
        {
            var chunks = GetChunks(position);
            var blocks = new List<IBlock>();
            foreach (var chunk in chunks)
                for (var i = 0; i < _chunkXSize; i++)
                    for (var j = 0; j < _chunkYSize; j++)
                        for (var k = 0; k < _chunkZSize; k++)
                        {
                            if (chunk.Blocks[i, j, k] == 1)
                                blocks.Add(
                                    new Block(new Vector3(i + chunk.X*_chunkXSize, j + chunk.Y*_chunkYSize,
                                                          k + chunk.Z*_chunkZSize)));
                        }

            return blocks.ToArray();
        }

        public void DeleteBlock(Vector3 position)
        {
            throw new System.NotImplementedException();
        }

        public void SetBlock(Vector3 position, BlockType cType)
        {
            throw new System.NotImplementedException();
        }

        public IBlock GetBlock(Vector3 position)
        {
            throw new System.NotImplementedException();
        }

        public Block[] GetChunkBlocks(Vector3 position)
        {
            var chunkName = ChunkController.GetChunkName(position);

            var chunk = _chunks.Find(a => a.Name == chunkName) ??
                        CreateChunk((int) position.x, (int) position.y, (int) position.z);
            if (chunk == null) return new Block[0];

            var blocks = new List<Vector3>();
            for (var i = 0; i < _chunkXSize; i++)
                for (var j = 0; j < _chunkYSize; j++)
                    for (var k = 0; k < _chunkZSize; k++)
                        if (chunk.Blocks[i, j, k] == 1)
                        {
                            var vector = new Vector3(i + chunk.X*_chunkXSize, j + chunk.Y*_chunkYSize,
                                                     k + chunk.Z*_chunkZSize);
                            blocks.Add(vector);
                        }

            return blocks.ConvertAll(a => new Block(a)).ToArray();
        }

        private Chunk[] GetChunks(Vector3 pos)
        {
            var chunkX = (int) Mathf.Round(pos.x/_chunkXSize);
            var chunkY = (int) Mathf.Round(pos.y/_chunkYSize);
            var chunkZ = (int) Mathf.Round(pos.z/_chunkZSize);

            var chunks = new List<Chunk>();

            for (var i = chunkX - _chunkLoadRadius; i <= chunkX + _chunkLoadRadius; i++)
                for (var j = chunkY - _chunkLoadRadius; j <= chunkY + _chunkLoadRadius; j++)
                    for (var k = chunkZ - _chunkLoadRadius; k <= chunkZ + _chunkLoadRadius; k++)
                    {
                        var chunk = _chunks.Find(a => a.X == i && a.Y == j && a.Z == k) ?? CreateChunk(i, j, k);
                        chunks.Add(chunk);
                    }

            return chunks.ToArray();
        }


        private Chunk CreateChunk(int x, int y, int z)
        {
            var chunk = new Chunk(x, y, z, _chunkXSize, _chunkYSize, _chunkZSize);

            var worldX = x*_chunkXSize;
            var worldY = y*_chunkYSize;
            var worldZ = z*_chunkZSize;

            if (y == 0)
                for (var i = 0; i < _chunkXSize; i++)
                    for (var j = 0; j < _chunkYSize; j++)
                        for (var k = 0; k < _chunkZSize; k++)
                        {
                            if (worldY + j == 0 || (worldY + j == 1 && Random.Range(0, 7) == 0))
                                chunk.Blocks[i, j, k] = 1;
                            else
                                chunk.Blocks[i, j, k] = 0;
                        }

            _chunks.Add(chunk);
            //Debug.Log(string.Format("Chunks: {0}", _chunks.Count));

            return chunk;
        }


        private class Chunk
        {
            public string Name;
            public int X;
            public int Y;
            public int Z;

            public Chunk(int x, int y, int z, int sizeX, int sizeY, int sizeZ)
            {
                X = x;
                Y = y;
                Z = z;
                Name = ChunkController.GetChunkName(new Vector3(x, y, z));
                Blocks = new byte[sizeX,sizeY,sizeZ];
            }

            public byte[,,] Blocks;
        }

    }
}
