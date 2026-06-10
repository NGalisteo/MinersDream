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
    private GameObject selectionBoxPrefab;

    [SerializeField]
    private PlacementSystem placementSystem;

    private GameObject selectionBox;

    private PlacedItemInfo selectedGameObjectInfo;

    private GameObject selectedGameObject;



    private void Start()
    {
        selectionBox = Instantiate(selectionBoxPrefab);
        selectionBox.SetActive(false);
    }
    private void Update()
    {
        if(placementSystem.IsBuilding() == true)
        {
            selectionBox.SetActive(false);
            return;
        }
        Vector2 mousePosition = inputManager.GetMousePosition();
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, itemLayerMask))
        {
            selectedGameObject = hit.collider.gameObject;
            selectedGameObjectInfo = selectedGameObject.GetComponent<PlacedItemInfo>();
            if (selectedGameObjectInfo != null && selectedGameObjectInfo.item != null)
            {
                Debug.Log($"tracknumber {selectedGameObjectInfo.trackingNumber}, item name {selectedGameObjectInfo.item.name}" +
                    $"grid pos {selectedGameObjectInfo.gridPosition}");
                selectionBox.SetActive(true);
                BoxCollider collider = selectedGameObject.GetComponent<BoxCollider>();
                selectionBox.transform.localScale = collider.bounds.size;
                selectionBox.transform.position = collider.bounds.center;
            }

        }
        else
        {
            selectedGameObject = null;
            selectedGameObjectInfo = null;
            selectionBox.SetActive(false);
        }
    }
}
