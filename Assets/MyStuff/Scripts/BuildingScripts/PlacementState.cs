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
        float angle = previewSystem.GetPreviewInfo().transform.eulerAngles.y;
        if (previewSystem.GetPreviewInfo().transform.eulerAngles.y == 90 || previewSystem.GetPreviewInfo().transform.eulerAngles.y == 270)
        {
            return new Vector2Int(item.Size.y, item.Size.x);
        }
        return item.Size;

    }

    public void OnAction(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition);
        if (!placementValidity)
            return;

        Vector3 spawnPosition = PreviewSystem.GetFootprintCenter(grid.CellToWorld(gridPosition), GetSize());
        GameObject selectedGameObject = objectPlacer.PlaceObject(item.Prefab, spawnPosition);
        PlacedItemInfo selectedGameObjectInfo = selectedGameObject.GetComponent<PlacedItemInfo>();
        selectedGameObject.transform.rotation = previewSystem.GetPreviewInfo().transform.rotation;
        selectedGameObjectInfo.item = item;
        selectedGameObjectInfo.gridPosition = gridPosition;
        placedObjectsData.AddObjectAt(gridPosition, GetSize(), item, selectedGameObjectInfo.trackingNumber);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);

        if (inventoryData.RemoveItem(item) == 0 )
            placementSystem.StopPlacement();
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition)
    {
        return placedObjectsData.CanPlaceObjectAt(gridPosition, GetSize());
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
    }
}