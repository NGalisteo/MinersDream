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

    private PlacedItemInfo lastClicked;

    private GameObject hoveredGameObject;

    public PlacedItemInfo CurrentHovered {  get { return hoveredItemInfo; } }

    private void Start()
    {
    }
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
            Debug.Log($"{clickedItemInfo.item.name}");
        }
        else
        {
            if(clickedItemInfo != null)
            {
                clickedItemInfo.Unhighlight();
                clickedItemInfo = null;
                Debug.Log($"Deselected");
            }
        }
    }
    private void OnEnable()
    {
        inputManager.OnClicked += HandleClick;
    }
    private void OnDisable()
    {
        inputManager.OnClicked -= HandleClick;
    }
}
