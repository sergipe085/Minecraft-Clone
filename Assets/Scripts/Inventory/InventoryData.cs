using Minecraft.WorldGeneration;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryData", menuName = "Minecraft Clone/InventoryData", order = 0)]
public class InventoryData : ScriptableObject {
    public BlockType blockType;
    public Sprite inventorySprite = null;
}