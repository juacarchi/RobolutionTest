using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Melee Weapon", menuName = "Weapon/Melee Weapon")]
public class MeleeWeapon : Weapon
{
    public void Awake()
    {
        TypeWeapon = WeaponType.MeleeWeapon;
    }
}
