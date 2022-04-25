using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
   public Weapon sword;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController.instance.Hit(sword.Damage);
            this.gameObject.SetActive(false);
        }
    }
}
