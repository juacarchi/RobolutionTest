using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithPlayer : MonoBehaviour
{
    public GameObject player;
    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(player.transform.position.x,0.01f,player.transform.position.z);
    }
}
