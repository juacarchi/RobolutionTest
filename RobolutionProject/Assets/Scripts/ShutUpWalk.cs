using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShutUpWalk : MonoBehaviour
{
    [SerializeField]
    AudioSource audioSource;
    private void Awake()
    {
        GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }
    private void OnDestroy()
    {
        GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }
    private void OnGameStateChanged(GameState newGameState)
    {
        //Cambiar audio de los pasos
        if (newGameState == GameState.Gameplay)
        {
            //audioSource.enabled = true;
            //audioSource.Play();
        }
        else
        {
            audioSource.Pause();
        }
    }
}
