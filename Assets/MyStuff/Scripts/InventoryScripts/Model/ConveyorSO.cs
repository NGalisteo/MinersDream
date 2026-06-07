using Inventory.Model;
using UnityEngine;

[CreateAssetMenu]
public class ConveyorSO : ItemSO
{
    [field: SerializeField]
    public float Speed {  get; set; }
}
