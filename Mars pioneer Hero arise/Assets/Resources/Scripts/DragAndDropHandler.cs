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
            if (CheckForSlot() != null)
            {
                HandleSlotClick(CheckForSlot());
            }
        }
    }

    private void HandleSlotClick(UIItemSlot clickedSlot)
    {
        if (clickedSlot == null)
            return;

        if (!cursorSlot.HasItem && !clickedSlot.HasItem)
            return;

        if (clickedSlot.itemSlot.isCreative)
        {
            cursorItemSlot.EmptySlot();
            cursorItemSlot.InsertStack(clickedSlot.itemSlot.stack);
        }

        if (!cursorSlot.HasItem && clickedSlot.HasItem)
        {
            cursorSlot.itemSlot.InsertStack(clickedSlot.itemSlot.TakeAll());
            return;
        }

        if (cursorSlot.HasItem && !clickedSlot.HasItem)
        {
            clickedSlot.itemSlot.InsertStack(cursorItemSlot.TakeAll());
            return;
        }

        if (cursorSlot.HasItem && clickedSlot.HasItem)
        {
            if(cursorSlot.itemSlot.stack.id != clickedSlot.itemSlot.stack.id)
            {
                ItemStack oldCursorSlot = cursorSlot.itemSlot.TakeAll();
                ItemStack oldSlot = clickedSlot.itemSlot.TakeAll();

                clickedSlot.itemSlot.InsertStack(oldCursorSlot);
                cursorSlot.itemSlot.InsertStack(oldSlot);
            }
        }
    }

    private UIItemSlot CheckForSlot()
    {
        m_PointEventData = new PointerEventData(m_Events);
        m_PointEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        m_Raycaster.Raycast(m_PointEventData, results);

        foreach(RaycastResult result in results)
        {
            if (result.gameObject.tag == "UIItemSlot")
                return result.gameObject.GetComponent<UIItemSlot>();
        }
        return null;
    }
}
