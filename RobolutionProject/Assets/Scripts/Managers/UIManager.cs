using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject joystick;
    public Image imageFade;
    public float speedFade;
    public RectTransform panelIndicators;
    GameObject lootGO;
    public Image spriteLoot;
    public Image enemyMark;
    public Button superButton;
    public Image imageSuperButton;
    public static UIManager instance;
    List<Weapon> weaponsHUD;
    public List<Button> spaceWeapons;
    public List<Image> spaceWeaponsImages;
    public RectTransform UISelector;
    List<Text> weaponText;
    public Text superText;
    public Text goldText;
    public Text chipText;
    int space;
    public int selectedWeapon;
    bool changed;
    public Dictionary<int, Image> ammoDict = new Dictionary<int, Image>();
    public Image ammo1;
    public Image ammo2;
    public Image ammo3;
    public Image ammo4;
   

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        weaponsHUD = new List<Weapon>();
        for (int i = 0; i < spaceWeapons.Count; i++)
        {
            spaceWeapons[i].interactable = false;
        }
        selectedWeapon = 0;
        spriteLoot.enabled = false;
        weaponText = new List<Text>();
        Invoke("UpdateNameAndAmmo", 0.5f);
        ammoDict.Add(0, ammo1);
        ammoDict.Add(1, ammo2);
        ammoDict.Add(2, ammo3);
        ammoDict.Add(3, ammo4);
    }
    private void Start()
    {
        goldText.text = GameManager.instance.GetGold().ToString();
        chipText.text = GameManager.instance.GetChips().ToString();
        ammo1.fillAmount = 0;
        ammo2.fillAmount = 0;
        ammo3.fillAmount = 0;
        ammo4.fillAmount = 0;
    }

    public void AddWeaponHUD(Weapon weaponCollected)
    {
        if (space < spaceWeapons.Count)
        {
            weaponsHUD.Add(weaponCollected);
            spaceWeaponsImages[space].sprite = weaponCollected.SpriteWeapon;
            spaceWeapons[space].interactable = true;
            Text text1 = spaceWeapons[space].GetComponentInChildren<Text>();
            weaponText.Add(text1);
            WeaponSystem.instance.ammoWeapon.Add(weaponCollected.Ammo);

        }
        else
        {
            return;
        }
        space++;
    }

    public void UpdateNameAndAmmo()
    {
        for (int i = 0; i < weaponText.Count; i++)
        {
            weaponText[i].text = GameManager.instance.weaponsInPossesion[i].WeaponName + "\n " + WeaponSystem.instance.ammoWeapon[i];
        }
    }
    public void SetActiveWeapon(int selected)
    {
        selectedWeapon = selected;
        UISelector.GetComponent<RectTransform>().anchoredPosition = spaceWeapons[selected].GetComponent<RectTransform>().anchoredPosition;
        GameManager.instance.SetActualWeapon(selected);
        if (WeaponSystem.instance.ammoWeapon[selected] == 0)
        {
            PlayerController.instance.Reload();
        }
    }

    public void ActivateSuper()
    {
        PlayerController.instance.ActivateSuper();
        superButton.interactable = false;
    }

    public void ActiveSpriteLoot(GameObject lootGO)
    {
        this.lootGO = lootGO;
        spriteLoot.enabled = true;
    }
    public void DesactivateSpriteLoot()
    {
        spriteLoot.enabled = false;
    }
    private void Update()
    {
        if (spriteLoot.enabled)
        {
            Vector3 screenCenter = new Vector3(panelIndicators.sizeDelta.x, panelIndicators.sizeDelta.y, 0) / 2;
            Vector3 targetPositionScreenPoint = Camera.main.WorldToScreenPoint(lootGO.transform.position);
            Vector3 dir = (targetPositionScreenPoint - screenCenter).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x);
            spriteLoot.transform.position = screenCenter + new Vector3(Mathf.Cos(angle) * screenCenter.x * 0.7f, Mathf.Sin(angle) * screenCenter.y * 0.5f, 0);
            spriteLoot.transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
        }
        //CARGA DEL BOTÓN SUPER
        imageSuperButton.fillAmount = PlayerController.instance.chargeSuper / PlayerController.instance.chargeMax;

 
        //ACTUALIZAR MUNICIÓN ARMAS

    }
    public void RefreshAmmo(int ammo)
    {
        weaponText[selectedWeapon].text = GameManager.instance.weaponsInPossesion[selectedWeapon].WeaponName + "\n " + ammo;
    }
    public Image InstantiateMarkEnemy()
    {
        Image enemyMark1 = Instantiate(enemyMark, enemyMark.rectTransform);
        return enemyMark1;
    }
    public void UpdateGoldText()
    {
        goldText.text = GameManager.instance.GetGold().ToString();
    }
    public void UpdateChipText()
    {
        chipText.text = GameManager.instance.GetChips().ToString();
    }
    public IEnumerator FadeImageToBlack(GameObject player, Transform spawnPoint, GameState newState,bool changeLevel)
    {
        
        GameStateManager.Instance.SetState(newState);
        imageFade.enabled = true;
        PlayerController.instance.OnElevatorEnter();
        joystick.SetActive(false);
        
        //Time.timeScale = 0;
        GameManager.instance.SetGameStart(false);
        for (float i = 0; i < 1; i += Time.deltaTime * 0.5f)
        {

            imageFade.color = new Color(0, 0, 0, i);
            //if (i > 0.5f)
            //{
            //    imageRun.enabled = true;
            //    imageRun.color = new Color(1, 1, 1, i);
            //}
            yield return null;
        }

        imageFade.color = new Color(0, 0, 0, 1);
        if (changeLevel)
        {
            GameManager.instance.levelNumber++;
            GameManager.instance.InstantiateStage();
        }
        player.transform.position = spawnPoint.transform.position;
        
        //imageRun.color = new Color(1, 1, 1, 1);
        yield return new WaitForSeconds(1.5f);

        StartCoroutine("FadeImageToTransparent");
        
    }
    public IEnumerator FadeImageToTransparent()
    {
        for (float i = 1; i > 0; i -= Time.deltaTime * speedFade)
        {
            SoundManager.instance.audioSource.volume = 1 - i;
            imageFade.color = new Color(0, 0, 0, i);
            //if (i < 0.5f)
            //{
            //    imageRun.color = new Color(1, 1, 1, i);
            //}
            yield return null;
        }
        imageFade.color = new Color(0, 0, 0, 0);
        //imageRun.color = new Color(1, 1, 1, 0);
        //HUDManager.instance.ChangeLifeUI();
        yield return new WaitForSeconds(1);
        Debug.Log("SegundaCorroutine");
        imageFade.enabled = false;
        joystick.SetActive(true);
        PlayerController.instance.OnElevatorExit();
        speedFade = 0.5f;


    }
}
