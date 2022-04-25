using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalExit : MonoBehaviour
{
    public Transform spawnPoint;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(UIManager.instance.FadeImageToBlack(other.gameObject, spawnPoint, GameState.Gameplay,false));

        }
    }
}
