using Inventory.Model;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CellContent
{
    Dictionary<Vector3Int, PlacedItem> placedObjects = new(); //its like a real dictionary, you look something up by a key and get something back, in this case a value
                                                                 // in this case, the key is the vector3int, the cell grid address like 2, 0, 1. and the value, what you get back is the placementdata, basically everything thats placed in that cell.
                                                                 //its way faster than a list.
    public void AddObjectAt(Vector3Int gridPosition,
                            Vector2Int objectSize,
                            ItemSO item,
                            int placedObjectIndex) //this registers a placed item in the dictionary. it takes the grid position it was placed, the items size, its id and the index ticket from objectplacer
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize); //figures out every cell that the item occupies
        PlacedItem data = new PlacedItem(positionToOccupy, placedObjectIndex, item ); //creates a data package containing all the information about this item we placed
        foreach (var pos in positionToOccupy) //loops through every cell the item occupies and registers the same data object at each cell. for example a 2x2 item registers the same data to 4 cells.
        {
            if (placedObjects.ContainsKey(pos)) //if a cell has already any entry for this key, and sometihng triess to register it again, crashes with a message. for debugging.
                throw new Exception($"Dictionary already contains this cell position {pos}");
            placedObjects[pos] = data; //this writes the data to the dictionary
        }
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize) //calculates every cell a multi cell item occupies. basically the 2 fors loop across the x and y axis, because the grid is 2d (in this case, y maps to the z axis in a 2d world space.)
    {                                                                                           //it starts at the placement corner and steps outward, i will ask later what happens if i wanna rotate the item or something, this doesnt seem super dynamic tbh.
        List<Vector3Int> returnVal = new();
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                returnVal.Add(gridPosition + new Vector3Int(x, 0, y)); //adds the cell to returnval. grid position is the position we clicked, and the the fors offset from that.
            }

            // the loop generates x and y offset values: (0,0) (1,0) (0,1) (1,1)
            // we wrap them in a Vector3Int so we can add them to the starting cell position
            // for a 2x2 item placed at (2,0,1):
            // (2,0,1) + (0,0,0) = (2,0,1)
            // (2,0,1) + (1,0,0) = (3,0,1)
            // (2,0,1) + (0,0,1) = (2,0,2)
            // (2,0,1) + (1,0,1) = (3,0,2)
        }
        return returnVal;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize) //before placing, this checks if all the cells the item needs are free.
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize); //it calculates the same positions as AddObjectAt would.
        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos)) //asks the dictionary, if it has any entry for this cell, if every single cell is free, returns true, if not, returns false
                return false;
        }
        return true;
    } //this is called every frame in update to color the preview ghost red or white, and again in Onaction before placing.

    internal int GetObjectIndex(Vector3Int gridPosition) //this is used  by removingstate to get the index ticket of whatever is in a cell.
    {
        if (placedObjects.ContainsKey(gridPosition) == false) // if nothing is in a cell, return -1 as a signal to say nothing is found.
            return -1;
        return placedObjects[gridPosition].PlacedObjectIndex; //otherwhise, it looks up the cell in the dictionary and returns theplaced OBjectIndex and the ticket number that objectPlacer uses to find and destroy the right gameobject.
    }
    internal ItemSO GetItemAt(Vector3Int gridPosition) //this is used  by removingstate to get the index ticket of whatever is in a cell.
    {
        if (placedObjects.ContainsKey(gridPosition) == false) // if nothing is in a cell, return -1 as a signal to say nothing is found.
            return null;
        return placedObjects[gridPosition].Item; //otherwhise, it looks up the cell in the dictionary and returns theplaced OBjectIndex and the ticket number that objectPlacer uses to find and destroy the right gameobject.
    }

    internal void RemoveObjectAt(Vector3Int gridPosition) //removes an item from the dictionary.
    {
        foreach (var pos in placedObjects[gridPosition].OccupiedCells) // looks up the placementdata at a clicked cell, gets the full list of occupied positions, because when im removing a multi cell item, we need to clear all the cells not just the one i clicked.
        {
            placedObjects.Remove(pos); //removes it from the dictionary
        }
    }
}

public class PlacedItem //data package stored at each occupied cell.
{
    public List<Vector3Int> OccupiedCells; //full list of cells this item takes up, we need it for when removing an item.
    public int PlacedObjectIndex { get; private set; } //ticket number pointing to the real gameobject in the objectPlacer List.

    public ItemSO Item;
    public PlacedItem(List<Vector3Int> occupiedCells, int placedObjectIndex, ItemSO item)
    {
        this.OccupiedCells = occupiedCells;
        this.PlacedObjectIndex = placedObjectIndex;
        this.Item = item;
    }

    /*
     HOW GRIDDATA CONNECTS EVERYTING
    Player clicks to place
 PlacementState.OnAction checks CanPlaceObjectAt()  asks GridData
 ObjectPlacer.PlaceObject() spawns the GameObject, returns index ticket
 GridData.AddObjectAt() registers the cells with the ticket

Player clicks to remove
 RemovingState.OnAction checks CanPlaceObjectAt()  if false, something is there
 GridData.GetRepresentationIndex() returns the ticket
 GridData.RemoveObjectAt() clears the cells
 ObjectPlacer.RemoveObjectAt() destroys the GameObject using the ticket
     */
}

