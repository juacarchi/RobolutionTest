using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class RoomManager : MonoBehaviour
{
    public GameObject createdGameObject;
    public static RoomManager instance;
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
        GameManager.instance.InstantiateStage();
    }
    public void InstantiateStage(GameObject stage)
    {
        if (createdGameObject != null)
        {
            Destroy(createdGameObject.gameObject);
            System.GC.Collect();
            UnityEngine.Scripting.GarbageCollector.CollectIncremental();


        }
        GameManager.instance.navMeshCreated = false;

        GameObject stageInstantiated = Instantiate(stage, transform.position, Quaternion.identity);
        createdGameObject = stageInstantiated;
        GameplayManager.instance.SetCheckEnemiesInRoom(true);
        
    }

}
