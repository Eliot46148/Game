using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    public GameObject slotPrefab;
    World world;
    public Basic basic;
    public List<ItemSlot> slots = new List<ItemSlot>();

    private void Start()
    {
        world = GameObject.Find("World").GetComponent<World>();

        for (int i = 0; i < 27; i++)
        {
            GameObject newslot = Instantiate(slotPrefab, transform);

            ItemStack stack = new ItemStack(2, 10);
            slots.Add(new ItemSlot(newslot.GetComponent<UIItemSlot>(), stack));
            //slot.isCreative = true;
        }
    }
}
