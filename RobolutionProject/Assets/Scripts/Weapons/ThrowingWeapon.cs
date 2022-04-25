using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Throwing Weapon", menuName = "Weapon/ThrowingWeapon")]
public class ThrowingWeapon : Weapon
{
    public void Awake()
    {
        TypeWeapon = WeaponType.ThrowingWeapon;
    }
}
