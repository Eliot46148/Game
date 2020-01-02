using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCraftRecipe
{
    public int[] recipe = new int[4];
    public int result;
    
    public HandCraftRecipe(int[] _recipe, int _result)
    {
        recipe = _recipe;
        result = _result;
    }
}

public class TableCraftRecipe
{
    public int[] recipe = new int[9];
    public int result;

    public TableCraftRecipe(int[] _recipe, int _result)
    {
        recipe = _recipe;
        result = _result;
    }
}