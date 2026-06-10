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
            PrepareInventoryData();
            PrepareUI();
        }

        private void PrepareInventoryData()
        {
            inventoryData.AddItem(IronMineSO, 5);
            inventoryData.AddItem(RubyMineSO, 5);
            inventoryData.OnInventoryChanged += UpdateInventoryUI;
        }

        private void PrepareUI()
        {
            inventoryUI.OnItemClicked += HandlePlacementCall;
        }
        private void UpdateInventoryUI()
        {
            Dictionary<ItemSO, int> currentInventory = inventoryData.GetCurrentItems();
            inventoryUI.FillInventory(currentInventory);
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
                    UpdateInventoryUI();
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