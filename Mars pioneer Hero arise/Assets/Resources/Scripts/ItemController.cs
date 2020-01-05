using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour {

    public GameObject Toolbar;
    public GameObject Inventory;

    public UIItemSlot[] toolbar;
    public UIItemSlot[] bag;
    public UIItemSlot[] equiments;
    public UIItemSlot[] craftslots;    

    public void Start()
    {
        toolbar = Toolbar.GetComponent<Toolbar>().slots;
        bag = Inventory.GetComponent<Inventory>().bag;
        equiments = Inventory.GetComponent<Inventory>().equiments;
        craftslots = Inventory.GetComponent<Inventory>().craftslots;
    }

    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            PickItem(new ItemStack(5, 1));
        }
    }*/

    // 撿取物品
    public bool PickItem(ItemStack stack)
    {
        if (InsertStackIntoSlots(toolbar, stack))
            return true;
        else
            return InsertStackIntoSlots(bag, stack);        
    }

    // 在一個UIItemSlot陣列裡插入stack
    private bool InsertStackIntoSlots(UIItemSlot[] slots, ItemStack stack)
    {
        UIItemSlot emptySlot = null;
        foreach (UIItemSlot s in slots)
        {
            
                if (s.HasItem)
                {
                    if (s.itemSlot.stack.amount > 0)
                    {
                        if (s.itemSlot.stack.id == stack.id && 64 - s.itemSlot.stack.amount >= stack.amount)
                        {
                            s.itemSlot.add(stack.amount);
                            return true;
                        }
                    }
                }
                else if (emptySlot == null)
                {
                    emptySlot = s;
                }            
        }

        if (emptySlot != null)
        {
            emptySlot.itemSlot.InsertStack(stack);
            return true;
        }        
        return false;
    }

}
