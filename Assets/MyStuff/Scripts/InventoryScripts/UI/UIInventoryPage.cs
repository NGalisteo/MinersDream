using Inventory.Model;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory.UI
{
    public class UIInventoryPage : MonoBehaviour
    {
        [SerializeField]
        private GameObject itemListEntryPrefab;
        [SerializeField] 
        private Transform inventoryContentParent;

        private List<GameObject> currentInventoryItems = new List<GameObject>();

        public event Action<ItemSO> OnItemClicked;


        private void Awake()
        {
            Hide();
        }
        public void FillInventory(Dictionary<ItemSO, int> ownedItems)
        {
            foreach (GameObject item in currentInventoryItems)
            {
                Destroy(item);
            }
            currentInventoryItems.Clear();

            foreach (var item in ownedItems)
            {
                GameObject newEntry = Instantiate(itemListEntryPrefab,inventoryContentParent);
                currentInventoryItems.Add(newEntry);
                UIInventoryItem entryScript = newEntry.GetComponent<UIInventoryItem>();
                entryScript.SetData(item.Key, item.Value);
                entryScript.OnItemSelected += HandleItemClicked;
            }
        }
        private void HandleItemClicked(ItemSO item)
        {
            OnItemClicked?.Invoke(item);
        }
        public void Show()
        {
            gameObject.SetActive(true);
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}