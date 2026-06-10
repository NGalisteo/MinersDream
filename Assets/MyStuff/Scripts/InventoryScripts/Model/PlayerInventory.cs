using System;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Model
{
    [CreateAssetMenu]
    public class PlayerInventory : ScriptableObject
    {
        private Dictionary<ItemSO, int> ownedItems = new Dictionary<ItemSO, int>();

        public event Action OnInventoryChanged;

        public void AddItem(ItemSO item, int quantity)
        {

            if (ownedItems.ContainsKey(item))
            {
                ownedItems[item] += quantity;
            }
            else
            {
                ownedItems.Add(item, quantity);
            }
            OnInventoryChanged?.Invoke();
        }

        public int RemoveItem(ItemSO item)
        {
            if (ownedItems.ContainsKey(item))
            {
                ownedItems[item] -= 1;
                OnInventoryChanged?.Invoke();
                if (ownedItems[item] <= 0)
                {
                    ownedItems.Remove(item);
                    return 0;
                }
                return ownedItems[item];
            }
            return 0;
        }

        public Dictionary<ItemSO, int> GetCurrentItems()
        {
            return ownedItems;
        }

    }

}
