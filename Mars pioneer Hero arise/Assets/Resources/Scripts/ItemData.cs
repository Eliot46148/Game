using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : MonoBehaviour {

    public GameObject Toolbar;
    public GameObject Inventory;

    public ItemStack[] GetItemData()
    {
        int index=0;
        ItemStack[] stacks = new ItemStack[36];
        UIItemSlot[] ToolbarSlots = Toolbar.GetComponent<Toolbar>().slots;
        List<ItemSlot> InventorySlots = Inventory.GetComponent<Inventory>().bag;
        foreach (UIItemSlot s in ToolbarSlots)
        {
            stacks[index++] = s.itemSlot.GetStack();
        }
        foreach (ItemSlot s in InventorySlots)
        {
            stacks[index++] = s.GetStack();
        }
        return stacks;
    }
}
