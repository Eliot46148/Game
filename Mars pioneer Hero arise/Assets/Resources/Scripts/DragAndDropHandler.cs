using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDropHandler : MonoBehaviour {

    [SerializeField] UIItemSlot cursorSlot = null;
    private ItemSlot cursorItemSlot;

    [SerializeField] GraphicRaycaster m_Raycaster = null;
    private PointerEventData m_PointEventData;

    [SerializeField] EventSystem m_Events = null;

    World world;
    public Basic basic;

    public UIItemSlot[] handCraftSlots;

    private void Start()
    {
        world = GameObject.Find("World").GetComponent<World>();

        cursorItemSlot = new ItemSlot(cursorSlot);
    }

    private void Update()
    {
        if (world.UIState == 1)
            return;

        cursorSlot.transform.position = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            CheckForSlot();
        }

        if (Input.GetMouseButtonDown(1))
        {
            CheckForSlot(0);
        }
    }

    private void HandleSlotClick(UIItemSlot clickedSlot, int amt = -1)
    {
        if (clickedSlot == null)
            return;

        if (!cursorSlot.HasItem && !clickedSlot.HasItem)
            return;

        /*if (clickedSlot.itemSlot.isCreative)
        {
            cursorItemSlot.EmptySlot();
            cursorItemSlot.InsertStack(clickedSlot.itemSlot.stack);
        }*/

        if (!cursorSlot.HasItem && clickedSlot.HasItem)
        {   
            if(amt==-1)
                cursorSlot.itemSlot.InsertStack(clickedSlot.itemSlot.TakeAll());
            else
                cursorSlot.itemSlot.InsertStack(new ItemStack(clickedSlot.itemSlot.stack.id, clickedSlot.itemSlot.Take((int)clickedSlot.itemSlot.stack.amount/2)));
            return;
        }

        if (cursorSlot.HasItem && !clickedSlot.HasItem)
        {
            if (amt == -1)
                clickedSlot.itemSlot.InsertStack(cursorItemSlot.TakeAll());
            else                     
                clickedSlot.itemSlot.InsertStack(new ItemStack(cursorItemSlot.stack.id,cursorItemSlot.Take(1)));                                            
            return;
        }

        if (cursorSlot.HasItem && clickedSlot.HasItem)
        {
            if(cursorSlot.itemSlot.stack.id != clickedSlot.itemSlot.stack.id && amt==-1)
            {
                ItemStack oldCursorSlot = cursorSlot.itemSlot.TakeAll();
                ItemStack oldSlot = clickedSlot.itemSlot.TakeAll();

                clickedSlot.itemSlot.InsertStack(oldCursorSlot);
                cursorSlot.itemSlot.InsertStack(oldSlot);
            }

            if (cursorSlot.itemSlot.stack.id == clickedSlot.itemSlot.stack.id)
            {
                if (amt == -1)
                {
                    int num = 64 - clickedSlot.itemSlot.stack.amount;
                    if (num > 0)
                    {
                        clickedSlot.itemSlot.add(cursorSlot.itemSlot.Take(num));
                    }
                }
                else                
                    clickedSlot.itemSlot.add(cursorSlot.itemSlot.Take(1));       
            }
        }
    }

    private void HandleHandCraftResultSlotClick(UIItemSlot clickedSlot)
    {
        if (clickedSlot == null)
            return;

        if (cursorSlot.HasItem)
        {
            if (cursorSlot.itemSlot.stack.id != clickedSlot.itemSlot.stack.id)
                return;
            cursorSlot.itemSlot.add(1);
            clickedSlot.itemSlot.EmptySlot();
            ReduceHandCraftSlots();
        }
        else
        {
            cursorSlot.itemSlot.InsertStack(clickedSlot.itemSlot.TakeAll());
            ReduceHandCraftSlots();
        }   
                
    }

    private void ReduceHandCraftSlots()
    {
        foreach(UIItemSlot s in handCraftSlots)
        {
            if (s.itemSlot.HasItem)
                s.itemSlot.add(-1);
        }
    }

    private void CheckForSlot(int amt = -1)
    {
        m_PointEventData = new PointerEventData(m_Events);
        m_PointEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        m_Raycaster.Raycast(m_PointEventData, results);

        foreach(RaycastResult result in results)
        {            
            if (result.gameObject.tag == "HandCraftResultSlot")
            {
                HandleHandCraftResultSlotClick(result.gameObject.GetComponent<UIItemSlot>());
                return;
            }

            if (result.gameObject.tag == "UIItemSlot")
            {
                HandleSlotClick(result.gameObject.GetComponent<UIItemSlot>(), amt);
                return;
            }
        }
        return;
    }
}
