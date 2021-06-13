using System.Collections;
using System.Collections.Generic;
using Minecraft.WorldGeneration;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private InventorySlot[] slots;
    [SerializeField] private SlotLocation[]  slotsPosition = null;
    [SerializeField] private GameObject      canvas        = null;

    private void Start() {
        slotsPosition = GetComponentsInChildren<SlotLocation>();
        slots = new InventorySlot[slotsPosition.Length];

        canvas.SetActive(false);
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
