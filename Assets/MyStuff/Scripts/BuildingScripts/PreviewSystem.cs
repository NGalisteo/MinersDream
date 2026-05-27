using System;
using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField]
    private float previewYOffset = 0.06f; //to avoid z fighting, makes the preview float slightly above groound.

    [SerializeField]
    private GameObject cellIndicator; //visual highlight on the ground
    private GameObject previewObject; //ghost copy of the object of the object we're about to place, created at runtime.

    [SerializeField]
    private Material previewMaterialPrefab; //transparent material i crated in assets
    private Material previewMaterialInstance; //runtime copy of it, so we can modify the runtime one and not the normal one, since if we do that it would modify the object permanently.

    private Renderer cellIndicatorRenderer; //stores a reference to cellindicator renderer components so we can change the color without calling getcomponent every frame.


    private void Start()
    {
        previewMaterialInstance = new Material(previewMaterialPrefab); //creates the runtime copy of the material.
        cellIndicator.SetActive(false);// hdies the indicator at the start, since it should be only active during build mode.
        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>(); //finds the renderer component inside cellIndicator and stores it
    }

    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size) //called by placementstate when we enter build mode, 
    {
        previewObject = Instantiate(prefab); //spawns the ghost
        PreparePreview(previewObject);//applies transparent material
        PrepareCursor(size); //resizes the cell indicator to match item size
        cellIndicator.SetActive(true); //shows the cell indicator
    }

    private void PrepareCursor(Vector2Int size) //resizes the cell indicator to match the items size.
    {
        if(size.x > 0 || size.y > 0)
        {
            cellIndicator.transform.localScale = new Vector3(size.x, 1, size.y); //scales the indicator on x and z to match the items size, we dont scale vertically so Y is 1.
            cellIndicatorRenderer.material.mainTextureScale = size;//scales the texture on the indictor to tile correctly. without this the texture would just stretch
        }
    }

    private void PreparePreview(GameObject previewObject) //replaces every material on the ghost preview object with the transparent preview material.
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>(); //gets every renderer on the object and all its children.
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials; //returns a copy of the materials array, not a direct reference
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = previewMaterialInstance; //we modify the copy with the transparent materials.
            }
            renderer.materials = materials; //we reassign it back and apply the materials
        }
    }

    public void StopShowingPreview() 
    {
        cellIndicator.SetActive(false); //hides the cell indicator
        if(previewObject != null )
            Destroy(previewObject); //destroys ghost object
    }

    public void UpdatePosition(Vector3 position, bool validity) //called every time the mouse moves to a new cell
    {
        if(previewObject != null)
        {
            MovePreview(position); //moves the preview to the new pos
            ApplyFeedbackToPreview(validity);//updates colors
        }

        MoveCursor(position); //moves the cell indicator
        ApplyFeedbackToCursor(validity); //updates if valid and colors
    }

    private void ApplyFeedbackToPreview(bool validity)
    {
        Color c = validity ? Color.white : Color.red;

        c.a = 0.5f;
        previewMaterialInstance.color = c;
    }

    private void ApplyFeedbackToCursor(bool validity)
    {
        Color c = validity ? Color.white : Color.red; //if validity is true use white, if not, red.

        c.a = 0.5f; //sets alpha to 50%
        cellIndicatorRenderer.material.color = c; //Since all sub-meshes share the same previewMaterialInstance, changing it once updates all of them simultaneously — that's the payoff of using a shared material instance.
    }

    private void MoveCursor(Vector3 position)
    {
        cellIndicator.transform.position = position; //moves the cursor to the new pos.
    }

    private void MovePreview(Vector3 position)
    {
        previewObject.transform.position = new Vector3(position.x,
            position.y + previewYOffset,
            position.z); //adds the previewYoffset to Y so the ghost floats above grounds preventing zfighting, also moves the preview to the new pos.
        /*
         why not just do previewObject.transform.position = position and add the offset separately? Because position is a Vector3 and you can't just do position.y += previewYOffset directly — Vector3 components are read only when accessed that way. So instead a brand new Vector3 is constructed with the offset already baked into Y, and that gets assigned all at once.
         */
    }

    internal void StartShowingRemovePreview() //called by removingstate. no ghost created cos its removing
    {
        cellIndicator.SetActive(true); //shows cell indicator
        PrepareCursor(Vector2Int.one); //makes the cursor always 1x1 during remove mode
        ApplyFeedbackToCursor(false); //starts it red immediately.
    }
}
