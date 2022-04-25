using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePopUp : MonoBehaviour,IPooledObject
{
    public Camera cam;
    [SerializeField]
    Vector3 offset;
    private void Awake()
    {
        cam = Camera.main;
    }
    public void OnObjectSpawn()
    {
        offset = Vector3.up * 2;
        this.transform.position += offset;
        Invoke("DesactivatePopUp", 0.25f);
    }
    private void Update()
    {
        transform.LookAt(cam.transform);
    }
    void DesactivatePopUp()
    {
        this.gameObject.SetActive(false);
    }

  
}
