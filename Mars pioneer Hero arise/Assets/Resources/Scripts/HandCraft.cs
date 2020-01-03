using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCraft : MonoBehaviour
{
    public GameObject InventoryPanel;
    
    public UIItemSlot[] craftSlots;
    public UIItemSlot resultSlot;

    public int[] craftIdArray;

    public List<HandCraftRecipe> recipes = new List<HandCraftRecipe>();

    public void Start()
    {
        InitializeRecipes();
    }

    public void Update()
    {
        if (InventoryPanel.activeSelf)
        {            
            UpdateCraftIdArray();
            foreach (HandCraftRecipe r in recipes)
            {
                bool isEqual = true;
                for (int i = 0; i < 9; i++)
                {
                    if (craftIdArray[i] != r.recipe[i])
                    {                                            
                        isEqual = false;
                    }
                }

                if (isEqual)
                {
                    ShowCraftResult(r.result);
                    break;
                }
                else
                    ShowCraftResult(0);
            }
        }
    }

    private void InitializeRecipes()
    {
        //recipes.Add(new HandCraftRecipe(new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 0));
        int i = 5;
        recipes.Add(new HandCraftRecipe(new int[] { i, 0, 0, i, 0 ,0, 0, 0, 0 }, 20));
        recipes.Add(new HandCraftRecipe(new int[] { 0, 0, 0, i, 0, 0, i, 0, 0 }, 20));
        recipes.Add(new HandCraftRecipe(new int[] { 0, i, 0, 0, i, 0, 0, 0, 0 }, 20));
        recipes.Add(new HandCraftRecipe(new int[] { 0, 0, 0, 0, i, 0, 0, i, 0 }, 20));
        recipes.Add(new HandCraftRecipe(new int[] { 0, 0, i, 0, 0, i, 0, 0, 0 }, 20));
        recipes.Add(new HandCraftRecipe(new int[] { 0, 0, 0, 0, 0, i, 0, 0, i }, 20));

        i = (int)BlockType.Stick;
        int j = (int)BlockType.IronOre;
        int k = (int)BlockType.Sword;
        recipes.Add(new HandCraftRecipe(new int[] { j, 0, 0, j, 0, 0, i, 0, 0 }, k));
        recipes.Add(new HandCraftRecipe(new int[] { 0, j, 0, 0, j, 0, 0, i, 0 }, k));
        recipes.Add(new HandCraftRecipe(new int[] { 0, 0, j, 0, 0, j, 0, 0, i }, k));

    }

    private void UpdateCraftIdArray()
    {
        int[] arr = new int[9];
        for (int i = 0; i < 9; i++)
        {            
            if (!craftSlots[i].itemSlot.HasItem)
                arr[i] = 0;
            else
                arr[i] = craftSlots[i].itemSlot.stack.id;
        }
        craftIdArray = arr;
    }

    private void ShowCraftResult(int ID)
    {
        if (ID == 0)
        {
            if (resultSlot.itemSlot.HasItem)
                resultSlot.itemSlot.EmptySlot();
        }
        else
        {
            resultSlot.itemSlot.InsertStack(new ItemStack(ID, 1));
        }
    }
}
