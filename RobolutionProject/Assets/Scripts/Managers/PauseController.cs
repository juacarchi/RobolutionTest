using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    GameState statePreviousPause;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameState currentGameState = GameStateManager.Instance.CurrentGameState;
            GameState newGameState=currentGameState;
            switch (currentGameState)
            {
                case GameState.Gameplay:
                    statePreviousPause = currentGameState;
                    newGameState = GameState.Paused;
                    break;
                case GameState.Bonus:
                    statePreviousPause = currentGameState;
                    newGameState = GameState.Paused;
                    break;
                case GameState.Paused:
                    newGameState = statePreviousPause;
                    break;
        }
            GameStateManager.Instance.SetState(newGameState);
               
            }
    }
}
