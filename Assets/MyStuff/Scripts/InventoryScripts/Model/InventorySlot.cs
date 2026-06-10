using Inventory.Model;
using System;
using UnityEngine;

[Serializable]
public class InventorySlot
{
    public ItemSO item;
    public int quantity;

    public bool isEmpty
    {
        get { return item == null; }
    }

    public InventorySlot() { }

    public InventorySlot(ItemSO item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }
}
