using Inventory.Model;
using UnityEngine;

[CreateAssetMenu]
public class MineSO : ItemSO
{
    [field: SerializeField]
    public float OreValue { get; set; }

    [field: SerializeField]
    public float OreRate { get; set; }

    [field: SerializeField]
    public float OreSize { get; set; }
}
