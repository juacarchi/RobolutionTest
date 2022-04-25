using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField]
    float timeToDestroy;
    void Start()
    {
        Destroy(this.gameObject, timeToDestroy);
    }
    
}
