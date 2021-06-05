using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunck : MonoBehaviour
{
    [SerializeField] private Material cubeMaterial = null;
    public Block[,,] chunckData;

    private void Start() {
        StartCoroutine(CreateChunck(8, 8, 8));
    }

    private IEnumerator CreateChunck(int xSize, int ySize, int zSize) {
        chunckData = new Block[xSize, ySize, zSize];

        //Create blocks
        for (int x = 0; x < xSize; x++) {   
            for (int y = 0; y < ySize; y++) {
                for (int z = 0; z < zSize; z++) {
                    Vector3 blockPos = new Vector3(x, y, z);
                    Block.BlockType type = (Block.BlockType)Random.Range(0, 5);

                    chunckData[x, y, z] = new Block(cubeMaterial, type, blockPos, this);
                }
            }
        }

        //Draw blocks
        for (int x = 0; x < xSize; x++) {
            for (int y = 0; y < ySize; y++) {
                for (int z = 0; z < zSize; z++) {
                    chunckData[x, y, z].Draw();
                }
            }
        }
        CombineQuads();
        yield return null;
    }

    private void CombineQuads() {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combineInstances = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length)
        {
            combineInstances[i].mesh = meshFilters[i].mesh;
            combineInstances[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }

        MeshFilter meshFilter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        meshFilter.mesh = new Mesh();
        meshFilter.mesh.name = "CombinedMesh";
        meshFilter.mesh.CombineMeshes(combineInstances);

        MeshRenderer meshRenderer = gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        meshRenderer.material = cubeMaterial;

        foreach (Transform quad in transform)
        {
            Destroy(quad.gameObject);
        }
    }
}
