using System;
using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField]
    private float previewYOffset = 0.06f;

    [SerializeField]
    private GameObject cellIndicator;
    private GameObject previewObject;

    [SerializeField]
    private Material previewMaterialPrefab;
    private Material previewMaterialInstance;

    private Renderer cellIndicatorRenderer;
    private Vector2Int currentSize;

    private void Start()
    {
        previewMaterialInstance = new Material(previewMaterialPrefab);
        cellIndicator.SetActive(false);
        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }

    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size)
    {
        currentSize = size;
        previewObject = Instantiate(prefab);
        PreparePreview(previewObject);
        PrepareCursor(size);
        cellIndicator.SetActive(true);
    }

    private void PrepareCursor(Vector2Int size)
    {
        if (size.x > 0 || size.y > 0)
        {
            cellIndicator.transform.localScale = new Vector3(size.x, 1, size.y);
            cellIndicatorRenderer.material.mainTextureScale = size;
        }
    }

    public GameObject GetPreviewInfo()
    {
        return previewObject;
    }
    private void PreparePreview(GameObject previewObject)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
                materials[i] = previewMaterialInstance;
            renderer.materials = materials;
        }
    }

    public void StopShowingPreview()
    {
        cellIndicator.SetActive(false);
        if (previewObject != null)
            Destroy(previewObject);
    }

    public void UpdatePosition(Vector3 position, bool validity)
    {
        if (previewObject != null)
        {
            MovePreview(position);
            ApplyFeedbackToPreview(validity);
        }
        MoveCursor(position);
        ApplyFeedbackToCursor(validity);
    }

    private void ApplyFeedbackToPreview(bool validity)
    {
        Color c = validity ? Color.white : Color.red;
        c.a = 0.5f;
        previewMaterialInstance.color = c;
    }

    private void ApplyFeedbackToCursor(bool validity)
    {
        Color c = validity ? Color.white : Color.red;
        c.a = 0.5f;
        cellIndicatorRenderer.material.color = c;
    }

    // Cursor stays at corner — its prefab has a (0.5, 0, 0.5) offset on the child that auto-scales
    private void MoveCursor(Vector3 position)
    {
        cellIndicator.transform.position = new Vector3(
            position.x + currentSize.x / 2f,
            position.y,
            position.z + currentSize.y / 2f
        );
    }

    // Ghost goes to footprint center — matches where real item spawns
    private void MovePreview(Vector3 position)
    {
        previewObject.transform.position = new Vector3(
            position.x + currentSize.x / 2f,
            position.y + previewYOffset,
            position.z + currentSize.y / 2f
        );
    }

    public void RotatePreview()
    {
        previewObject.transform.Rotate(0, 90, 0);
    }

    internal void StartShowingRemovePreview()
    {
        cellIndicator.SetActive(true);
        PrepareCursor(Vector2Int.one);
        ApplyFeedbackToCursor(false);
    }

    // Helper used by PlacementState so real item spawns at same place as ghost
    public static Vector3 GetFootprintCenter(Vector3 cornerPosition, Vector2Int size)
    {
        return new Vector3(
            cornerPosition.x + size.x / 2f,
            cornerPosition.y,
            cornerPosition.z + size.y / 2f
        );
    }
}