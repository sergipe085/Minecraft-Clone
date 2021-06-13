using System.Collections;
using System.Collections.Generic;
using Minecraft.WorldGeneration;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public GameObject meshObject = null;

    private BlockType blockType = BlockType.AIR;
    
    private List<Vector3> vertices  = new List<Vector3>();
    private List<Vector2> uvs       = new List<Vector2>();
    private List<int>     triangles = new List<int>();

    private Rigidbody rig = null;

    private float initialY = 0.0f;
    private float yTime    = 0.0f;
    private bool onGround  = false;
    public  bool isPicking = false;

    private void Awake() {
        rig = GetComponent<Rigidbody>();
    }

    private void CreateBlock() {
        AddBlockFaceUp(Vector3Int.zero, BlockData.uvs[blockType][TileSide.TOP]);
        AddBlockFaceBottom(Vector3Int.zero, BlockData.uvs[blockType][TileSide.BOTTOM]);
        AddBlockFaceRight(Vector3Int.zero, BlockData.uvs[blockType][TileSide.SIDE]);
        AddBlockFaceLeft(Vector3Int.zero, BlockData.uvs[blockType][TileSide.SIDE]);
        AddBlockFaceFront(Vector3Int.zero, BlockData.uvs[blockType][TileSide.SIDE]);
        AddBlockFaceBack(Vector3Int.zero, BlockData.uvs[blockType][TileSide.SIDE]);
    }

    public void Setup(BlockType _blockType) {
        blockType = _blockType;

        CreateBlock();

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        MeshFilter meshFilter = meshObject.GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        meshObject.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);

        rig.AddForce(Vector3.up * 5 + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)) * 0.5f, ForceMode.Impulse);
    }

    private void Update() {
        if (isPicking) return;

        if (Physics.Raycast(transform.position, Vector3.down, 0.4f) && rig.isKinematic == false) {
            Land();
        } else if (!Physics.Raycast(transform.position, Vector3.down, 0.4f)) {
            rig.isKinematic = false;
            rig.useGravity = true;
            onGround = false;
        }

        if (onGround) {
            yTime += Time.deltaTime;
            meshObject.transform.position = new Vector3(meshObject.transform.position.x, initialY + Mathf.Sin(yTime * 1.5f) / 7 + 0.2f, meshObject.transform.position.z);
        }
    }

    private void Land() {
        rig.isKinematic = true;
        rig.useGravity = false;
        rig.velocity = Vector3.zero;

        yTime    = 0.0f;
        onGround = true;
        initialY = meshObject.transform.position.y;
    }

    private void AddBlockFaceUp(Vector3Int pos, Vector2[] blockUVs)
    {
        AddFacesForNextFourVertices();

        vertices.Add(new Vector3(pos.x, pos.y, pos.z) + new Vector3(-0.5f, 0.5f, -0.5f));
        vertices.Add(new Vector3(pos.x, pos.y, pos.z) + new Vector3(-0.5f, 0.5f, 0.5f));
        vertices.Add(new Vector3(pos.x, pos.y, pos.z) + new Vector3(0.5f, 0.5f, 0.5f));
        vertices.Add(new Vector3(pos.x, pos.y, pos.z) + new Vector3(0.5f, 0.5f, -0.5f));

        uvs.AddRange(blockUVs);
    }
    private void AddBlockFaceBottom(Vector3Int pos, Vector2[] blockUVs)
    {
        AddFacesForNextFourVertices();

        vertices.Add(new Vector3(pos.x, pos.y, pos.z) + new Vector3(-0.5f, -0.5f, -0.5f));
        vertices.Add(new Vector3(pos.x, pos.y, pos.z) + new Vector3(0.5f, -0.5f, -0.5f));
        vertices.Add(new Vector3(pos.x, pos.y, pos.z) + new Vector3(0.5f, -0.5f, 0.5f));
        vertices.Add(new Vector3(pos.x, pos.y, pos.z) + new Vector3(-0.5f, -0.5f, 0.5f));

        uvs.AddRange(blockUVs);
    }
    private void AddBlockFaceFront(Vector3Int pos, Vector2[] blockUVs)
    {
        AddFacesForNextFourVertices();

        vertices.Add(new Vector3(pos.x, pos.y, pos.z) + new Vector3(0.5f, -0.5f, 0.5f));
        vertices.Add(new Vector3(pos.x, pos.y, pos.z) + new Vector3(0.5f, 0.5f, 0.5f));
        vertices.Add(new Vector3(pos.x, pos.y, pos.z) + new Vector3(-0.5f, 0.5f, 0.5f));
        vertices.Add(new Vector3(pos.x, pos.y, pos.z) + new Vector3(-0.5f, -0.5f, 0.5f));

        uvs.AddRange(blockUVs);
    }
    private void AddBlockFaceBack(Vector3Int pos, Vector2[] blockUVs)
    {
        AddFacesForNextFourVertices();

        vertices.Add(new Vector3(pos.x, pos.y, pos.z) + new Vector3(-0.5f, -0.5f, -0.5f));
        vertices.Add(new Vector3(pos.x, pos.y, pos.z) + new Vector3(-0.5f, 0.5f, -0.5f));
        vertices.Add(new Vector3(pos.x, pos.y, pos.z) + new Vector3(0.5f, 0.5f, -0.5f));
        vertices.Add(new Vector3(pos.x, pos.y, pos.z) + new Vector3(0.5f, -0.5f, -0.5f));

        uvs.AddRange(blockUVs);
    }
    private void AddBlockFaceRight(Vector3Int pos, Vector2[] blockUVs)
    {
        AddFacesForNextFourVertices();

        vertices.Add(new Vector3(pos.x, pos.y, pos.z) + new Vector3(0.5f, -0.5f, -0.5f));
        vertices.Add(new Vector3(pos.x, pos.y, pos.z) + new Vector3(0.5f, 0.5f, -0.5f));
        vertices.Add(new Vector3(pos.x, pos.y, pos.z) + new Vector3(0.5f, 0.5f, 0.5f));
        vertices.Add(new Vector3(pos.x, pos.y, pos.z) + new Vector3(0.5f, -0.5f, 0.5f));

        uvs.AddRange(blockUVs);
    }
    private void AddBlockFaceLeft(Vector3Int pos, Vector2[] blockUVs)
    {
        AddFacesForNextFourVertices();

        vertices.Add(new Vector3(pos.x, pos.y, pos.z) + new Vector3(-0.5f, -0.5f, 0.5f));
        vertices.Add(new Vector3(pos.x, pos.y, pos.z) + new Vector3(-0.5f, 0.5f, 0.5f));
        vertices.Add(new Vector3(pos.x, pos.y, pos.z) + new Vector3(-0.5f, 0.5f, -0.5f));
        vertices.Add(new Vector3(pos.x, pos.y, pos.z) + new Vector3(-0.5f, -0.5f, -0.5f));

        uvs.AddRange(blockUVs);
    }
    private void AddFacesForNextFourVertices()
    {
        triangles.Add(vertices.Count);
        triangles.Add(vertices.Count + 1);
        triangles.Add(vertices.Count + 2);
        triangles.Add(vertices.Count);
        triangles.Add(vertices.Count + 2);
        triangles.Add(vertices.Count + 3);
    }
}
