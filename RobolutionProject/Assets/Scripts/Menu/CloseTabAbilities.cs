using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseTabAbilities : MonoBehaviour
{
    public GameObject panel;
    public TabGroup tabGroup;

    //MODIFICAR M�TODO PARA VARIAR ICONOS
    public void Close()
    {
        panel.SetActive(false);
    }
}
