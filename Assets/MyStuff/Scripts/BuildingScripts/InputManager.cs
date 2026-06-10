using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private Camera sceneCamera; //basically we get the main camera in this case.

    private Vector3 lastPosition; //this will store the last valid position the raycast hit.

    [SerializeField]
    private LayerMask placementLayerMask; //this basically tells the raycast to only hit that layer and ignore the others (in this case Placement layer)

    private PlayerInputActions action; // for input actions, new input system


    public event Action OnClicked, OnExit; //These are events, it notifies that something happened, they take no parameters and they return nothing


    private void Awake()
    {
        action = new PlayerInputActions(); // just the enable for inputs, always put on awake
    }


    private void OnEnable() //start listening for input
    {
        action.Enable();
    }

    private void OnDisable() //stops listening for input
    {
        action.Disable();
    }

    private void Update()// this runs every frame
    {
        if (action.BuildingSystem.PlaceItem.WasPressedThisFrame()) //this returns true on exactly one frame, the moment the button is pressed. in this case, placeitem is left click.
            OnClicked?.Invoke(); //the ?. means that only call invoke if someone is subscribed to this event, if not, do nothing

        //same for this
        if (action.BuildingSystem.Escap.WasPressedThisFrame())
            OnExit?.Invoke();
    }

    public bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    public Vector3 GetMouseWorldPosition()
    {
        Vector2 mousePos = action.BuildingSystem.CursorPosition.ReadValue<Vector2>(); //reads the current mouse 2d screen position in pixels, like (676, 69)
        Ray ray = sceneCamera.ScreenPointToRay(mousePos); //the camera shoots a raycast, like a lazer pointer.

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, placementLayerMask))//this makes the ray fire, up to 100 units away, out hit means that fills the variable with what you hit, in this case it will fill the variable with the exact 3d point position coordinate thats under the mouse
        {
            lastPosition = hit.point; //lets store the last position the ray hit.
        }
        return lastPosition; //returns the last hit position, if the mouse stops touching the ground, it keeps the last position instead of going insane
    }
}
