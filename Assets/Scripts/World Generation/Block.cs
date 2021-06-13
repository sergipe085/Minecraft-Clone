using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minecraft.WorldGeneration
{
    public class Block
    {
        public Vector3Int localPosition = Vector3Int.zero;
        private Vector3Int worldPosition = Vector3Int.zero;
        private Chunk   chunkParent      = null;
        public  BlockType type;
        private bool isSolid = false;

        public BlockType healthBlockType;
        int currentHealth = 10;

        private event Action onDestroy;

        private List<Vector3> vertices = new List<Vector3>();
        private List<int> triangles = new List<int>();

        public Block(Vector3Int _localPosition, Vector3Int _worldPosition, BlockType _type, Chunk _chunkParent) {
            localPosition = _localPosition;
            worldPosition = _worldPosition;
            type          = _type;
            chunkParent   = _chunkParent;
            isSolid       = type != BlockType.AIR;
            healthBlockType = BlockType.NOCRACK;

            onDestroy += CreateParticles;
            onDestroy += CreatePickup;
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

            triangles.Add(vertices.Count);
            triangles.Add(vertices.Count + 1);
            triangles.Add(vertices.Count + 2);
            triangles.Add(vertices.Count);
            triangles.Add(vertices.Count + 2);
            triangles.Add(vertices.Count + 3);
            vertices.AddRange(BlockData.GetVerticesPositions(face, localPosition));

            chunkParent.meshStruct.uvs.AddRange(BlockData.uvs[type][side]);
            chunkParent.meshStruct.suvs.AddRange(BlockData.uvs[healthBlockType][side]);

            chunkParent.meshStruct.triangles.AddRange(BlockData.GetTrianglesPositions(face, chunkParent.currentIndex));
            chunkParent.currentIndex++;
        }

        public void SetType(BlockType newType) {
            type = newType;
            if (type == BlockType.AIR) {
                isSolid = false;
                return;
            }
            isSolid = true;

            healthBlockType = BlockType.NOCRACK;
        }

        public bool BuildBlock(BlockType type) {
            SetType(type);
            chunkParent.Redraw();
            return true;
        }

        public bool HitBlock() {
            if (type == BlockType.BEDROCK) return false;

            healthBlockType = (BlockType)(17 - currentHealth);
            currentHealth--;

            if (currentHealth < 0) {
                onDestroy();

                type = BlockType.AIR;
                isSolid = false;
                healthBlockType = BlockType.NOCRACK;
                chunkParent.Redraw();
                return true;
            }

            chunkParent.Redraw();
            return false;
        }

        public void CancelHit() {
            currentHealth = 10;
            healthBlockType = BlockType.NOCRACK;
            chunkParent.Redraw();
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

        private void CreateParticles() {
            GameObject particlePrefab = Resources.Load<GameObject>("DestroyParticle");
            GameObject particleInstance = GameObject.Instantiate(particlePrefab, worldPosition, Quaternion.identity);
            ParticleSystemRenderer particleSystemRenderer = particleInstance.GetComponent<ParticleSystem>().GetComponent<ParticleSystemRenderer>();

            Vector3[] vertices = new Vector3[4] {
                new Vector3(0, 0, 0), new Vector3(0.3f, 0, 0), new Vector3(0.3f, 0.3f, 0), new Vector3(0, 0.3f, 0)
            };

            int[] triangles = new int[12] {
                0, 3, 1, 1, 3, 2,
                1, 3, 0, 2, 3, 1,
            };

            List<Vector2> uvs = new List<Vector2>();
            float uvOffset = 0.0275f;
            uvs.Add(BlockData.uvs[type][TileSide.TOP][0] + new Vector2(uvOffset, uvOffset));
            uvs.Add(BlockData.uvs[type][TileSide.TOP][1] + new Vector2(-uvOffset,uvOffset));
            uvs.Add(BlockData.uvs[type][TileSide.TOP][2] + new Vector2(-uvOffset,-uvOffset));
            uvs.Add(BlockData.uvs[type][TileSide.TOP][3] + new Vector2(uvOffset, -uvOffset));
            

            Mesh mesh      = new Mesh();
            mesh.vertices  = vertices;
            mesh.uv        = uvs.ToArray();
            mesh.triangles = triangles;

            particleSystemRenderer.mesh = mesh;
        }

        private void CreatePickup() {
            GameObject pickupPrefab   = Resources.Load<GameObject>("PickupPrefab");
            GameObject pickupInstance = GameObject.Instantiate(pickupPrefab, worldPosition, Quaternion.identity);
            PickupBlock pickupBlock   = pickupInstance.GetComponent<PickupBlock>();
            pickupBlock.Setup(type);
        }
    }
}
