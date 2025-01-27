using System;
using UnityEngine;

[Serializable]
public struct Recipe
{
    public ItemData item1;
    public ItemData item2;
    public ItemData item3;
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "Items")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public GameObject mesh;
    public int maxStackSize = 3;
    public bool canBeCrafted;
    public Recipe craftRecipe;
}
