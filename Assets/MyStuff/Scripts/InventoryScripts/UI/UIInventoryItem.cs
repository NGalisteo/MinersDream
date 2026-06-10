using Inventory.Model;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Inventory.UI
{
    public class UIInventoryItem : MonoBehaviour
    {
        [SerializeField]
        private Image itemImage;
        [SerializeField]
        private TMP_Text quantityTxt;

        public event Action<ItemSO> OnItemSelected;

        private ItemSO storedItem;

        private void Awake()
        {
            Button myButton = GetComponent<Button>();
            myButton.onClick.AddListener(HandleClick);
        }
        public void SetData(ItemSO item, int quantity)
        {
            itemImage.sprite = item.ItemImage;
            quantityTxt.text = quantity.ToString();
            storedItem = item;
        }

        private void HandleClick()
        {
            OnItemSelected?.Invoke(storedItem);
        }
    }
}