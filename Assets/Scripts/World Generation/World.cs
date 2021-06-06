using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {
    public GameObject player       = null;
    public Material   textureAtlas = null;

    public static int chunckSize   = 16;
    public static int columnHeight = 16;
    public static int worldSize    = 2;
    public static int radius       = 1;

    public static Dictionary<string, Chunck> chuncks;

    private void Start() {
        player.SetActive(false);
        chuncks = new Dictionary<string, Chunck>();
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        StartCoroutine(BuildWorld());
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

        for (int z = -radius; z <= radius; z++) {
            for (int x = -radius; x <= radius; x++) {
                for (int i = 0; i < columnHeight; i++) {
                    Vector3 chunckPos = new Vector3((x + posX) * chunckSize, i * chunckSize, (z + posZ) * chunckSize);

                    Chunck c = new Chunck(chunckPos, textureAtlas);
                    c.chunck.transform.parent = this.transform;
                    chuncks.Add(c.chunck.name, c);
                }
            }
        }

        foreach (KeyValuePair<string, Chunck> c in chuncks) {
            c.Value.DrawChunck();
            yield return null;
        }
        player.SetActive(true);
    }

    public static string BuildChunckName(Vector3 position) {
        return $"{(int)position.x}_{(int)position.y}_{(int)position.z}";
    }
}