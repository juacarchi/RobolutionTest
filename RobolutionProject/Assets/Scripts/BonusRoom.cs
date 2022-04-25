using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
public class BonusRoom : MonoBehaviour
{
    Transform spawnPointPlayer;
    public static BonusRoom instance;
    public GameObject[] coinsRoom;
    public float timeBetweenChanges;
    public TMP_Text clockText;
    public float timeCountDown=15;
    bool startBonus;
    public GameObject chest;
    GameObject portal;
    int coinsRecollected;
    private void Awake()
    {
        instance = this;
        GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;
        chest.SetActive(false);
    }
    private void Start()
    {
        spawnPointPlayer = GameObject.FindGameObjectWithTag("Respawn").transform;
    }
    private void OnGameStateChanged(GameState newGameState)
    {
        if (newGameState == GameState.Bonus)
        {
            portal = GameplayManager.instance.portal;
            Invoke("StartClock", timeBetweenChanges);
        }
        else if(newGameState == GameState.Gameplay)
        {
            ResetRoom();
        }
    }
    public void ResetRoom()
    {
        startBonus = false;
        timeCountDown = 15; //Cambiar cuando tenga variable del tiempo;
        DisplayTime(timeCountDown);
    }
    public void RecollectBonusCoin()
    {
        coinsRecollected++;
        if (coinsRoom.Length == coinsRecollected && timeCountDown > 0)
        {
            Debug.Log("COMPLETADA SALA");
            startBonus = false; //Para parar contador;
            chest.SetActive(true);
        }
    }
    void Update()
    {
        if (startBonus)
        {
            if (timeCountDown > 0)
            {
                timeCountDown -= Time.deltaTime;
            }
            else
            {
                timeCountDown = 0;
                Debug.Log("Sala incompleta");
            }
            DisplayTime(timeCountDown);
        }
    }
    void DisplayTime(float timeToDisplay)
    {
        if (timeToDisplay < 0)
        {
            timeToDisplay = 0;
        }
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        float miliseconds = Mathf.FloorToInt((timeToDisplay*1000)%1000);
        clockText.text = string.Format("{0}:{1}", seconds, miliseconds);
    }
    public void StartClock()
    {
        startBonus = true;
        portal.SetActive(false);
    }
    private void OnDestroy()
    {
        GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }
    public void EndBonusRoom()
    {
        StartCoroutine(UIManager.instance.FadeImageToBlack(GameManager.instance.GetPlayer(), spawnPointPlayer, GameState.Gameplay, false));
    }
}
