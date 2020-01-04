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

    World world;

    public void Start()
    {
        world = GameObject.Find("World").GetComponent<World>();        
        for(int i=0;i<9;i++)
        {            
            ItemSlot slot = new ItemSlot(slots[i], world.toolbarLoadData[i]);            
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
