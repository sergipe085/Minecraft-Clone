using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunck
{
    [SerializeField] private Material cubeMaterial = null;
    public Block[,,]  chunckData;
    public GameObject chunck = null;

    public Chunck(Vector3 _position, Material _cubeMaterial) {
        chunck = new GameObject(World.BuildChunckName(_position));
        chunck.transform.position = _position;
        cubeMaterial = _cubeMaterial;
        BuildChunck();
    }

    private void BuildChunck() {
        chunckData = new Block[World.chunckSize, World.chunckSize, World.chunckSize];

        //Create blocks
        for (int x = 0; x < World.chunckSize; x++) {   
            for (int y = 0; y < World.chunckSize; y++) {
                for (int z = 0; z < World.chunckSize; z++) {
                    Vector3 blockPos = new Vector3(x, y, z);
                    Block.BlockType type;

                    int xWorld = (int) (x + chunck.transform.position.x);
                    int yWorld = (int) (y + chunck.transform.position.y);
                    int zWorld = (int) (z + chunck.transform.position.z);

                    if (Utils.FBM3D(xWorld, yWorld, zWorld, 0.03f, 3, 2) < 0.46f) {
                        type = Block.BlockType.AIR;
                    }
                    else if (yWorld <= Utils.GenerateStoneHeight(xWorld, zWorld)) {
                        type = Block.BlockType.STONE;
                    }
                    else if (yWorld == Utils.GenerateHeight(xWorld, zWorld)) {
                        type = Block.BlockType.GRASS;
                    }
                    else if (yWorld < Utils.GenerateHeight(xWorld, zWorld)) {
                        type = Block.BlockType.DIRT;
                    }
                    else {
                        type = Block.BlockType.AIR;
                    }

                    //type = (Block.BlockType)Random.Range(0, 4);

                    chunckData[x, y, z] = new Block(cubeMaterial, type, blockPos, chunck, this);
                }
            }
        }
    }

    public void DrawChunck() {
        //Draw blocks
        for (int x = 0; x < World.chunckSize; x++){
            for (int y = 0; y < World.chunckSize; y++) {
                for (int z = 0; z < World.chunckSize; z++) {
                    chunckData[x, y, z].Draw();
                }
            }
        }
        CombineQuads();
    }

    private void CombineQuads() {
        MeshFilter[] meshFilters = chunck.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combineInstances = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length)
        {
            combineInstances[i].mesh = meshFilters[i].mesh;
            combineInstances[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }

        MeshFilter meshFilter = chunck.AddComponent(typeof(MeshFilter)) as MeshFilter;
        meshFilter.mesh = new Mesh();
        meshFilter.mesh.name = "CombinedMesh";
        meshFilter.mesh.CombineMeshes(combineInstances);

        MeshRenderer meshRenderer = chunck.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        meshRenderer.material = cubeMaterial;

        foreach (Transform quad in chunck.transform)
        {
            GameObject.Destroy(quad.gameObject);
        }
    }
}
