using System.Collections;
using System.Collections.Generic;
using Minecraft.WorldGeneration;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class InventorySlot : MonoBehaviour
{
    public BlockType currentType   = BlockType.AIR;
    public int       currentAmount = 0;

    [Header("COMPONENTS")]
    private Image image = null;
    private Text  quant = null;

    private void Awake() {
        image = GetComponentInChildren<Image>();
        quant = GetComponentInChildren<Text>();
        image.enabled = false;
        quant.enabled = false;
    }

    public void Equip(BlockType _blockType) {
        currentType = _blockType;
        currentAmount++;

        image.enabled = true;
        image.sprite  = DataHolder.instance.GetInventoryData(_blockType).inventorySprite;
        quant.enabled = true;
        quant.text    = currentAmount.ToString();
    }
}
