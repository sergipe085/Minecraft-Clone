using System.Collections;
using System.Collections.Generic;
using Minecraft.WorldGeneration;
using UnityEngine;

public class DataHolder : MonoBehaviour
{
    public static DataHolder instance;

    private void Awake() {
        instance = this;
    }

    public InventoryData[] inventoryDatas;
    public InventoryData GetInventoryData(BlockType _blockType) {
        foreach(InventoryData inventoryData in inventoryDatas) {
            if (inventoryData.blockType == _blockType) {
                return inventoryData;
            }
        }
        return null;
    }
}
