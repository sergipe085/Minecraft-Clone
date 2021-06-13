using System.Collections;
using System.Collections.Generic;
using Minecraft.WorldGeneration;
using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public BlockType currentType   = BlockType.AIR;
    public int       currentAmount = 0;

    public void Equip(BlockType _blockType) {
        currentType = _blockType;
        currentAmount++;
    }
}
