using System.Collections;
using System.Collections.Generic;
using Minecraft.WorldGeneration;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int             slotsAmount = 12;
    [SerializeField] private InventorySlot[] slots;

    private void Start() {
        slots = new InventorySlot[12];
    }

    public bool Equip(BlockType _blockType) {
        foreach(InventorySlot slot in slots) {
            if ((slot.currentType == _blockType || slot.currentType == BlockType.AIR) && slot.currentAmount < 64)
            {
                slot.Equip(_blockType);
                return true;
            }
        }
        return false;
    }
}
