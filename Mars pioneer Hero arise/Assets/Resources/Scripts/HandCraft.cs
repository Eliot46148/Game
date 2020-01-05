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
                    ShowCraftResult(r.result, r.amount);
                    break;
                }
                else
                    ShowCraftResult(0, r.amount);
            }
        }
    }

    private void InitializeRecipes()
    {
        //recipes.Add(new HandCraftRecipe(new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 0));
        int i = 5;
        recipes.Add(new HandCraftRecipe(new int[] { i, 0, 0, i, 0 ,0, 0, 0, 0 }, 20, 4));
        recipes.Add(new HandCraftRecipe(new int[] { 0, 0, 0, i, 0, 0, i, 0, 0 }, 20, 4));
        recipes.Add(new HandCraftRecipe(new int[] { 0, i, 0, 0, i, 0, 0, 0, 0 }, 20, 4));
        recipes.Add(new HandCraftRecipe(new int[] { 0, 0, 0, 0, i, 0, 0, i, 0 }, 20, 4));
        recipes.Add(new HandCraftRecipe(new int[] { 0, 0, i, 0, 0, i, 0, 0, 0 }, 20, 4));
        recipes.Add(new HandCraftRecipe(new int[] { 0, 0, 0, 0, 0, i, 0, 0, i }, 20, 4));

        i = (int)BlockType.Stick;
        int j = (int)BlockType.Wooden;
        int k = (int)BlockType.WoodenSword;
        recipes.Add(new HandCraftRecipe(new int[] { j, 0, 0, j, 0, 0, i, 0, 0 }, k));
        recipes.Add(new HandCraftRecipe(new int[] { 0, j, 0, 0, j, 0, 0, i, 0 }, k));
        recipes.Add(new HandCraftRecipe(new int[] { 0, 0, j, 0, 0, j, 0, 0, i }, k));
        k = (int)BlockType.WoodenShavel;
        recipes.Add(new HandCraftRecipe(new int[] { j, 0, 0, i, 0, 0, i, 0, 0 }, k));
        recipes.Add(new HandCraftRecipe(new int[] { 0, j, 0, 0, i, 0, 0, i, 0 }, k));
        recipes.Add(new HandCraftRecipe(new int[] { 0, 0, j, 0, 0, i, 0, 0, i }, k));
        k = (int)BlockType.WoodenAxe;
        recipes.Add(new HandCraftRecipe(new int[] { j, j, 0, j, i, 0, 0, i, 0 }, k));
        recipes.Add(new HandCraftRecipe(new int[] { 0, j, j, 0, j, i, 0, 0, i }, k));
        recipes.Add(new HandCraftRecipe(new int[] { 0, j, j, 0, i, j, 0, i, 0 }, k));
        recipes.Add(new HandCraftRecipe(new int[] { j, j, 0, i, j, 0, i, 0, 0 }, k));
        k = (int)BlockType.WoodenPickaxe;
        recipes.Add(new HandCraftRecipe(new int[] { j, j, j, 0, i, 0, 0, i, 0 }, k));

        j = (int)BlockType.IronOre;
        k = (int)BlockType.IronSword;
        recipes.Add(new HandCraftRecipe(new int[] { j, 0, 0, j, 0, 0, i, 0, 0 }, k));
        recipes.Add(new HandCraftRecipe(new int[] { 0, j, 0, 0, j, 0, 0, i, 0 }, k));
        recipes.Add(new HandCraftRecipe(new int[] { 0, 0, j, 0, 0, j, 0, 0, i }, k));
        k = (int)BlockType.IronShavel;
        recipes.Add(new HandCraftRecipe(new int[] { j, 0, 0, i, 0, 0, i, 0, 0 }, k));
        recipes.Add(new HandCraftRecipe(new int[] { 0, j, 0, 0, i, 0, 0, i, 0 }, k));
        recipes.Add(new HandCraftRecipe(new int[] { 0, 0, j, 0, 0, i, 0, 0, i }, k));
        k = (int)BlockType.IronAxe;
        recipes.Add(new HandCraftRecipe(new int[] { j, j, 0, j, i, 0, 0, i, 0 }, k));
        recipes.Add(new HandCraftRecipe(new int[] { 0, j, j, 0, j, i, 0, 0, i }, k));
        recipes.Add(new HandCraftRecipe(new int[] { 0, j, j, 0, i, j, 0, i, 0 }, k));
        recipes.Add(new HandCraftRecipe(new int[] { j, j, 0, i, j, 0, i, 0, 0 }, k));
        k = (int)BlockType.IronPickaxe;
        recipes.Add(new HandCraftRecipe(new int[] { j, j, j, 0, i, 0, 0, i, 0 }, k));

        j = (int)BlockType.GoldOre;
        k = (int)BlockType.GoldenSword;
        recipes.Add(new HandCraftRecipe(new int[] { j, 0, 0, j, 0, 0, i, 0, 0 }, k));
        recipes.Add(new HandCraftRecipe(new int[] { 0, j, 0, 0, j, 0, 0, i, 0 }, k));
        recipes.Add(new HandCraftRecipe(new int[] { 0, 0, j, 0, 0, j, 0, 0, i }, k));
        k = (int)BlockType.GoldenShavel;
        recipes.Add(new HandCraftRecipe(new int[] { j, 0, 0, i, 0, 0, i, 0, 0 }, k));
        recipes.Add(new HandCraftRecipe(new int[] { 0, j, 0, 0, i, 0, 0, i, 0 }, k));
        recipes.Add(new HandCraftRecipe(new int[] { 0, 0, j, 0, 0, i, 0, 0, i }, k));
        k = (int)BlockType.GoldenAxe;
        recipes.Add(new HandCraftRecipe(new int[] { j, j, 0, j, i, 0, 0, i, 0 }, k));
        recipes.Add(new HandCraftRecipe(new int[] { 0, j, j, 0, j, i, 0, 0, i }, k));
        recipes.Add(new HandCraftRecipe(new int[] { 0, j, j, 0, i, j, 0, i, 0 }, k));
        recipes.Add(new HandCraftRecipe(new int[] { j, j, 0, i, j, 0, i, 0, 0 }, k));
        k = (int)BlockType.GoldenPickaxe;
        recipes.Add(new HandCraftRecipe(new int[] { j, j, j, 0, i, 0, 0, i, 0 }, k));

        i = (int)BlockType.OakLog;
        j = (int)BlockType.Wooden;
        recipes.Add(new HandCraftRecipe(new int[] { i, 0, 0, 0, 0, 0, 0, 0, 0 }, j, 4));
        recipes.Add(new HandCraftRecipe(new int[] { 0, i, 0, 0, 0, 0, 0, 0, 0 }, j, 4));
        recipes.Add(new HandCraftRecipe(new int[] { 0, 0, i, 0, 0, 0, 0, 0, 0 }, j, 4));
        recipes.Add(new HandCraftRecipe(new int[] { 0, 0, 0, i, 0, 0, 0, 0, 0 }, j, 4));
        recipes.Add(new HandCraftRecipe(new int[] { 0, 0, 0, 0, i, 0, 0, 0, 0 }, j, 4));
        recipes.Add(new HandCraftRecipe(new int[] { 0, 0, 0, 0, 0, i, 0, 0, 0 }, j, 4));
        recipes.Add(new HandCraftRecipe(new int[] { 0, 0, 0, 0, 0, 0, i, 0, 0 }, j, 4));
        recipes.Add(new HandCraftRecipe(new int[] { 0, 0, 0, 0, 0, 0, 0, i, 0 }, j, 4));
        recipes.Add(new HandCraftRecipe(new int[] { 0, 0, 0, 0, 0, 0, 0, 0, i }, j, 4));

        i = (int)BlockType.Wooden;
        j = (int)BlockType.Box;
        recipes.Add(new HandCraftRecipe(new int[] { i, i, 0, i, i, 0, 0, 0, 0 }, j));
        recipes.Add(new HandCraftRecipe(new int[] { 0, i, i, 0, i, i, 0, 0, 0 }, j));
        recipes.Add(new HandCraftRecipe(new int[] { 0, 0, 0, i, i, 0, i, i, 0 }, j));
        recipes.Add(new HandCraftRecipe(new int[] { 0, 0, 0, 0, i, i, 0, i, i }, j));

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

    private void ShowCraftResult(int ID, int amt)
    {
        if (ID == 0)
        {
            if (resultSlot.itemSlot.HasItem)
                resultSlot.itemSlot.EmptySlot();
        }
        else
        {
            resultSlot.itemSlot.InsertStack(new ItemStack(ID, amt));
        }
    }

}
