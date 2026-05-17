using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private Camera sceneCamera;

    private Vector3 lastPosition;

    [SerializeField]
    private LayerMask placementLayerMask;

    private PlayerInputActions action;


    public event Action OnClicked, OnExit;

    private void Update()
    {
        if (action.BuildingSystem.PlaceItem.WasPressedThisFrame())
            OnClicked?.Invoke();
        if (action.BuildingSystem.Escap.WasPressedThisFrame())
            OnExit?.Invoke();
    }

    public bool IsPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();

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
    {
        Vector2 mousePos = action.BuildingSystem.CursorPosition.ReadValue<Vector2>();
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, placementLayerMask))
        {
            lastPosition = hit.point;
        }
        return lastPosition;
    }
}
