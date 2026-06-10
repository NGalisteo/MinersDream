using Inventory.Model;
using UnityEngine;

public class RemovingState : IBuildingState
{
    Grid grid;
    PreviewSystem previewSystem;
    GridData placedObjectsData;
    ObjectPlacer objectPlacer;
    PlayerInventory inventoryData;

    public RemovingState(Grid grid,
                         PreviewSystem previewSystem,
                         GridData placedObjectsData,
                         ObjectPlacer objectPlacer,
                         PlayerInventory inventoryData)
    {
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.placedObjectsData = placedObjectsData;
        this.objectPlacer = objectPlacer;
        this.inventoryData = inventoryData;
        previewSystem.StartShowingRemovePreview();
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public Vector2Int GetSize()
    {
        return Vector2Int.one;
    }
    public void OnAction(Vector3Int gridPosition)
    {
        if (placedObjectsData.CanPlaceObjectAt(gridPosition, Vector2Int.one) == false)
        {
            int gameObjectIndex = placedObjectsData.GetObjectIndex(gridPosition);
            ItemSO item = placedObjectsData.GetItemAt(gridPosition);
            inventoryData.AddItem(item, 1);
            placedObjectsData.RemoveObjectAt(gridPosition);
            objectPlacer.RemoveObjectAt(gameObjectIndex);
        }
        Vector3 cellPosition = grid.CellToWorld(gridPosition);
        previewSystem.UpdatePosition(cellPosition, CheckIfSelectionIsValid(gridPosition));
    }

    private bool CheckIfSelectionIsValid(Vector3Int gridPosition)
    {
        return !(placedObjectsData.CanPlaceObjectAt(gridPosition, Vector2Int.one));
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool validity = CheckIfSelectionIsValid(gridPosition);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), validity);
    }
}