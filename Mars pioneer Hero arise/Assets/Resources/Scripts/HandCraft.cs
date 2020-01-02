using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCraft : MonoBehaviour
{
    public GameObject InventoryPanel;
    
    public UIItemSlot[] craftSlots;
    public UIItemSlot resultSlot;

    int[] craftIdArray;

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
                for (int i = 0; i < 4; i++)
                {
                    if (craftIdArray[i] != r.recipe[i])
                        isEqual = false;
                }
                
                if (isEqual)
                    ShowCraftResult(r.result);                
            }
        }
    }

    private void InitializeRecipes()
    {
        recipes.Add(new HandCraftRecipe(new int[] { 5, 0, 5, 0 }, 10));
        recipes.Add(new HandCraftRecipe(new int[] { 0, 5, 0, 5 }, 10));
    }

    private void UpdateCraftIdArray()
    {
        int[] arr = new int[4] { 0, 0, 0, 0 };
        for (int i = 0; i < 4; i++)
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
        resultSlot.itemSlot.InsertStack(new ItemStack(ID, 1));
    }
}
