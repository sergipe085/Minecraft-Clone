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
    [SerializeField] private Image image = null;
    [SerializeField] private Text  quant = null;

    private void Awake() {
        image.enabled = false;
        quant.enabled = false;
    }

    public void Equip(BlockType _blockType, bool increment) {
        currentType = _blockType;
        currentAmount += increment ? 1 : 0;

        image.enabled = true;
        image.sprite  = DataHolder.instance.GetInventoryData(_blockType).inventorySprite;
        quant.enabled = true;
        quant.text    = currentAmount.ToString();
    }

    public void UpdateAmount(int amount) {
        currentAmount = amount;
        quant.text = currentAmount.ToString();
    }
}
