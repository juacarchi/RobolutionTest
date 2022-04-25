using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusCoin : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            BonusRoom.instance.RecollectBonusCoin();
            Destroy(this.gameObject);
        }
    }
}
