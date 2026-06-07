using Inventory.Model;
using UnityEngine;

public class PlacementState : IBuildingState //implement the buildingstate contract interface
{
    ItemSO item;
    Grid grid;
    PreviewSystem previewSystem;
    GridData placedObjectsData;
    ObjectPlacer objectPlacer;
    InventorySO inventoryData;
    PlacementSystem placementSystem;

    public PlacementState(ItemSO item,
                          Grid grid,
                          PreviewSystem previewSystem,
                          GridData placedObjectsData,
                          ObjectPlacer objectPlacer,
                          InventorySO inventoryData,
                          PlacementSystem placementSystem) //we dont use serializefield cos we pass all the necesssary references
    {
        this.item = item;
        this.grid = grid; //this grid refers to the variable, grid refers to the parameter of the constructor
        this.previewSystem = previewSystem;
        this.placedObjectsData = placedObjectsData;
        this.objectPlacer = objectPlacer;
        this.inventoryData = inventoryData;
        this.placementSystem = placementSystem;


            previewSystem.StartShowingPlacementPreview(
                this.item.Prefab, //we pass the prefab
                this.item.Size); //we pass the size to resize the cursor
    }

    public void EndState() //required by the interface, when the placement mode ends,  destroy the ghost preview.
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition) //required by the interface, this runs when the player clicks
    {
        bool placementValidity = CheckPlacementValidity(gridPosition); //if the cell is occupied, return early and dont do anything
        if (!placementValidity)
            return;

        int index = objectPlacer.PlaceObject(item.Prefab, grid.CellToWorld(gridPosition)); //spawns the real gameobject and returns the index ticket. converts the cell adress back to world position, so the item gets placed correctly

        placedObjectsData.AddObjectAt(gridPosition,
            item.Size,
            item,
            index);//registers the cells as occupied, storing the index ticket.
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false); // after the placing updates the color to red, this gives visual feedback  that the cell is occupied before moving the mouse and "updating" again.
        if(inventoryData.RemoveItem(item) == 0)
        {
            placementSystem.StopPlacement();
        }
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition) //just to make onaction cleaner.
    {

        return placedObjectsData.CanPlaceObjectAt(gridPosition, item.Size); //instead of this long line in onaction, we put it here so the purpose and code is cleaner
    }

    public void UpdateState(Vector3Int gridPosition) //required by the interface, called every frame when the mouse moves to a new cell.
    {
        bool placementValidity = CheckPlacementValidity(gridPosition); //checks validity

        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity); //updates the ghost color and the position of the cursor.
    }
}
