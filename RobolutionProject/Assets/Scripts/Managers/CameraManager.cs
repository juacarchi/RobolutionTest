using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    GameObject mainCamera;
    [SerializeField]
   GameObject cameraBonus;
    private void Awake()
    {
        GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;
        mainCamera.SetActive(true);
        cameraBonus.SetActive(false);
    }
    private void OnGameStateChanged(GameState newGameState)
    {
        switch (newGameState)
        {
            case GameState.Bonus:
                Invoke("ActiveCameraBonus", 2f);
                break;
            case GameState.Gameplay:
                Invoke("ActiveMainCamera", 2f);
                break;
            case GameState.Paused:
                break;
        }
        
    }
    public void ActiveCameraBonus()
    {
        mainCamera.SetActive(false);
        cameraBonus.SetActive(true);
        
    }
    public void ActiveMainCamera()
    {
        cameraBonus.SetActive(false);
        mainCamera.SetActive(true);
    }
    private void OnDestroy()
    {
        GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }
    
}
