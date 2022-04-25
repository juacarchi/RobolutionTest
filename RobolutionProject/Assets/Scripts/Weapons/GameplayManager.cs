using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public SpriteRenderer targetCircunference;
    public static GameplayManager instance;
    public int enemiesInRoom;
    public bool levelCompleted; //Convertir en privada despues de testear
    bool checkEnemiesInRoom;
    public GameObject portal;
    public GameObject[] bonusRoom;
    private void Awake()
    {
        //Singleton
        instance = this;

        GameManager.instance.targetCircunference = targetCircunference;
    }
    private void Start()
    {
        int rnd = Random.Range(0, bonusRoom.Length);
        Instantiate(bonusRoom[rnd], transform.position, Quaternion.identity);
    }
    private void Update()
    {
        if (checkEnemiesInRoom)
        {
            if (enemiesInRoom <= 0)
            {
                StartCoroutine(WeaponSystem.instance.ReloadWeapon(PlayerController.instance.weaponSelected));
                levelCompleted = true;
                SetCheckEnemiesInRoom(false);
            }
            else
            {
                levelCompleted = false;
            }
        }
       
    }
    public void SetCheckEnemiesInRoom(bool check)
    {
        checkEnemiesInRoom = check;
        PlayerController.instance.SetCheckEnemies(check);
    }
    public bool GetLevelCompleted()
    {
        return levelCompleted;
    }
    public void AddEnemy()
    {
        enemiesInRoom++;
    }
    public void RemoveEnemy()
    {
        enemiesInRoom--;
    }
    public void InstantiateBonusRoom()
    {
        int rnd = Random.Range(0, bonusRoom.Length);
        Instantiate(bonusRoom[rnd], transform.position, Quaternion.identity);
    }
}
