using UnityEngine;

public class SelectionSystem : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private LayerMask itemLayerMask;

    [SerializeField]
    private PlacementSystem placementSystem;

    private PlacedItemInfo hoveredItemInfo;

    private PlacedItemInfo lastHovered;

    private PlacedItemInfo clickedItemInfo;

    private GameObject hoveredGameObject;

    [SerializeField]
    private SelectionCardUI selectedItem;
    public PlacedItemInfo CurrentHovered {  get { return hoveredItemInfo; } }
    public PlacedItemInfo CurrentSelected {  get { return clickedItemInfo; } }

    private void Update()
    {
        if (placementSystem.IsBuilding() == true)
        {
            return;
        }
        Vector2 mousePosition = inputManager.GetMousePosition();
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, itemLayerMask))
        {
            hoveredGameObject = hit.collider.gameObject;
            hoveredItemInfo = hoveredGameObject.GetComponent<PlacedItemInfo>();
            if (hoveredItemInfo != null && hoveredItemInfo.item != null)
            {
                if (hoveredItemInfo != lastHovered)
                {
                    if (lastHovered != null && lastHovered != clickedItemInfo)
                    {
                        lastHovered.Unhighlight();

                    }
                    if(hoveredItemInfo != clickedItemInfo)
                    {
                        hoveredItemInfo.HighlightHover();
                        lastHovered = hoveredItemInfo;
                    }
                }
            }
        }
        else
        {
            hoveredGameObject = null;
            hoveredItemInfo = null;

            if (lastHovered != null && lastHovered != clickedItemInfo)
            {
                lastHovered.Unhighlight();
                lastHovered = null;
            }
        }
    }
    private void HandleClick()
    {
        if (placementSystem.IsBuilding() == true)
        {
            return;
        }
        if (hoveredItemInfo != null)
        {
            if (clickedItemInfo != null)
            {
                clickedItemInfo.Unhighlight();
            }
            clickedItemInfo = hoveredItemInfo;
            clickedItemInfo.HighlightClicked();
        }
        else
        {
            if(clickedItemInfo != null)
            {
                clickedItemInfo.Unhighlight();
                clickedItemInfo = null;
            }
        }
    }

    public void HandleRemove()
    {
        if(clickedItemInfo != null)
        {
            placementSystem.RemoveItem(CurrentSelected);
            clickedItemInfo = null;
        }

    }

    public void HandleMove()
    {
        if (clickedItemInfo != null)
        {
            placementSystem.StartMoving(CurrentSelected.item, CurrentSelected);
            clickedItemInfo = null;
        }
    }
    private void OnEnable()
    {
        inputManager.OnClicked += HandleClick;
        selectedItem.OnRemoveClicked += HandleRemove;
        selectedItem.OnMovingClicked += HandleMove;
    }
    private void OnDisable()
    {
        inputManager.OnClicked -= HandleClick;
        selectedItem.OnRemoveClicked -= HandleRemove;
        selectedItem.OnMovingClicked -= HandleMove;
    }
}
