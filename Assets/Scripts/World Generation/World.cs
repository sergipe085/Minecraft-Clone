using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class World : MonoBehaviour {
    public PlayerController player       = null;
    public Material         textureAtlas = null;

    public static int chunckSize   = 16;
    public static int columnHeight = 16;
    public static int worldSize    = 2;
    public static int radius       = 4;
    public static ConcurrentDictionary<string, Chunck> chuncks;
    public static bool firstBuild = true;

    public Vector3 lastBuildPos = Vector3.zero;

    private CoroutineQueue queue = null;
    public static uint maxCoroutines = 500;

    [Header("UI")]
    [SerializeField] private GameObject menuUI    = null;
    [SerializeField] private GameObject loadingUI = null;
    private Slider loadingAmount = null;

    private void Awake() {
        loadingAmount = loadingUI.GetComponentInChildren<Slider>();
    }

    private void Start() {
        Vector3 playerPos = player.transform.position;
        player.transform.position = new Vector3(playerPos.x, Utils.GenerateHeight(playerPos.x, playerPos.z) + 1, playerPos.z);
        lastBuildPos = player.transform.position;
        player.gameObject.SetActive(false);

        firstBuild = true;

        chuncks = new ConcurrentDictionary<string, Chunck>();

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity; 

        menuUI.SetActive(false);
        loadingUI.SetActive(false);

        queue = new CoroutineQueue(maxCoroutines, StartCoroutine);

        //build starting chunk
        BuildChunckAt((int)(player.transform.position.x / chunckSize), (int)(player.transform.position.y / chunckSize), (int)(player.transform.position.z / chunckSize));

        //draw starting chunk
        queue.Run(DrawChunks());

        //build world
        queue.Run(BuildRecursiveWorld((int)(player.transform.position.x / chunckSize), (int)(player.transform.position.y / chunckSize), (int)(player.transform.position.z / chunckSize), radius));
    }

    private void Update() {
        if (Vector3.Distance(player.transform.position, lastBuildPos) > chunckSize) {
            BuildNearPlayer();
            lastBuildPos = player.transform.position;
        }

        if (!player.gameObject.activeSelf) {
            player.gameObject.SetActive(true);
            firstBuild = false;
        }

        queue.Run(DrawChunks());
    }

    void BuildNearPlayer() {
        StopCoroutine(nameof(BuildRecursiveWorld));
        queue.Run(BuildRecursiveWorld((int)(player.transform.position.x / chunckSize), (int)(player.transform.position.y / chunckSize), (int)(player.transform.position.z / chunckSize), radius));
    }

    void BuildChunckAt(int x, int y, int z) {
        Vector3 chunckPos = new Vector3(x, y, z) * chunckSize;
        string chunckName = BuildChunckName(chunckPos);

        Chunck c;
        if (!chuncks.TryGetValue(chunckName, out c)) {
            c = new Chunck(chunckPos, textureAtlas);
            c.chunck.transform.parent = transform;
            chuncks.TryAdd(chunckName, c);
        }
    }

    IEnumerator BuildRecursiveWorld(int x, int y, int z, int rad) {
        rad--;

        if (rad <= 0) {
            yield break;
        }

        //build chunk front
        BuildChunckAt(x, y, z + 1);
        queue.Run(BuildRecursiveWorld(x, y, z + 1, rad));
        yield return null;

        //build chunk back
        BuildChunckAt(x, y, z - 1);
        queue.Run(BuildRecursiveWorld(x, y, z - 1, rad));
        yield return null;

        //build chunk up
        BuildChunckAt(x, y + 1, z);
        queue.Run(BuildRecursiveWorld(x, y + 1, z, rad));
        yield return null;

        //build chunk down
        BuildChunckAt(x, y - 1, z);
        queue.Run(BuildRecursiveWorld(x, y - 1, z, rad));
        yield return null;

        //build chunk right
        BuildChunckAt(x + 1, y, z);
        queue.Run(BuildRecursiveWorld(x + 1, y, z, rad));
        yield return null;

        //build chunk left
        BuildChunckAt(x - 1, y, z);
        queue.Run(BuildRecursiveWorld(x - 1, y, z, rad));
        yield return null;
    }

    IEnumerator DrawChunks() {
        foreach(KeyValuePair<string, Chunck> c in chuncks) {
            if (c.Value.status == Chunck.ChunckStatus.DRAW) {
                c.Value.DrawChunck();
            }
            yield return null;
        }
    }

    public static string BuildChunckName(Vector3 position) {
        return $"{(int)position.x}_{(int)position.y}_{(int)position.z}";
    }
}