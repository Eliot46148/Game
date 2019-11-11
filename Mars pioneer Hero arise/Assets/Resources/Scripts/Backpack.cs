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

    void Start()
    {
    }

    private void Update()
    {
        // Chose cube
        var d = Input.GetAxis("Mouse ScrollWheel");
        if (d > 0.08f)
        {
            CurrentBlock--;
            if ((int)CurrentBlock < 0)
                CurrentBlock = 0;
        }
        else if (d < -0.15f)
        {
            CurrentBlock++;
            if ((int)CurrentBlock > 8)
                CurrentBlock = 8;
        }
    }
}
