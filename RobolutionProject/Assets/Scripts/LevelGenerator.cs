using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public Transform[] spawnPointsPortal;
    GameObject portal;
    public float minTimeSpawn=10;
    public float maxTimeSpawn=18;
    float timeToSpawn;
    public float timeToDisappear = 8;
    bool spawned;
    bool startedGame;
    int randomTransform;
    BoxCollider portalCollider;
    void Start()
    {
        timeToSpawn = Random.Range(minTimeSpawn, maxTimeSpawn);
        randomTransform = Random.Range(0, spawnPointsPortal.Length);
        portal = GameplayManager.instance.portal;
        portalCollider = portal.GetComponent<BoxCollider>();
        portalCollider.enabled = true;
    }

    void Update()
    {

        if (!spawned && GameManager.instance.GetGameStart())
        {
            timeToSpawn -= Time.deltaTime;
            if (timeToSpawn <= 0)
            {
                portal.transform.position = spawnPointsPortal[randomTransform].position;
                portal.SetActive(true);
                spawned = true;
            }
        }
        else if(spawned)
        {
            if (GameStateManager.Instance.CurrentGameState == GameState.Gameplay)
            {
                timeToDisappear -= Time.deltaTime;
                if (timeToDisappear <= 0)
                {
                    portalCollider.enabled = false;
                    Invoke("DesactivatePortal", 3f);
                }
            }
          
        }
    }
    void DesactivatePortal()
    {
        portal.SetActive(false);
    }
}
