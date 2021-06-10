using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minecraft.WorldGeneration
{
    public class Block
    {
        private Vector3Int localPosition = Vector3Int.zero;
        private Vector3Int worldPosition = Vector3Int.zero;
        private Chunk   chunkParent      = null;
        public  BlockType type;
        private bool isSolid = false;

        public Block(Vector3Int _localPosition, Vector3Int _worldPosition, BlockType _type, Chunk _chunkParent) {
            localPosition = _localPosition;
            worldPosition = _worldPosition;
            type          = _type;
            chunkParent   = _chunkParent;
            isSolid       = type != BlockType.AIR;
        }

        public void GenerateBlock() {
            if (type == BlockType.AIR) return;

            if (!IsSolid(localPosition.x, localPosition.y + 1, localPosition.z))
                GenerateFace(BlockFace.TOP, TileSide.TOP);
            if (!IsSolid(localPosition.x, localPosition.y - 1, localPosition.z))
                GenerateFace(BlockFace.BOTTOM, TileSide.BOTTOM);
            if (!IsSolid(localPosition.x + 1, localPosition.y, localPosition.z))
                GenerateFace(BlockFace.RIGHT, TileSide.SIDE);
            if (!IsSolid(localPosition.x - 1, localPosition.y, localPosition.z))
                GenerateFace(BlockFace.LEFT, TileSide.SIDE);
            if (!IsSolid(localPosition.x, localPosition.y, localPosition.z + 1))
                GenerateFace(BlockFace.FRONT, TileSide.SIDE);
            if (!IsSolid(localPosition.x, localPosition.y, localPosition.z - 1))
                GenerateFace(BlockFace.BACK, TileSide.SIDE);
        }

        public void GenerateFace(BlockFace face, TileSide side) {
            chunkParent.meshStruct.vertices.AddRange(BlockData.GetVerticesPositions(face, localPosition));
            chunkParent.meshStruct.normals.AddRange(BlockData.normals[face]);
            chunkParent.meshStruct.uvs.AddRange(BlockData.uvs[type][side]);
            chunkParent.meshStruct.triangles.AddRange(BlockData.GetTrianglesPositions(face, chunkParent.currentIndex));
            chunkParent.currentIndex++;
        }

        private bool IsSolid(int x, int y, int z) {
            Block[,,] chunkData;

            if (x < 0 || x >= World.chunkSize ||
                y < 0 || y >= World.chunkSize ||
                z < 0 || z >= World.chunkSize)
            {
                Vector3 neighbourChunckPos = chunkParent.chunkObject.transform.position + new Vector3((x - localPosition.x) * World.chunkSize, (y - localPosition.y) * World.chunkSize, (z - localPosition.z) * World.chunkSize);
                string neighbourChunckName = World.BuildChunckName(neighbourChunckPos);

                x = ConvertBlockIndexToLocal(x);
                y = ConvertBlockIndexToLocal(y);
                z = ConvertBlockIndexToLocal(z);

                Chunk neighbourChunk;
                if (World.chunks.TryGetValue(neighbourChunckName, out neighbourChunk))
                {
                    chunkData = neighbourChunk.chunkData;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                chunkData = chunkParent.chunkData;
            }

            try
            {
                if (chunkData[x, y, z] == null) return false;
                return chunkData[x, y, z].isSolid;
            }
            catch (System.IndexOutOfRangeException e) { }

            return false;
        }

        private int ConvertBlockIndexToLocal(int i) {
            if (i == -1) {
                i = World.chunkSize - 1;
            }
            else if (i == World.chunkSize) {
                i = 0;
            }
            return i;
        }
    }
}
