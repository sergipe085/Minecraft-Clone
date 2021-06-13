using System.Collections;
using System.Collections.Generic;
using Minecraft.WorldGeneration;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryHolder : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler
{
    private Inventory inventory;
    private InventorySlot mySlot = null;

    public BlockType type;
    public int quant = 0;

    private Vector2 initialPosition;

    private void Awake() {
        inventory = GetComponentInParent<Inventory>();
        mySlot    = GetComponentInParent<InventorySlot>();
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (inventory.currentHolding != null) return;

        inventory.currentHolding = this;

        InventorySlot slot = GetComponentInParent<InventorySlot>();
        type = slot.currentType;
        quant = slot.currentAmount;
        initialPosition = transform.position;
    }

    public void OnDrag(PointerEventData eventData) {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData) {
        transform.position = initialPosition;
        inventory.currentHolding = null;

        if (inventory.currentTarget != null) {
            inventory.currentTarget.GetBlocks(mySlot);
            inventory.currentTarget = null;
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        print("entro");
        InventoryHolder otherInventoryHolder = eventData.pointerEnter.GetComponent<InventoryHolder>() ?? eventData.pointerEnter.GetComponentInParent<InventoryHolder>();
        if (otherInventoryHolder != null && inventory.currentHolding != null) {
            inventory.currentTarget = otherInventoryHolder;
        }
    }

    public void GetBlocks(InventorySlot slot) {
        print(transform.name + "PEGUEI");

        BlockType tempType = mySlot.currentType;
        int       tempAmount = mySlot.currentAmount;

        mySlot.UpdateAmount(slot.currentAmount);
        mySlot.Equip(slot.currentType, false);

        slot.UpdateAmount(tempAmount);
        slot.Equip(tempType, false);
    }
}
