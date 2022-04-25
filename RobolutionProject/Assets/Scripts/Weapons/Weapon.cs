using UnityEngine;
using System.Collections.Generic;

public enum WeaponType
{
    NormalWeapon,
    ThrowingWeapon,
    MeleeWeapon
}
[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon")]
public abstract class Weapon : ScriptableObject
{
    public List<WeaponStats> weaponStats;
    public int currentLevel;
    [SerializeField] GameObject prefabWeapon;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] WeaponType typeWeapon;
    [SerializeField] Sprite spriteWeapon;
    [SerializeField] bool isCharged;
    [SerializeField] string weaponName;
    [SerializeField] string tag;
    [SerializeField] int ammo;
    [SerializeField] float timeReload;

    public GameObject PrefabWeapon { get => prefabWeapon; set => prefabWeapon = value; }
    public GameObject BulletPrefab { get => bulletPrefab; set => bulletPrefab = value; }
    public WeaponType TypeWeapon { get => typeWeapon; set => typeWeapon = value; }
    public Sprite SpriteWeapon { get => spriteWeapon; set => spriteWeapon = value; }
    public bool IsCharged { get => isCharged; set => isCharged = value; }
    public string WeaponName { get => weaponName; set => weaponName = value; }
    public string Tag { get => tag; set => tag = value; }
    public int Damage { get => weaponStats[currentLevel].damage; set => weaponStats[currentLevel].damage = value; }
    public float Cadence { get => weaponStats[currentLevel].cadence; set => weaponStats[currentLevel].cadence = value; }
    public int Ammo { get => ammo; set => ammo = value; }
    public float Range { get => weaponStats[currentLevel].range; set => weaponStats[currentLevel].range = value; }
    public float Dispersion { get => weaponStats[currentLevel].dispersion; set => weaponStats[currentLevel].dispersion = value; }
    public float TimeReload { get => timeReload; set => timeReload = value; }

    public void UpgradeLevel()
    {
        if (currentLevel < weaponStats.Count)
        {
            currentLevel++;
        }
    }
}

