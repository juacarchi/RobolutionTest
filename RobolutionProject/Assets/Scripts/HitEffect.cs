using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : MonoBehaviour, IPooledObject
{
    public ParticleSystem pSystem;
    public void OnObjectSpawn()
    {
        pSystem.Play();
        Invoke("DesactivateGO", pSystem.main.duration);
    }
    public void DesactivateGO()
    {
        this.gameObject.SetActive(false);
    }
}
