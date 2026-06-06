using Inventory.Model;
using UnityEngine;

public class PlacementState : IBuildingState //implement the buildingstate contract interface
{
    private int selectedObjectIndex = -1;
    int ID;
    Grid grid;
    PreviewSystem previewSystem;
    ObjectsDatabaseSO database;
    GridData placedObjectsData;
    ObjectPlacer objectPlacer;
    InventorySO inventoryData;
    PlacementSystem placementSystem;

    public PlacementState(int iD,
                          Grid grid,
                          PreviewSystem previewSystem,
                          ObjectsDatabaseSO database,
                          GridData placedObjectsData,
                          ObjectPlacer objectPlacer,
                          InventorySO inventoryData,
                          PlacementSystem placementSystem) //we dont use serializefield cos we pass all the necesssary references
    {
        ID = iD;
        this.grid = grid; //this grid refers to the variable, grid refers to the parameter of the constructor
        this.previewSystem = previewSystem;
        this.database = database;
        this.placedObjectsData = placedObjectsData;
        this.objectPlacer = objectPlacer;
        this.inventoryData = inventoryData;
        this.placementSystem = placementSystem;

        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID); //searches the database for the item that matches the id we passed in. for each item called data in the list, check if data.ID == ID
        if (selectedObjectIndex > -1) //returns the index, if it returns -1, is a notfound
        {
            previewSystem.StartShowingPlacementPreview(
                database.objectsData[selectedObjectIndex].Prefab, //we pass the prefab
                database.objectsData[selectedObjectIndex].Size); //we pass the size to resize the cursor
        }
        else
            throw new System.Exception($"No object with IS {iD}"); //throw an exception

    }

    public void EndState() //required by the interface, when the placement mode ends,  destroy the ghost preview.
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition) //required by the interface, this runs when the player clicks
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex); //if the cell is occupied, return early and dont do anything
        if (!placementValidity)
            return;

        int index = objectPlacer.PlaceObject(database.objectsData[selectedObjectIndex].Prefab, grid.CellToWorld(gridPosition)); //spawns the real gameobject and returns the index ticket. converts the cell adress back to world position, so the item gets placed correctly


        //  GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ?
        //  floorData :
        //  furnitureData;
        placedObjectsData.AddObjectAt(gridPosition,
            database.objectsData[selectedObjectIndex].Size,
            database.objectsData[selectedObjectIndex].ID,
            index);//registers the cells as occupied, storing the index ticket.
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false); // after the placing updates the color to red, this gives visual feedback  that the cell is occupied before moving the mouse and "updating" again.
        if(inventoryData.UseItem(database.objectsData[selectedObjectIndex].inventoryItem, 1) == 0)
        {
            placementSystem.StopPlacement();
        }
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex) //just to make onaction cleaner.
    {
        //  GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? 
        //      floorData : 
        //     furnitureData;

        return placedObjectsData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size); //instead of this long line in onaction, we put it here so the purpose and code is cleaner
    }

    public void UpdateState(Vector3Int gridPosition) //required by the interface, called every frame when the mouse moves to a new cell.
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex); //checks validity

        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity); //updates the ghost color and the position of the cursor.
    }
}
