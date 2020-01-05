using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    public GameObject slotPrefab;
    World world;
    public Basic basic;
    public UIItemSlot[] bag = new UIItemSlot[27];
    public UIItemSlot[] equiments = new UIItemSlot[5];
    public UIItemSlot[] craftslots = new UIItemSlot[5];


    public void Awake()
    {        
        world = GameObject.Find("World").GetComponent<World>();

        for (int i = 0; i < 27; i++)
        {
            GameObject newslot = Instantiate(slotPrefab, transform);

            ItemSlot itemSlot = new ItemSlot(newslot.GetComponent<UIItemSlot>(), world.bagLoadData[i]);
            bag[i] = newslot.GetComponent<UIItemSlot>();
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
