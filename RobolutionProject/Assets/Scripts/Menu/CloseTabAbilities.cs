using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseTabAbilities : MonoBehaviour
{
    public GameObject panel;
    public TabGroup tabGroup;

    //MODIFICAR MÉTODO PARA VARIAR ICONOS
    public void Close()
    {
        panel.SetActive(false);
    }
}
