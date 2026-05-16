using UnityEngine;
using UnityEngine.Windows;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private Camera sceneCamera;

    private Vector3 lastPosition;

    [SerializeField]
    private LayerMask placementLayerMask;

    private PlayerInputActions action;

    private void Awake()
    {
        action = new PlayerInputActions();
    }

    private void OnEnable()
    {
        action.Enable();
    }

    private void OnDisable()
    {
        action.Disable();
    }
    public Vector3 GetSelectedMapPosition()
    {;
        Vector2 mousePos = action.BuildingSystem.CursorPosition.ReadValue<Vector2>();
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100,  placementLayerMask))
        {
            lastPosition = hit.point;
        }
        return lastPosition;
    }
}
