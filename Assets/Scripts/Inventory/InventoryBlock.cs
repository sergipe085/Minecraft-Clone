using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Minecraft.WorldGeneration;

[System.Serializable]
public class InventoryBlock : InventoryItem
{
    public BlockType type;

    public InventoryBlock(BlockType _type) {
        type = _type;
    }

    public InventoryBlock(Block _block) {
        type = _block.type;
    }
}
