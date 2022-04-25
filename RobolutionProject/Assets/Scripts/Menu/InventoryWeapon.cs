using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryWeapon : MonoBehaviour
{
    public static InventoryWeapon instance;
    private void Awake()
    {
        instance = this;
    }

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    public int space = 10; //Cantidad de items espacios
    [HideInInspector]
    public List<Weapon> weaponItems;

    private void Start()
    {
        weaponItems = GameManager.instance.allWeapons;
    }

    

}
