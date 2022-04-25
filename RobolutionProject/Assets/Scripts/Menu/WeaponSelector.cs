using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class WeaponSelector : MonoBehaviour
{
    int weaponToChange;
    public List<Button> buttonsWeapons;
    public List<GameObject> panelWeapons;
    private void Start()
    {
        CheckWeaponInPosession();
        weaponToChange = 0;
    }
  
    public void CheckWeaponInPosession()
    {
        for (int i = 0; i < buttonsWeapons.Count; i++)
        {
            if (GameManager.instance.weaponsInPossesion[i] != null)
            {
                buttonsWeapons[i].image.sprite = GameManager.instance.weaponsInPossesion[i].SpriteWeapon;
            }
        }
    }
    public void WeaponToChange(int weaponToChange)
    {
        this.weaponToChange = weaponToChange;
        panelWeapons[weaponToChange].SetActive(true);
        InventoryWeaponSlot[] slots = FindObjectsOfType<InventoryWeaponSlot>();
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].CheckInteractable();
        }
    }
    public void WeaponSelected(int i)
    {
        if (GameManager.instance.allWeapons[i] != null)
        {
            Weapon weaponSelected = GameManager.instance.allWeapons[i];
            buttonsWeapons[weaponToChange].image.sprite = weaponSelected.SpriteWeapon;
            GameManager.instance.SetWeaponsInPossesion(weaponToChange, weaponSelected);
        }
        else
        {
            buttonsWeapons[weaponToChange].image.sprite = null;
            GameManager.instance.SetWeaponsInPossesion(weaponToChange, null);
        }
        for (int x = 0; x < panelWeapons.Count; x++)
        {
            panelWeapons[x].SetActive(false);
        }
    }
}
