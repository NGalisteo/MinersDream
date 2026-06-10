using Inventory.Model;
using UnityEngine;

public class PlacementState : IBuildingState
{
    ItemSO item;
    Grid grid;
    PreviewSystem previewSystem;
    CellContent placedObjectsData;
    PlacedObjectTracker objectPlacer;
    PlayerInventory inventoryData;
    PlacementSystem placementSystem;

    public PlacementState(ItemSO item,
                          Grid grid,
                          PreviewSystem previewSystem,
                          CellContent placedObjectsData,
                          PlacedObjectTracker objectPlacer,
                          PlayerInventory inventoryData,
                          PlacementSystem placementSystem)
    {
        this.item = item;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.placedObjectsData = placedObjectsData;
        this.objectPlacer = objectPlacer;
        this.inventoryData = inventoryData;
        this.placementSystem = placementSystem;

        previewSystem.StartShowingPlacementPreview(this.item.Prefab, this.item.Size);
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
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

        Vector3 spawnPosition = PreviewSystem.GetFootprintCenter(grid.CellToWorld(gridPosition), item.Size);
        GameObject selectedGameObject = objectPlacer.PlaceObject(item.Prefab, spawnPosition);
        PlacedItemInfo selectedGameObjectInfo = selectedGameObject.GetComponent<PlacedItemInfo>();
        selectedGameObjectInfo.item = item;
        selectedGameObjectInfo.gridPosition = gridPosition;
        placedObjectsData.AddObjectAt(gridPosition, item.Size, item, selectedGameObjectInfo.trackingNumber);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);

        if (inventoryData.RemoveItem(item) == 0)
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