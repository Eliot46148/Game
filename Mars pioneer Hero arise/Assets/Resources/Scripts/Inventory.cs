using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    public GameObject slotPrefab;
    World world;
    public Basic basic;
    public List<ItemSlot> bag = new List<ItemSlot>();
    public UIItemSlot[] equiments = new UIItemSlot[5];
    public UIItemSlot[] craftslots = new UIItemSlot[5];

    private void Start()
    {
        world = GameObject.Find("World").GetComponent<World>();

        for (int i = 0; i < 27; i++)
        {
            GameObject newslot = Instantiate(slotPrefab, transform);

            ItemStack stack = new ItemStack(2, 10);
            bag.Add(new ItemSlot(newslot.GetComponent<UIItemSlot>(), stack));       
        }

        foreach(UIItemSlot s in equiments)
        {           
            ItemSlot slot = new ItemSlot(s);
        }

        foreach (UIItemSlot s in craftslots)
        {
            ItemSlot slot = new ItemSlot(s);
        }

    }
}
