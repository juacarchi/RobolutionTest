using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ChipsQuantity : MonoBehaviour
{
    public TMP_Text chipText;
    // Start is called before the first frame update
    void Start()
    {
        chipText.text = GameManager.instance.GetChips().ToString();
    }
}
