﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemSlot : MonoBehaviour {

    public Basic basic;

    public bool isLinked = false;
    public ItemSlot itemSlot;
    public Image slotImage;
    public Image slotIcon;
    public Text slotAmount;

    World world;
    

    private void Awake()
    {
        world = GameObject.Find("World").GetComponent<World>();
    }

    public bool HasItem
    {
        get
        {
            if (itemSlot == null)
                return false;
            else
                return itemSlot.HasItem;
        }
    }

    public void Link(ItemSlot _itemSlot)
    {
        itemSlot = _itemSlot;
        isLinked = true;
        itemSlot.LinkUISlot(this);
        UpdateSlot();
    }

    public void UnLink()
    {
        itemSlot.UnLinkUISlot();
        itemSlot = null;
        isLinked = false;
        UpdateSlot();
    }

    public void UpdateSlot()
    {
        if(itemSlot != null && itemSlot.HasItem)
        {
            try
            {
                slotIcon.sprite = world.blocks[itemSlot.stack.id].iconImage;
            }
            catch(Exception e)
            {
                Debug.Log(itemSlot.stack.id);
                Debug.Log(e);
            }
            slotAmount.text = itemSlot.stack.amount.ToString();
            if (itemSlot.stack.amount < 0)
                slotAmount.text = "";
            slotIcon.enabled = true;
            slotAmount.enabled = true;
        }
        else
        {
            Clear();
        }
    }


    public void Clear()
    {
        slotIcon.sprite = null;
        slotAmount.text = "";
        slotIcon.enabled = false;
        slotAmount.enabled = false;
    }

    /*private void OnDestroy()
    {
        if (isLinked)        
            itemSlot.UnLinkUISlot();        
    }*/

}

public class ItemSlot
{
    public ItemStack stack = null;
    private UIItemSlot uiItemSlot = null;

    public bool isCreative = false;

    public ItemSlot(UIItemSlot _uIItemSlot)
    {
        stack = null;
        uiItemSlot = _uIItemSlot;
        uiItemSlot.Link(this);
    }

    public ItemSlot(UIItemSlot _uIItemSlot, ItemStack _stack)
    {
        stack = _stack;
        uiItemSlot = _uIItemSlot;
        uiItemSlot.Link(this);
    }

    public ItemSlot(UIItemSlot _uIItemSlot, SaveItem _item)
    {
        if(_item.amount < 0)
        {
            stack = new ItemStack(_item.id, -1);
            isCreative = true;
        }
        else if (_item.amount > 0)
        {
            stack = new ItemStack(_item.id, _item.amount);
        }
        uiItemSlot = _uIItemSlot;
        uiItemSlot.Link(this);
    }

    public void LinkUISlot(UIItemSlot uiSlot)
    {
        uiItemSlot = uiSlot;
    }

    public void UnLinkUISlot()
    {
        uiItemSlot = null;
    }

    public void EmptySlot()
    {
        stack = null;
        isCreative = false;
        if (uiItemSlot != null)
            uiItemSlot.UpdateSlot();
    }

    public int Take(int amt)
    {
        if (amt > stack.amount)
        {
            int temp = stack.amount;
            EmptySlot();
            return temp;        
        }
        else if(amt < stack.amount)
        {
            stack.amount -= amt;
            uiItemSlot.UpdateSlot();
            return amt;
        }
        else
        {
            EmptySlot();
            return amt;
        }
    }

    public void add(int amt)
    {
        stack.amount += amt;
        if (stack.amount <= 0)
            EmptySlot();
        else
            uiItemSlot.UpdateSlot();
    }

    public ItemStack TakeAll()
    {
        ItemStack handover = new ItemStack(stack.id, stack.amount);
        EmptySlot();
        return handover;
    }

    public ItemStack GetStack()
    {
        ItemStack handover = new ItemStack(stack.id, stack.amount);
        return handover;
    }

    public void InsertStack(ItemStack _stack)
    {
        if (_stack.amount < 0)
        {
            isCreative = true;
            stack = _stack;
            uiItemSlot.UpdateSlot();
        }
        else if (_stack.amount > 0)
        {
            stack = _stack;
            uiItemSlot.UpdateSlot();
        }
        else
            EmptySlot();
    }

    public void InsertStack(SaveItem _item)
    {
        if (_item.amount > 0)
        {
            stack = new ItemStack(_item.id, _item.amount);
            uiItemSlot.UpdateSlot();
        }
        else
            EmptySlot();
    }

    public bool HasItem
    {
        get
        {
            if (stack != null)
                return true;
            else
                return false;
        }
    }

}
