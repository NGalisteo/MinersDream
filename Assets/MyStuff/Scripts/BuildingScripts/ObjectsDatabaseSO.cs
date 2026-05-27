using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu] //lets you create one of these as a file

public class ObjectsDatabaseSO : ScriptableObject ////a data container that lives as an asset file, not a gameobject.
{
    public List<ObjectData> objectsData; //its just a list of items with the properties of objectsdata below
}

[Serializable]
public class ObjectData
{
    [field: SerializeField] //serializefield only works for variables, for properties you need this.
    public string Name { get; private set; } //name lol
    [field: SerializeField]
    public int ID { get; private set; } //id lmao
    [field: SerializeField]
    public Vector2Int Size { get; private set; } = Vector2Int.one; //size, it defaults to 1x1 in case we dont put a size
    [field: SerializeField]
    public GameObject Prefab { get; private set; } //the prefab or model for this item.
}