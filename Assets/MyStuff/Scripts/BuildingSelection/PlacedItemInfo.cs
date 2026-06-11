using Inventory.Model;
using UnityEngine;

public class PlacedItemInfo : MonoBehaviour
{
    public ItemSO item;
    public Vector3Int gridPosition;
    public int trackingNumber;
    [SerializeField]
    private GameObject selectionBox;

    [SerializeField]
    private Material hoveredMaterial;
    [SerializeField]
    private Material clickedMaterial;

    Renderer selectionBoxRenderer;

    private void Awake()
    {
        selectionBoxRenderer = selectionBox.GetComponent<Renderer>();
    }
    public void HighlightHover()
    {
        selectionBox.SetActive(true);
        selectionBoxRenderer.material = hoveredMaterial;
    }
    public void HighlightClicked()
    {
        selectionBox.SetActive(true);
        selectionBoxRenderer.material = clickedMaterial;
    }
    public void Unhighlight()
    {
        selectionBox.SetActive(false);
    }
}
