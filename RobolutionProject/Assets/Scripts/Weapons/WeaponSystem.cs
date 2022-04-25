using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    public static WeaponSystem instance;
    public List<int> ammoWeapon;
    public float timeReload = 1;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    private void Start()
    {
        for (int i = 0; i < GameManager.instance.weaponsInPossesion.Count; i++)
        {
            if (GameManager.instance.weaponsInPossesion[i] != null)
            {
                UIManager.instance.AddWeaponHUD(GameManager.instance.weaponsInPossesion[i]);
            }
        }
        GameManager.instance.SetActualWeapon(0);
    }
 
    public void UpdateAmmo(int weaponSelected, int ammo)
    {
        ammoWeapon[weaponSelected] = ammo;
    }
    public IEnumerator ReloadWeapon(int ammoPos)
    {
        for (float i = 1; i > 0; i -= Time.deltaTime)
        {
            UIManager.instance.ammoDict[ammoPos].fillAmount = i;
            yield return null;
        }
        UIManager.instance.ammoDict[ammoPos].fillAmount = 0;
        yield return new WaitForSecondsRealtime(timeReload);
    }
}
