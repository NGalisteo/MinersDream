using Inventory.Model;
using UnityEngine;

[CreateAssetMenu]
public class UpgraderSO : ItemSO
{

    [field: SerializeField]
    public float Multiplier { get; set; }
}
