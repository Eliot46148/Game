using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    public GameObject slotPrefab;
    World world;
    public Basic basic;
    List<ItemSlot> slots = new List<ItemSlot>();

    private void Start()
    {
        world = GameObject.Find("World").GetComponent<World>();

        for (int i = 1; i < world.blocks.Count-1; i++)
        {
            GameObject newslot = Instantiate(slotPrefab, transform);

            ItemStack stack = new ItemStack(2, 10);
            ItemSlot slot = new ItemSlot(newslot.GetComponent<UIItemSlot>(), stack);
            //slot.isCreative = true;
        }
    }
}
