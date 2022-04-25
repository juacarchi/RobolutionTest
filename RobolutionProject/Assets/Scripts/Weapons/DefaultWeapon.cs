using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Default Weapon",menuName ="Weapon/DefaultWeapon")]
public class DefaultWeapon : Weapon
{
    public void Awake()
    {
        TypeWeapon = WeaponType.NormalWeapon;
    }
}
