using Inventory.Model;
using System;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{

    [SerializeField]
    private InputManager inputManager; //reference to inputmanager, need it to subscribe to onclicked and onexit
    [SerializeField]
    private Grid grid;


    [SerializeField]
    private GameObject gridVisualization; //visual grid i see on the ground when entering buildmode

    private GridData placedObjectsData; //This tracks which grid cells are occupied, every time i place an item it registers it here

    [SerializeField]
    private PreviewSystem preview; //ghostpreview that follows my cursors when im in build mode.

    private Vector3Int lastDetectedPosition = Vector3Int.zero; //Store the last grid cell the mouse was on

    [SerializeField]
    private ObjectPlacer objectPlacer; //this that physically spawns and destroys the gameobjects(items) in the scene.

    [SerializeField]
    InventorySO inventoryData;




    IBuildingState buildingState; //This is the state pattern interface, it holds placementstate and removingstate


    void Start()
    {
        StopPlacement(); // makes sure the building mode is hidden and inactive
        placedObjectsData = new(); //crates a fresh empty grid data trackers, to see which cells are occupied
    }


    public void StartPlacement(ItemSO item) //this is called by the ui buttons, it passes the id and starts placing the items with that ID
    {
        StopPlacement(); //cleans every previous state, so if i was already placing something it resets and prevents bugs.
        gridVisualization.SetActive(true); //shows the grid on the ground.
        buildingState = new PlacementState(item, //creates a new placement state and passes everything it needs, so from now on it knows which item im placing and how to handle it
                                           grid,
                                           preview,
                                           placedObjectsData,
                                           objectPlacer,
                                           inventoryData,
                                           this);
        inputManager.OnClicked += PlaceStructure; //basically this is where we subscribe to the events, its basically telling the input manager: when the player clicks, call this method.
        inputManager.OnExit += StopPlacement; //same but when the player presses escape
    } //Inputmanager doesnt know this exists, it just subscribes to the event thats in there, so it "hears" it and runs the method we assigned in for.


    private void PlaceStructure() //this runs whenever the player licks, because placestructure is subscribed to the onclicked event.
    {
        if (inputManager.IsPointerOverUI()) //basically, if we're clicking a UI button, do nothing and return early, this stops from placing stuff "behind" the button
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition(); //gets the 3D world position under the cursor, using the raycast from inputmanager.
        Vector3Int gridPosition = grid.WorldToCell(mousePosition); //converts that world position into a grid cell adress, like (2.5, 0, 1.5) is converted to (2, 0, 1)

        buildingState.OnAction(gridPosition);// tells the current state to do whatever its supposed to, so if we're building it builds, if removing it removes lol
    }

    public void StopPlacement()
    {
    
        gridVisualization.SetActive(false); //hides the grid
        if (buildingState == null) //if nothing is active just do nothing, prevents crashes
            return;
        buildingState.EndState(); //tells the current state to cleans itself up, like destroying the preview ghost for example.
        inputManager.OnClicked -= PlaceStructure; //this unsubscribes from the events, without this place structure would keep firing every click, even if i stopped placing.
        inputManager.OnExit -= StopPlacement; //same, remember to always unsubscribe when im done.
        lastDetectedPosition = Vector3Int.zero; //resets the last cell pos so next time i enter the first cell it detects its the correct one
        buildingState = null; //clears the state so update knows nothing is active.

    }

    private void Update()
    {
        if (buildingState == null) //if buildingstate is null returns, it doesnt need to do anything.
            return;
        Vector3 mousePosition = inputManager.GetSelectedMapPosition(); //gets mouse world position
        Vector3Int gridPosition = grid.WorldToCell(mousePosition); //converts it to cell
        if (lastDetectedPosition != gridPosition) //only runs the logic when the mouse has moved to a different cell.
        {
            buildingState.UpdateState(gridPosition); //tells the active state to update the preview position and the color
            lastDetectedPosition = gridPosition; //stores the current cell so next frame can be compared again.
        }

    }

    public void StartRemoving() //same as startplacement but for removing, no ID needed since we're not selecting an item, just entering remove mode. creates a removingstate instead of placementstate.
    {
        StopPlacement();
        gridVisualization.SetActive(true );
        buildingState = new RemovingState(grid, preview, placedObjectsData, objectPlacer, inventoryData);
        inputManager.OnClicked += PlaceStructure; //still subscribes to the same events, clicking will now remove instead of place because removingstate handles onaction differently
        inputManager.OnExit += StopPlacement; //same for here
    }

}
