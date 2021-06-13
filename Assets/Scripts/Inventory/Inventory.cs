using System.Collections;
using System.Collections.Generic;
using Minecraft.WorldGeneration;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private InventorySlot[] slots;
    [SerializeField] public  GameObject      canvas        = null;

    public InventoryHolder currentHolding = null;
    public InventoryHolder currentTarget  = null;

    private void Start() {
        slots = GetComponentsInChildren<InventorySlot>();

        canvas.SetActive(false);
    }

    public bool Equip(BlockType _blockType) {
        foreach(InventorySlot slot in slots) {
            if ((slot.currentType == _blockType || slot.currentType == BlockType.AIR) && slot.currentAmount < 64)
            {
                slot.Equip(_blockType, true);
                //slotsPosition[i].UpdateSlot(_blockType);
                return true;
            }
        }
        return false;
    }
}
