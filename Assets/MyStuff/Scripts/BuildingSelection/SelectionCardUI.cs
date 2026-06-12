using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectionCardUI : MonoBehaviour
{
    [SerializeField]
    private GameObject selectionCard;
    [SerializeField]
    private SelectionSystem selectionSystem;
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private TextMeshProUGUI selectionText;
    [SerializeField]
    private Button removeButton;

    public event Action OnRemoveClicked;


    private void Awake()
    {
        removeButton.onClick.AddListener(HandleRemoveClick);
    }
    private void Update()
    {
        if (selectionSystem.CurrentSelected != null)
        {
            selectionCard.SetActive(true);
            selectionText.text = selectionSystem.CurrentSelected.item.Name;
            BoxCollider selectedCollider = selectionSystem.CurrentSelected.GetComponent<BoxCollider>();
            selectionCard.transform.position = selectedCollider.bounds.center;
            selectionCard.transform.rotation = mainCamera.transform.rotation;
        }
        else
        {
            selectionCard.SetActive(false);
        }
    }

    private void HandleRemoveClick()
    {
        OnRemoveClicked?.Invoke();
    }
}
