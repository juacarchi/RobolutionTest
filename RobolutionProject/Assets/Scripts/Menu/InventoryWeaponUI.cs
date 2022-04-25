using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryWeaponUI : MonoBehaviour
{
    public Transform itemsParent;
    InventoryWeapon inventory;
    

    private void Start()
    {
        inventory = InventoryWeapon.instance;
        inventory.onItemChangedCallback += UpdateUI;
        UpdateUI();
        this.gameObject.SetActive(false);
    }
   
    private void UpdateUI()
    {

        InventoryWeaponSlot[] slots = itemsParent.GetComponentsInChildren<InventoryWeaponSlot>();

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.weaponItems.Count)
            {
                slots[i].AddWeapon(inventory.weaponItems[i]);
            }
        }
    }
}
