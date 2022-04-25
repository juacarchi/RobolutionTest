using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryWeaponSlot : MonoBehaviour
{
    public Button thisButton;
    public Image icon;
    Weapon weaponItem;

    
    public void AddWeapon(Weapon newWeapon)
    {
        if (newWeapon != null)
        {
            weaponItem = newWeapon;
            icon.enabled = true;
            icon.sprite = weaponItem.SpriteWeapon;
        }
        else
        {
            weaponItem = null;
            icon.sprite = null;
            icon.enabled = false;
        }
        
    }
    public void ClearSlot()
    {
        weaponItem = null;
        icon.sprite = null;
        icon.enabled = false;
    }
    public Weapon GetWeapon()
    {
        return weaponItem;
    }
    
    public void CheckInteractable()
    {
        thisButton.interactable = true;
        for (int i = 0; i < GameManager.instance.weaponsInPossesion.Count; i++)
        {
            if (GameManager.instance.weaponsInPossesion[i] == weaponItem)
            {
                thisButton.interactable = false;
            }
        }
    }
}

