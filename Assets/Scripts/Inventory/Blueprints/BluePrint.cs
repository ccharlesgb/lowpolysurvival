using System.Collections.Generic;
using UnityEngine;
using System.Collections;


[System.Serializable]
public class Blueprint
{
    public string printName;
    public List<ItemContainer> requiredItems;
    public List<ItemContainer> outputItems;
}
