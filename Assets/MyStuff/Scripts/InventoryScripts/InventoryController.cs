using Inventory.Model;
using Inventory.UI;
using System.Collections.Generic;
using UnityEngine;


namespace Inventory
{
    public class InventoryController : MonoBehaviour
    {
        [SerializeField]
        private UIInventoryPage inventoryUI;

        [SerializeField]
        private PlayerInventory inventoryData;

        [SerializeField]
        private PlacementSystem placementSystem;

        private PlayerInputActions action; // for input actions, new input system

        public List<InventorySlot> initialItems = new List<InventorySlot>();


        private void Awake()
        {
            action = new PlayerInputActions(); // just the enable for inputs, always put on awake
        }
        private void Start()
        {
            PrepareUI();
            PrepareInventoryData();

        }

        private void PrepareInventoryData()
        {
            inventoryData.Initialize();
            inventoryData.OnInventoryUpdated += UpdateInventoryUI;
            foreach (InventorySlot item in initialItems)
            {
                if (item.isEmpty)
                    continue;
                inventoryData.AddItem(item);
            }
        }

        private void UpdateInventoryUI(Dictionary<int, InventorySlot> inventoryState)
        {
            inventoryUI.ResetAllItems();
            foreach (var item in inventoryState)
            {
                inventoryUI.UpdateData(item.Key, item.Value.item.ItemImage, item.Value.quantity);
            }

        }

        private void PrepareUI()
        {
            inventoryUI.InitializeInventoryUI(inventoryData.Size);
            inventoryUI.OnDescriptionRequested += HandlePlacementCall;
            inventoryUI.OnSwapItems += HandleSwapItems;
            inventoryUI.OnStartDragging += HandleDragging;
            inventoryUI.OnItemActionRequested += HandleItemActionRequest;
        }

        private void HandleItemActionRequest(int itemIndex)
        {
        }

        private void HandleDragging(int itemIndex)
        {
            InventorySlot inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.isEmpty)
                return;
            inventoryUI.CreateDraggedItem(inventoryItem.item.ItemImage, inventoryItem.quantity);
        }

        private void HandleSwapItems(int itemIndex_1, int itemIndex_2)
        {
            inventoryData.SwapItems(itemIndex_1, itemIndex_2);
        }

        private void HandlePlacementCall(int itemIndex)
        {
            InventorySlot inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.isEmpty)
            {
                inventoryUI.ResetSelection();
                return;
            }
            ItemSO item = inventoryItem.item;

            placementSystem.StartPlacement(item);
            inventoryUI.Hide();

        }

        public void Update()
        {
            if (action.Player.OpenInventory.WasPressedThisFrame())//change this later to E, also change to new input actions, need reminder on how to do it.
            {
                if (inventoryUI.isActiveAndEnabled == false)
                {
                    inventoryUI.Show();
                    foreach (var item in inventoryData.GetCurrentInventoryState())
                    {
                        inventoryUI.UpdateData(item.Key,
                            item.Value.item.ItemImage,
                            item.Value.quantity);
                    }
                }
                else
                {
                    inventoryUI.Hide();
                }
            }
        }
        private void OnEnable() //start listening for input
        {
            action.Enable();
        }

        private void OnDisable() //stops listening for input
        {
            action.Disable();
        }
    }
}