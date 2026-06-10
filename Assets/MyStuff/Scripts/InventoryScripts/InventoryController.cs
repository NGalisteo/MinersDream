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

        [SerializeField]
        ItemSO IronMineSO;
        [SerializeField]
        ItemSO RubyMineSO;

        private PlayerInputActions action; // for input actions, new input system



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
            inventoryData.AddItem(IronMineSO, 5);
            inventoryData.AddItem(RubyMineSO, 5);
            inventoryData.OnInventoryChanged += UpdateInventoryUI;
        }

        private void UpdateInventoryUI()
        {
            Dictionary<ItemSO, int> currentInventory = inventoryData.GetCurrentItems();
            inventoryUI.ResetAllItems();
            foreach (var item in currentInventory)
            {
                inventoryUI.UpdateData(item.Key, item.Key.ItemImage, item.Value);
            }

        }

        private void PrepareUI()
        {
            inventoryUI.InitializeInventoryUI(inventoryData);
        }

        private void HandlePlacementCall(ItemSO item)
        {
            placementSystem.StartPlacement(item);
            inventoryUI.Hide();
        }

        public void Update()
        {
            if (action.Player.OpenInventory.WasPressedThisFrame())
            {
                if (inventoryUI.isActiveAndEnabled == false)
                {
                    inventoryUI.Show();
                    foreach (var item in inventoryData.GetCurrentItems())
                    {
                        inventoryUI.UpdateData(item.Key,
                            item.Key.ItemImage,
                            item.Value);
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