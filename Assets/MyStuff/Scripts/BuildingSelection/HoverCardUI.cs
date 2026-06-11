using TMPro;
using UnityEngine;

public class HoverCardUI : MonoBehaviour
{
    [SerializeField]
    private GameObject hoverCard;
    [SerializeField]
    private SelectionSystem selectionSystem;
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    TextMeshProUGUI hoverText;


    private void Update()
    {
        if(selectionSystem.CurrentHovered != null)
        {
            hoverCard.SetActive(true);
            hoverText.text = selectionSystem.CurrentHovered.item.Name;
            Debug.Log($"{hoverText.text}");
            BoxCollider hoveredCollider = selectionSystem.CurrentHovered.GetComponent<BoxCollider>();
            hoverCard.transform.position = hoveredCollider.bounds.center;
            hoverCard.transform.rotation = mainCamera.transform.rotation;
        }
        else
        {
            hoverCard.SetActive(false);
        }    
    }
}
