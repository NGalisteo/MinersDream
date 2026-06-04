using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField]
    private UIInventoryPage inventoryUI;

    public int inventorySize = 10;

    private void Start()
    {
        inventoryUI.InitializeInventoryUI(inventorySize);
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))//change this later to E, also change to new input actions, need reminder on how to do it.
        {
            if(inventoryUI.isActiveAndEnabled == false )
            {
                inventoryUI.Show();
            }
            else
            {
                inventoryUI.Hide();
            }
        }
    }
}
