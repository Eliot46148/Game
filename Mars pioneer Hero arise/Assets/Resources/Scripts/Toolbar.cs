using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Toolbar : MonoBehaviour
{
    public UIItemSlot[] slots;
    public RectTransform highlight;
    public Control control;
    public int slotindex = 0;

    public void Start()
    {
        byte index = 10;
        foreach(UIItemSlot s in slots)
        {
            ItemStack stack = new ItemStack(index, (byte)Random.Range(2,65));
            ItemSlot slot = new ItemSlot(s, stack);
            index++;
        }                
    }

    private void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            if (scroll > 0)
                slotindex--;
            else
                slotindex++;

            if (slotindex > slots.Length - 1)
                slotindex = 0;
            if (slotindex < 0)
                slotindex = slots.Length - 1;

            highlight.position = slots[slotindex].slotIcon.transform.position;            
        }
    }
}
