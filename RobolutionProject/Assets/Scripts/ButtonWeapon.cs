using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ButtonWeapon : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] GameObject panelUpgrade;
    private void Start() //Solo lo comprueba cuando cambia de escena
    {
        if (GameManager.instance.GetChips() >= 1000)
        {
            button.interactable = true;
        }
        else
        {
            button.interactable = false;
        }
    }
    public void OpenPanel()
    {
        panelUpgrade.SetActive(true);
    }
}
