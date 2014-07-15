using System.Collections.Generic;
using UnityEngine;
using System.Collections;


[System.Serializable]
public class Blueprint
{
    public string printName;
    public List<ItemSlot> requiredItems;
    public List<ItemSlot> outputItems;
}
