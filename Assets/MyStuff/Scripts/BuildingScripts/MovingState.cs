using Inventory.Model;
using UnityEngine;

public class MovingState : IBuildingState
{
    ItemSO item;
    Grid grid;
    PreviewSystem previewSystem;
    CellContent placedObjectsData;
    PlacedObjectTracker objectPlacer;
    PlayerInventory inventoryData;
    PlacementSystem placementSystem;
    PlacedItemInfo placedItemInfo;
    Vector3Int placedPosition;
    bool placed;

    public MovingState(ItemSO item,
                          Grid grid,
                          PreviewSystem previewSystem,
                          CellContent placedObjectsData,
                          PlacedObjectTracker objectPlacer,
                          PlayerInventory inventoryData,
                          PlacementSystem placementSystem,
                          PlacedItemInfo placedItemInfo)
    {
        this.item = item;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.placedObjectsData = placedObjectsData;
        this.objectPlacer = objectPlacer;
        this.inventoryData = inventoryData;
        this.placementSystem = placementSystem;
        this.placedItemInfo = placedItemInfo;

        previewSystem.StartShowingPlacementPreview(this.item.Prefab, this.item.Size);
        placementSystem.RemoveItemForMoving(placedItemInfo);
        placedPosition = placedItemInfo.gridPosition;
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
        if(!placed)
        {
            OnAction(placedPosition);
        }
    }

    public Vector2Int GetSize()
    {
        return item.Size;
    }

    public void OnAction(Vector3Int gridPosition)
    {

        bool placementValidity = CheckPlacementValidity(gridPosition);
        if (!placementValidity)
            return;
        placed = true;

        Vector3 spawnPosition = PreviewSystem.GetFootprintCenter(grid.CellToWorld(gridPosition), item.Size);
        GameObject selectedGameObject = objectPlacer.PlaceObject(item.Prefab, spawnPosition);
        PlacedItemInfo selectedGameObjectInfo = selectedGameObject.GetComponent<PlacedItemInfo>();
        selectedGameObjectInfo.item = item;
        selectedGameObjectInfo.gridPosition = gridPosition;
        placedObjectsData.AddObjectAt(gridPosition, item.Size, item, selectedGameObjectInfo.trackingNumber);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
        placementSystem.StopPlacement();
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition)
    {
        return placedObjectsData.CanPlaceObjectAt(gridPosition, item.Size);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
    }
}
