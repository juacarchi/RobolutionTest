using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    public GameObject pointSpawn;

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {

        if (GameplayManager.instance.GetLevelCompleted())
        {
            if (other.CompareTag("Player"))
            {
                //GameManager.instance.levelNumber++;
                //GameManager.instance.InstantiateStage();
                //other.gameObject.transform.position = pointSpawn.transform.position;
                GameObject bonusRoom = GameObject.FindGameObjectWithTag("BonusRoom");
                Destroy(bonusRoom);
                GameplayManager.instance.InstantiateBonusRoom();
                StartCoroutine(UIManager.instance.FadeImageToBlack(other.gameObject,pointSpawn.transform,GameState.Gameplay,true));

            }
      
        }
    }

}
