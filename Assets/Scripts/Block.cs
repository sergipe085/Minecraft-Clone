using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minecraft.WorldGeneration
{
    public class Block
    {
        private Vector3Int localPosition = Vector3Int.zero;
        private Vector3 worldPosition = Vector3.zero;
        private Chunk   chunkParent   = null;
        private BlockType type;
        private bool isSolid = false;

        public Block(Vector3Int _localPosition, Vector3 _worldPosition, BlockType _type, Chunk _chunkParent) {
            localPosition = _localPosition;
            worldPosition = _worldPosition;
            type          = _type;
            chunkParent   = _chunkParent;
            isSolid       = type != BlockType.AIR;
        }

        public void GenerateBlock() {
            if (type == BlockType.AIR) return;

            if (!IsSolid(localPosition.x, localPosition.y + 1, localPosition.z))
                GenerateFace(BlockFace.TOP);
            if (!IsSolid(localPosition.x, localPosition.y - 1, localPosition.z))
                GenerateFace(BlockFace.BOTTOM);
            if (!IsSolid(localPosition.x + 1, localPosition.y, localPosition.z))
                GenerateFace(BlockFace.RIGHT);
            if (!IsSolid(localPosition.x - 1, localPosition.y, localPosition.z))
                GenerateFace(BlockFace.LEFT);
            if (!IsSolid(localPosition.x, localPosition.y, localPosition.z + 1))
                GenerateFace(BlockFace.FRONT);
            if (!IsSolid(localPosition.x, localPosition.y, localPosition.z - 1))
                GenerateFace(BlockFace.BACK);
        }

        public void GenerateFace(BlockFace face) {
            chunkParent.meshStruct.vertices.AddRange(BlockData.GetVerticesPositions(face, worldPosition));
            chunkParent.meshStruct.normals.AddRange(BlockData.normals[face]);
            chunkParent.meshStruct.uvs.AddRange(BlockData.grassUVs[face]);
            chunkParent.meshStruct.triangles.AddRange(BlockData.GetTrianglesPositions(face, chunkParent.currentIndex));
            chunkParent.currentIndex++;
        }

        private bool IsSolid(int x, int y, int z) {
            try {
                return chunkParent.chunckData[x, y, z].isSolid;
            } catch (System.Exception e) {
                
            } 
            return false;
        }
    }
}
