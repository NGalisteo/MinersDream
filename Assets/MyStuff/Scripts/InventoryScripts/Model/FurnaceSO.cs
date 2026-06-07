using Inventory.Model;
using UnityEngine;

[CreateAssetMenu]
public class FurnaceSO : ItemSO
{
    [field: SerializeField]
    public float ConversionRate { get; set; }
}
