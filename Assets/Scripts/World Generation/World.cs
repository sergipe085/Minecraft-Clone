using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class World : MonoBehaviour {
    public PlayerController player       = null;
    public Material         textureAtlas = null;

    public static int chunckSize   = 16;
    public static int columnHeight = 16;
    public static int worldSize    = 2;
    public static int radius       = 1;

    [Header("UI")]
    [SerializeField] private GameObject menuUI    = null;
    [SerializeField] private GameObject loadingUI = null;
    private Slider loadingAmount = null;

    public static Dictionary<string, Chunck> chuncks;

    private void Awake() {
        loadingAmount = loadingUI.GetComponentInChildren<Slider>();
    }

    private void Start() {
        player.transform.position = new Vector3(Random.Range(-1000f, 1000f), transform.position.y, Random.Range(-1000f, 1000f));
        player.gameObject.SetActive(false);
        chuncks = new Dictionary<string, Chunck>();
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity; 
        menuUI.SetActive(true);
        loadingUI.SetActive(false);
    }

    public void StartBuild() {
        StartCoroutine(BuildWorld());
        menuUI.SetActive(false);
        loadingUI.SetActive(true);
    }

    private IEnumerator BuildChunckColumn() {
        for (int i = 0; i < columnHeight; i++) {
            Vector3 chunckPos = new Vector3(transform.position.x, i * chunckSize, transform.position.z);

            Chunck c = new Chunck(chunckPos, textureAtlas);
            c.chunck.transform.parent = this.transform;
            chuncks.Add(c.chunck.name, c);
        }

        foreach (KeyValuePair<string, Chunck> c in chuncks) {
            c.Value.DrawChunck();
            yield return null;
        }
    }

    private IEnumerator BuildWorld() {
        int posX = (int)Mathf.Floor(player.transform.position.x / chunckSize);
        int posZ = (int)Mathf.Floor(player.transform.position.z / chunckSize);

        float totalChunks = (Mathf.Pow(radius * 2 + 1, 2) * columnHeight) * 2;
        int processCount = 0;

        for (int z = -radius; z <= radius; z++) {
            for (int x = -radius; x <= radius; x++) {
                for (int i = 0; i < columnHeight; i++) {
                    Vector3 chunckPos = new Vector3((x + posX) * chunckSize, i * chunckSize, (z + posZ) * chunckSize);

                    Chunck c = new Chunck(chunckPos, textureAtlas);
                    c.chunck.transform.parent = this.transform;
                    chuncks.Add(c.chunck.name, c);
                    processCount++;
                    loadingAmount.value = processCount / totalChunks * 100;
                    yield return null;
                }
            }
        }

        foreach (KeyValuePair<string, Chunck> c in chuncks) {
            c.Value.DrawChunck();
            processCount++;
            loadingAmount.value = processCount / totalChunks * 100;
            yield return null;
        }
        player.gameObject.SetActive(true);
        player.OnCreated();
        loadingUI.SetActive(false);
    }

    public static string BuildChunckName(Vector3 position) {
        return $"{(int)position.x}_{(int)position.y}_{(int)position.z}";
    }
}