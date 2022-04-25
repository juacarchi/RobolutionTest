using UnityEngine;
using UnityEngine.UI;
public enum ChestState
{
    Closed, //Emite ondas en círculo, siendo el mismo epicentro, del mismo color que el cofre
    Blocked, //Aparece icono de un candado hasta que no cumplas objetivo de la sala (Eliminar todos los enemigos)
    Opening, //Si está el jugador dentro del radio está abriendo el cofre. Barra de carga descendiendo
    Open //Cuando el cofre se abre libera objetos
}
public class Chest : MonoBehaviour
{
    public float timeToOpen;
    float timer;
    public float radius;
    [SerializeField]
    LayerMask playerMask;
    ChestState chestState;
    bool playerInRange;
    public GameObject padlock;
    public Image chargeSlider;
    private void Awake()
    {
        chestState = ChestState.Closed;
        padlock.SetActive(false);
        
    }
    private void Update()
    {
        playerInRange = Physics.CheckSphere(this.transform.position, radius, playerMask);
        if (GameplayManager.instance.GetLevelCompleted())
        {
            if (playerInRange && chestState != ChestState.Open)
            {
                Opening();
            }
        }
        else
        {
            if (playerInRange)
            {
                Blocked();
            }
            else
            {
                Closed();
            }
        }
    }
    public void Closed()
    {
        chestState = ChestState.Closed;
        padlock.SetActive(false);
    }
    public void Blocked()
    {
        chestState = ChestState.Blocked;
        padlock.SetActive(true);
    }
    public void Opening()
    {
        padlock.SetActive(false);
        CheckProgress();
       if (chargeSlider.fillAmount>=1)
        {
            chestState = ChestState.Open;
            Open();
        }
    }
    public void Open()
    {
        //Instanciar loot;
        int rnd = Random.Range(0, LootManager.instance.lootItems.Count);
        ObjectPooler.instance.SpawnFromPool(LootManager.instance.lootItems[rnd], transform.position, Quaternion.Euler(-33, 180, 0));
    }
    public void CheckProgress()
    {
        chargeSlider.fillAmount += 1.0f / timeToOpen * Time.deltaTime;

    }
}
