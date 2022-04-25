using UnityEngine;
using UnityEngine.UI;
public class ChestBonus : MonoBehaviour
{
    Transform spawnPointPlayer;
    public float timeToOpen;
    float timer;
    public float radius;
    [SerializeField]
    LayerMask playerMask;
    ChestState chestState;
    bool playerInRange;
    public Image chargeSlider;
    private void Awake()
    {
        chestState = ChestState.Closed;
    }
    private void Start()
    {
        spawnPointPlayer = GameObject.FindGameObjectWithTag("Respawn").transform;
    }
    private void Update()
    {
        playerInRange = Physics.CheckSphere(this.transform.position, radius, playerMask);
        if (playerInRange && chestState != ChestState.Open)
        {
            Opening();
        }
    }
    public void Closed()
    {
        chestState = ChestState.Closed;
    }
    public void Opening()
    {
        CheckProgress();
        if (chargeSlider.fillAmount >= 1)
        {
            chestState = ChestState.Open;
            Open();
        }
    }
    public void Open()
    {
        int rnd = Random.Range(0, LootManager.instance.lootItems.Count);
        ObjectPooler.instance.SpawnFromPool(LootManager.instance.lootItems[rnd], transform.position, Quaternion.Euler(-33, 180, 0));
        Invoke("EndBonusRoom", 0.5f);
    }
    public void CheckProgress()
    {
        chargeSlider.fillAmount += 1.0f / timeToOpen * Time.deltaTime;
    }
    public void EndBonusRoom()
    {
        StartCoroutine(UIManager.instance.FadeImageToBlack(GameManager.instance.GetPlayer(), spawnPointPlayer, GameState.Gameplay, false));
    }
}
