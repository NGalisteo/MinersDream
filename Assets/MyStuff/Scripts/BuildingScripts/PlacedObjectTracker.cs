using System.Collections.Generic;
using UnityEngine;

public class PlacedObjectTracker : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> placedGameObjects = new(); //list of every item PLACED in the scene

    public GameObject PlaceObject(GameObject prefab, Vector3 position)// places an object with the prefab and position we pass through.
    {
        GameObject newObject = Instantiate(prefab); //spawns a copy of the prefab in the scene
        newObject.transform.position = position; //moves it to the correct grid position
        PlacedItemInfo placedItemInfo = newObject.GetComponent<PlacedItemInfo>();
        placedGameObjects.Add(newObject);//adds it to the list so we get track of every item placed in the scene
        placedItemInfo.trackingNumber = placedGameObjects.Count - 1;
        return newObject;//returns the index of the item we just added. If the list had 3 items and we just added a 4th, Count is 4 so we return 3 (because lists start at index 0). 
    }


    internal void RemoveObjectAt(int gameObjectIndex)// removes the object
    {
        if (placedGameObjects.Count <= gameObjectIndex
            || placedGameObjects[gameObjectIndex] == null) //is a safeguard, if the index doestn exist or the object was already destroyed do nothing, prevents crashes
            return;
        Destroy(placedGameObjects[gameObjectIndex]); //removes the gameobject from the scene
        placedGameObjects[gameObjectIndex] = null;// we set it to null because if we destroyed it all the items after it will shift an index lower, and would break the ticket numbers , and it would point to the wrong items.
    }

    public void ShowAllFootprints()
    {
        foreach (var item in placedGameObjects)
        {
            if(item != null)
            {
                PlacedItemInfo placedItemInfo = item.GetComponent<PlacedItemInfo>();
                placedItemInfo.HighlightHover();
            }
        }
    }

    public void HideAllFootprints()
    {
        foreach (var item in placedGameObjects)
        {
            if (item != null)
            {
                PlacedItemInfo placedItemInfo = item.GetComponent<PlacedItemInfo>();
                placedItemInfo.Unhighlight();
            }
        }
    }
}
