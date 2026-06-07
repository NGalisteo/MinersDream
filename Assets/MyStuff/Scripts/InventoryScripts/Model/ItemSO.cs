using UnityEngine;

namespace Inventory.Model
{
    public enum ItemType
    {
        Dropper,
        Upgrader,
        Furnace,
        Conveyor,
        Decoration
    }


    [CreateAssetMenu]
    public class ItemSO : ScriptableObject
    {

        [field: SerializeField]
        public int ID { get; private set; } //id lmao\

        [field: SerializeField]
        public string Name { get; set; }

        [field: SerializeField]
        [field: TextArea]
        public string Description { get; set; }

        [field: SerializeField]
        public Vector2Int Size { get; private set; } = Vector2Int.one; //size, it defaults to 1x1 in case we dont put a size
       
        [field: SerializeField]
        public GameObject Prefab { get; private set; } //the prefab or model for this item.

        [field: SerializeField]
        public float Cost { get; private set; }

        [field: SerializeField]
        public int Tier { get; private set; }

        [field: SerializeField]
        public Sprite ItemImage { get; set; }

        [field: SerializeField]
        public ItemType ItemType { get; set; }

        [field: SerializeField]
        public int MaxStackSize { get; set; } = 1;

        [field: SerializeField]
        public bool IsStackable { get; set; }
    }
}
