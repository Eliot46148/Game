using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryToolbar : MonoBehaviour {

    public GameObject slotPrefab;
    World world;
    public Basic basic;
    public UIItemSlot[] slots;

    private void Start()
    {
        world = GameObject.Find("World").GetComponent<World>();

        for(int i = 0; i < 9; i++)
        {
            GameObject newslot = Instantiate(slotPrefab, transform);
            ItemSlot slot = new ItemSlot(newslot.GetComponent<UIItemSlot>(), slots[i].itemSlot.stack);
        }

    }
}
