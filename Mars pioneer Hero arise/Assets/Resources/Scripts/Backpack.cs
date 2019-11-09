using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backpack : MonoBehaviour {

    public Basic basic;

    private int currentBlock = 0;
    public List<BlockType> itemsBar;

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

    void Start()
    {
    }
}
