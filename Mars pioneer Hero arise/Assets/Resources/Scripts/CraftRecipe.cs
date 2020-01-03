using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCraftRecipe
{
    public int[] recipe = new int[9];
    public int result;
    
    public HandCraftRecipe(int[] _recipe, int _result)
    {
        recipe = _recipe;
        result = _result;
    }
}
