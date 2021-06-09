using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minecraft.WorldGeneration
{
    public class Block
    {
        private Vector3 localPosition = Vector3.zero;
        private Vector3 worldPosition = Vector3.zero;
        private Chunk   chunkParent   = null;

        public Block(Vector3 _localPosition, Vector3 _worldPosition, Chunk _chunkParent) {
            localPosition = _localPosition;
            worldPosition = _worldPosition;
            chunkParent   = _chunkParent;
        }

        public void GenerateBlock() {
            GenerateFace(BlockFace.TOP);
            GenerateFace(BlockFace.BOTTOM);
            GenerateFace(BlockFace.RIGHT);
            GenerateFace(BlockFace.LEFT);
            GenerateFace(BlockFace.FRONT);
            GenerateFace(BlockFace.BACK);
        }

        public void GenerateFace(BlockFace face) {
            chunkParent.meshStruct.vertices.AddRange(BlockData.GetVerticesPositions(face, worldPosition));
            chunkParent.meshStruct.normals.AddRange(BlockData.normals[face]);
            chunkParent.meshStruct.uvs.AddRange(BlockData.uvs[face]);
            chunkParent.meshStruct.triangles.AddRange(BlockData.GetTrianglesPositions(face, chunkParent.currentIndex));
            chunkParent.currentIndex++;
        }
    }
}
