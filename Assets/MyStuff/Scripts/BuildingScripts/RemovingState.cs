using Inventory.Model;
using System;
using UnityEngine;

public class RemovingState : IBuildingState //same contract as placementstate.
{
    Grid grid;
    PreviewSystem previewSystem;
    GridData placedObjectsData;
    ObjectPlacer objectPlacer;
    InventorySO inventoryData;

    public RemovingState(Grid grid, //no id needed because we're removing,and we dont care about the item, just where its placed.
                         PreviewSystem previewSystem,
                         GridData placedObjectsData,
                         ObjectPlacer objectPlacer,
                         InventorySO inventoryData)
    {
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.placedObjectsData = placedObjectsData;
        this.objectPlacer = objectPlacer;
        this.inventoryData = inventoryData;

        previewSystem.StartShowingRemovePreview(); //shows the 1x1 red cursos to give feedback we're removing.
    }

    public void EndState() //required by the interface
    {
        previewSystem.StopShowingPreview(); //stops showing the preview, stops build mode?
    }

    public void OnAction(Vector3Int gridPosition)
    {
        if (placedObjectsData.CanPlaceObjectAt(gridPosition, Vector2Int.one) == false) //if the cell is occupied, that means theres something there
        {
            int gameObjectIndex = placedObjectsData.GetObjectIndex(gridPosition); //get the index ticket for the item at that cell
            ItemSO item = placedObjectsData.GetItemAt(gridPosition);
            inventoryData.AddItem(item, 1);
            placedObjectsData.RemoveObjectAt(gridPosition); //clears the cells from gridData and the dictionary
            objectPlacer.RemoveObjectAt(gameObjectIndex); //destroys the actual gameobject.
        }
        Vector3 cellPosition = grid.CellToWorld(gridPosition); //converts cell position to world position
        previewSystem.UpdatePosition(cellPosition, CheckIfSelectionIsValid(gridPosition)); //after removing, updates the cursor color, so its red again, because theres nothing now.
    }

    private bool CheckIfSelectionIsValid(Vector3Int gridPosition) //returns true if something is at the cell, cos its valid to remove.
    {
        return !(placedObjectsData.CanPlaceObjectAt(gridPosition, Vector2Int.one)); //we use the method backwards lol
    }

    public void UpdateState(Vector3Int gridPosition) //updates every time the mouse moves, cursor is white if theres something to remove, turns red if theres nothing to remove.
    {
        bool validity = CheckIfSelectionIsValid(gridPosition);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), validity);
    }
}
