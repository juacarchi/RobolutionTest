using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Menu : MonoBehaviour
{
public void Play()
    {
        System.GC.Collect();
        SceneManager.LoadScene(1);

    }
    public void PlayCineMachine()
    {
        System.GC.Collect();
        SceneManager.LoadScene(4);
    }
    public void GoToMenu()
    {
        System.GC.Collect();
        SceneManager.LoadScene(0);
    }
}
