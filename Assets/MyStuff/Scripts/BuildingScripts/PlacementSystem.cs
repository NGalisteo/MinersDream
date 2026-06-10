using Inventory.Model;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private GameObject gridVisualization;
    private CellContent placedObjectsData;

    [SerializeField]
    private PreviewSystem preview;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    [SerializeField]
    private PlacedObjectTracker objectPlacer;

    [SerializeField]
    PlayerInventory inventoryData;

    IBuildingState buildingState;

    void Start()
    {
        StopPlacement();
        placedObjectsData = new();
    }

    public void StartPlacement(ItemSO item)
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new PlacementState(item, grid, preview, placedObjectsData, objectPlacer, inventoryData, this);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI())
            return;

        Vector3Int gridPosition = GetCenteredGridPosition();
        buildingState.OnAction(gridPosition);
    }

    public bool IsBuilding()
    {
        if(buildingState != null)
        {
            return true;
        }
        return false;
    }
    public void StopPlacement()
    {
        gridVisualization.SetActive(false);
        if (buildingState == null)
            return;
        buildingState.EndState();
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;
        buildingState = null;
    }

    private void Update()
    {
        if (buildingState == null)
            return;

        Vector3Int gridPosition = GetCenteredGridPosition();
        if (lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }
    }

    // Returns the grid cell offset so the cursor sits at the CENTER of the item footprint, not the corner
    private Vector3Int GetCenteredGridPosition()
    {
        Vector3 mousePosition = inputManager.GetMouseWorldPosition();
        Vector2Int size = buildingState.GetSize();
        // Shift mouse position back by half size so the corner cell lands centered under mouse
        Vector3 offsetMouse = new Vector3(
            mousePosition.x - size.x / 2f,
            mousePosition.y,
            mousePosition.z - size.y / 2f
        );
        return grid.WorldToCell(offsetMouse);
    }

    public void StartRemoving()
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new RemovingState(grid, preview, placedObjectsData, objectPlacer, inventoryData);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }
}