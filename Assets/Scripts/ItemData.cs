using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Items")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public GameObject mesh;
    public int maxStackSize = 3;
}
