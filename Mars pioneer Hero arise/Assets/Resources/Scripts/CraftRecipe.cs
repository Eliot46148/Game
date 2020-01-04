using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCraftRecipe
{
    public int[] recipe = new int[9];
    public int result;
    public int amount;
    
    public HandCraftRecipe(int[] _recipe, int _result)
    {
        recipe = _recipe;
        result = _result;
        amount = 1;
    }

    public HandCraftRecipe(int[] _recipe, int _result, int _amount)
    {
        recipe = _recipe;
        result = _result;
        amount = _amount;
    }
}
