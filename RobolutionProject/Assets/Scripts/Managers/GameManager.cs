using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum PlayerType
{
    Tracking, //Misil teledirigido
    Shield, //Escudo absorbe una gran cantidad de daño
    ChainedBullet //Los disparos podrán rebotar
}

public class GameManager : MonoBehaviour
{
    public int levelNumber;
    public int worldNumber;
    public List<List<GameObject>> stageWorld = new List<List<GameObject>>();
    public List<GameObject> world1 = new List<GameObject>();
    public List<GameObject> world2 = new List<GameObject>();
    public GameObject actualStage;
    public static GameManager instance;
    bool gameStart;
    GameObject player;
    [SerializeField]
    PlayerType playerType;
    int gold;
    int chips;
    public GameObject damagePopUp;
    Weapon actualWeapon;
    public SpriteRenderer targetCircunference;

    public GameObject circleCircunference; //Circunferencia que marca el rango de cada arma para detectar enemigos

    public int numberWeaponSpace;
    public List<Weapon> weaponsInPossesion;
    public List<Weapon> allWeapons;
    Vector3 move;
    public bool navMeshCreated;
    private void Awake()
    {
        //Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        stageWorld.Add(world1);
        stageWorld.Add(world2);
        Application.targetFrameRate = 120;

    }
    private void Start()
    {

    }
    public void SetActualStage(GameObject stage)
    {
        actualStage = stage;
    }
    public PlayerType GetPlayerType()
    {
        return playerType;
    }
    public void SetPlayerType(int i)
    {
        if (i == 0)
        {
            playerType = PlayerType.Tracking;
        }
        else if (i == 1)
        {
            playerType = PlayerType.Shield;
        }
        else if (i == 2)
        {
            playerType = PlayerType.ChainedBullet;
        }
    }
    public Vector3 GetMovePlayer()
    {
        return move;
    }
    public void SetMovePlayer(Vector3 move)
    {
        this.move = move;
    }
    public void SetCircleCircunferenceRadius(Vector3 scale) //Método llamado para variar radio de la circunferencia
    {
        circleCircunference.transform.localScale = scale;
    }
    public void SetTargetCircunferencePosition(Vector3 position) //Método llamado para modificar posicion marca del target
    {
        targetCircunference.transform.position = position;
    }
    public void SetPlayer(GameObject player) // El propio jugador se asigna en su script.
    {
        this.player = player;
    }
    public GameObject GetPlayer()
    {
        return player;
    }
    public void SetActualWeapon(int selectedWeapon)
    {
        actualWeapon = weaponsInPossesion[selectedWeapon];
        PlayerController.instance.WeaponChanged(selectedWeapon);
    }
    public void SetGameStart(bool isActive)
    {
        gameStart = isActive;

    }
    public bool GetGameStart()
    {
        return gameStart;
    }
    public Weapon GetActualWeapon()
    {
        return actualWeapon;
    }
    public void SetWeaponsInPossesion(int pos, Weapon weaponChanged)
    {
        weaponsInPossesion[pos] = weaponChanged;
    }
    public void NextWeapon(int selectedWeapon)
    {
        StartCoroutine(WeaponSystem.instance.ReloadWeapon(selectedWeapon));
        selectedWeapon++;
        Debug.Log(weaponsInPossesion.Count);
        if (weaponsInPossesion[selectedWeapon] == null)
        {
            selectedWeapon = 0;
        }
        UIManager.instance.SetActiveWeapon(selectedWeapon);


        //bool weaponChanged = false;
        //for (int i = selectedWeapon ; i < weaponsInPossesion.Count; i++)
        //{
        //    if (weaponsInPossesion[i] != null)
        //    {
        //        if (weaponsInPossesion[i].isCharged)
        //        {
        //            UIManager.instance.SetActiveWeapon(i);
        //            weaponChanged = true;
        //            break;
        //        }
        //    }
        //}
        //if (!weaponChanged)
        //{
        //    for (int i = 0; i < selectedWeapon; i++)
        //    {
        //        if (weaponsInPossesion[i] != null)
        //        {
        //            if (weaponsInPossesion[i].isCharged)
        //            {
        //                Debug.Log("Arma cambiada");
        //                UIManager.instance.SetActiveWeapon(i);
        //                weaponChanged = true;
        //                break;
        //            }
        //        }
        //    }
        //    if (!weaponChanged)
        //    {
        //        PlayerController.instance.Reload();
        //    }

        //}

    }
    public int GetGold()
    {
        return gold;
    }
    public void SetGold(int gold)
    {
        this.gold = gold;
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateGoldText();
        }
    }
    public int GetChips()
    {
        return chips;
    }
    public void SetChips(int chips)
    {
        this.chips = chips;
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateChipText();
        }
    }

    public void InstantiateStage()
    {
        List<GameObject> listaMundo = stageWorld[worldNumber];
        if (listaMundo.Count > levelNumber)
        {
            if (levelNumber == 3 || levelNumber == 7)
            {
                SceneManager.LoadScene(0);
                levelNumber++;
            }
            else
            {
                RoomManager.instance.InstantiateStage(listaMundo[levelNumber]);
                actualStage = listaMundo[levelNumber];
            }

        }
        else
        {
            Debug.Log(listaMundo.Count);
            Debug.Log(levelNumber);
            levelNumber = 0;

            SceneManager.LoadScene(3);
            Debug.Log("LevelCompleted");
        }
    }

}
