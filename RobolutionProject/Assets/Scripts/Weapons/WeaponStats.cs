using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Weapon", menuName = "WeaponStats")]
public class WeaponStats :ScriptableObject
{
    public int damage;
    public float cadence;
    public float range;
    public float dispersion;
}
