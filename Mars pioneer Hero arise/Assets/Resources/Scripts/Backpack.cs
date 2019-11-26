using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backpack : MonoBehaviour {

    public Basic basic;

    private int currentBlock = 0;
    public List<BlockType> itemsBar;
    public List<Item> hotkeyItems;
    public List<Item> backpackItems;

    public List<BlockType> ItemsBar
    {
        get
        {
            return itemsBar;
        }

        set
        {
            itemsBar = value;
        }
    }

    public int CurrentBlock
    {
        get
        {
            return currentBlock;
        }

        set
        {
            currentBlock = value;
        }
    }

    public BlockType CurrentBlockType
    {
        get
        {
            return itemsBar[currentBlock];
        }

        set
        {
            itemsBar[currentBlock] = value;
        }
    }

    public List<Item> HotkeyItems
    {
        get
        {
            return hotkeyItems;
        }

        set
        {
            hotkeyItems = value;
        }
    }

    public List<Item> BackpackItems
    {
        get
        {
            return backpackItems;
        }

        set
        {
            backpackItems = value;
        }
    }
}
