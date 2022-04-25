using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class PlayerController : MonoBehaviour
{
    public Image reloadBar;
    public AudioSource playerAudioSource;
    public static PlayerController instance;
    public Slider sliderHealth;
    public Image imageHP;
    public Image imageRedHP;
    public Slider sliderRedHealth;
    public float timeDicreasing = 10;
    [Header("PlayerAttack")]
    [SerializeField]
    float timeToCheck;
    [SerializeField] LayerMask enemyLayer;
    public Text textDebug;
    Transform enemyNearest;
    float radius;
    public float timeThrowingShoot;
    float timeToReload;
    int actualAmmo;
    bool isReloading;
    bool isShooting;
    bool isEnemyOnRange;
    Weapon actualWeapon;

    [Header("PlayerMovement")]

    public Joystick joystickMovement;
    [SerializeField]
    Rigidbody rb;
    [SerializeField]
    float speed;
    public Vector3 move;

    [Header("PlayerStats")]
    public float health;
    public float healthMax;


    [Header("Super Tracking Shoot")]
    public float radiusTrackingShoot;
    public GameObject bulletTrackingShot;
    public float durationTrackingShoot;
    float timerDurationTrackingShoot;

    [Header("Super Chained Bullet")]
    public int damageChainedBullet;
    public float radiusChainedBullet;//max range of distance 
    public GameObject chainedBullet;
    public float durationChainedBullet;
    float timerDurationChainedBullet;
    public bool isChainedBulletActivated;

    [Header("Super Shield")]
    public GameObject shieldPrefab;
    public float durationShield;
    float timerDurationShield;
    bool isShieldActivated;
    [Range(0,1)]//porcentaje de la proteccion de shield
    public float shieldProtection;
    [Space(10)]
    public float chargeMax = 100;
    public int weaponSelected;
    [HideInInspector]
    public float chargeSuper;
    bool isSuperActivated;
    bool isSuperShooted;
    public bool wasChosen;
    Text superText;
    public GameObject superFX;
    bool superFXActivated;
    GameState stateGame;
    public int medicalKitValue; //Cantidad de curación de botiquín;

    //ragdoll
    public Animator anim;
    private Rigidbody[] rigidbodies;
    public bool checkingEnemies;
    public bool checkEnemies;
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
        GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;

    }

    private void Start()
    {
        //Obtener estado del juego GameStateManager
        stateGame = GameStateManager.Instance.CurrentGameState;
        GameManager.instance.SetPlayer(this.gameObject);
        GameManager.instance.circleCircunference = GameObject.FindGameObjectWithTag("CircleCircunference");
        GameManager.instance.SetCircleCircunferenceRadius(new Vector3(radius, radius, 1));
        timeThrowingShoot = 1f;
        actualWeapon = GameManager.instance.GetActualWeapon();
        sliderHealth.maxValue = healthMax;
        sliderRedHealth.maxValue = healthMax;
        sliderRedHealth.value = sliderRedHealth.maxValue;
        UIManager.instance.imageSuperButton.fillAmount = chargeSuper / chargeMax;
        UIManager.instance.ammoDict[weaponSelected].fillAmount = 0;
        if (actualWeapon != null)
        {
            radius = actualWeapon.Range;
        }
        timeToReload = actualWeapon.TimeReload;
        actualAmmo = actualWeapon.Ammo;

        //textDebug.text = "Balas restantes:" + actualAmmo;

        Invoke("ChargeWeapons", 0.5f);
        superText = UIManager.instance.superText;
        anim.SetBool("isMoving", false);
        imageRedHP.fillAmount = healthMax;
        reloadBar.fillAmount = 0;
        //rigidbodies = transform.GetComponentsInChildren<Rigidbody>();

        //SetRagdollEnabled(false); 
    }
    public void ChargeWeapons()
    {
        for (int i = 0; i < GameManager.instance.weaponsInPossesion.Count; i++)
        {
            if (GameManager.instance.weaponsInPossesion[i] != null)
            {
                GameManager.instance.weaponsInPossesion[i].IsCharged = true;
            }
        }
    }
    private void OnGameStateChanged(GameState newGameState)
    {
        stateGame = newGameState;
        if (newGameState == GameState.Paused)
        {
            Time.timeScale = 0;
            enabled = false;
        }
        else
        {
            Time.timeScale = 1;
            enabled = true;
        }
    }
    public void CheckEnemies()
    {
        //REGISTRAR QUE ENEMIGOS SE ENCUENTRAN DENTRO DE LA ESFERA
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, radius, enemyLayer); //MODIFICAR RADIO SEGUN ARMA
        if (hitColliders.Length > 0)
        {
            var transformList = new List<Transform>(hitColliders.Length);
            foreach (var hitCollider in hitColliders)
            {
                transformList.Add(hitCollider.transform);
            }
            GetClosestEnemy(transformList);
        }
        else
        {
            isEnemyOnRange = false;
            GameManager.instance.targetCircunference.gameObject.SetActive(false);
        }
    }
    public void SetCheckEnemies(bool check)
    {
        checkEnemies = check;
        if(check == true)
        {
            checkingEnemies = false;
        }
    }
    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + move * speed * Time.fixedDeltaTime);
    }
    private void Update()
    {
        
        //SOLO USAR JUGADOR CUANDO SE MUEVE
        if (GameManager.instance.GetGameStart())
        {
            #region Movimiento provisional y llamada al disparo

            move.x = joystickMovement.Horizontal;
            move.z = joystickMovement.Vertical;
            GameManager.instance.SetMovePlayer(move);

            //DETECCIÓN MOVIMIENTO PERSONAJE MODIFICAR CUANDO HAYA JOYSTICK
            if (move != Vector3.zero)
            {
                anim.SetBool("isMoving", true);
                CancelInvoke("NormalShoot");
                CancelInvoke("ThrowingShoot");
                isShooting = false;
                this.transform.forward = move; //Change with player movement script
            }
            else
            {
                anim.SetBool("isMoving", false);
                if (isSuperActivated && !isSuperShooted)
                {
                    if (GameManager.instance.GetPlayerType() == PlayerType.Tracking)
                    {
                        TrackingShoot();
                        isSuperShooted = true;
                    }
                    else if (GameManager.instance.GetPlayerType() == PlayerType.Shield)
                    {
                        Shield();
                        isSuperShooted = true; 
                    }
                    else if (GameManager.instance.GetPlayerType() == PlayerType.ChainedBullet)
                    {
                        Chainedbullet(); 
                        isSuperShooted = true;
                        isChainedBulletActivated = true;
                    }
                }
                else
                {
                    if (!isShooting && isEnemyOnRange)
                    {
                        if (GameManager.instance.GetActualWeapon().TypeWeapon == WeaponType.NormalWeapon)
                        {
                            InvokeRepeating("NormalShoot", 0.01f, actualWeapon.Cadence);
                            isShooting = true;
                        }
                        else if (GameManager.instance.GetActualWeapon().TypeWeapon == WeaponType.ThrowingWeapon)
                        {
                            InvokeRepeating("ThrowingShoot", 0.01f, actualWeapon.Cadence);
                            isShooting = true;
                        }
                    }
                }
            }

            if (!isSuperActivated)
            {
                if (chargeSuper >= chargeMax && checkEnemies)
                {
                    UIManager.instance.superButton.interactable = true;
                }
                else
                {
                    UIManager.instance.superButton.interactable = false;
                }
            }

            #endregion
            if (checkEnemies && !checkingEnemies)
            {
                InvokeRepeating("CheckEnemies", 0, timeToCheck);
                checkingEnemies = true;
            }
            if (!checkEnemies)
            {
                CancelInvoke();
                //GameManager.instance.targetCircunference.gameObject.SetActive(true);
            }

            if (enemyNearest != null)
            {
                GameManager.instance.targetCircunference.transform.position = new Vector3(enemyNearest.position.x, 0.1f, enemyNearest.position.z);
            }
            else
            {
                GameManager.instance.targetCircunference.gameObject.SetActive(false);
            }

            //RECARGA
            if (isReloading)
            {
                reloadBar.fillAmount = timeToReload;
                //textDebug.text = "Recargando" + timeToReload;
                timeToReload -= Time.deltaTime;
                if (timeToReload <= 0)
                {
                    isReloading = false;
                    timeToReload = actualWeapon.TimeReload;
                    actualAmmo = actualWeapon.Ammo;
                    WeaponSystem.instance.UpdateAmmo(weaponSelected, actualAmmo);
                    actualWeapon.IsCharged = true;
                    //textDebug.text = "Balas restantes:" + actualAmmo;
                    wasChosen = false;
                }
            }
            else
            {
                //textDebug.text = "Balas restantes:" + actualAmmo;
                UIManager.instance.RefreshAmmo(actualAmmo);
            }
        }

        //SLIDER VIDA
        //sliderHealth.value = health;
        //if (sliderRedHealth.value > sliderHealth.value)
        //{
        //    sliderRedHealth.value -= (Time.deltaTime / timeDicreasing);
        //}

        if (isSuperActivated)
        {
            //if (!superFXActivated)
            //{
            //    superFX.SetActive(true);
            //    superFXActivated = true;
            //}

            switch (GameManager.instance.GetPlayerType())
            {
                case PlayerType.Tracking:
                    timerDurationTrackingShoot -= Time.deltaTime;
                    superText.text = timerDurationTrackingShoot.ToString();
                    //activar aqui efecto de la habilidad especial
                    //
                    if (timerDurationTrackingShoot <= 0)
                    {
                        isSuperActivated = false;
                        isSuperShooted = false;
                        chargeSuper = 0;
                    };
                    break;
                case PlayerType.Shield:
                    timerDurationShield -= Time.deltaTime;
                    superText.text = timerDurationShield.ToString();
                    //activar aqui efecto de la habilidad especial
                    //activar el objeto hijo dentro del player que es el escudo
                    
                    if (timerDurationShield <= 0)
                    {
                        isSuperActivated = false;
                        isSuperShooted = false;
                        isShieldActivated = false;
                        chargeSuper = 0;
                    };
                    break;
                case PlayerType.ChainedBullet:
                    timerDurationChainedBullet -= Time.deltaTime;
                    superText.text = timerDurationChainedBullet.ToString();
                    //activar aqui efecto de la habilidad especial
                    //
                    if (timerDurationChainedBullet <= 0)
                    {
                        CancelInvoke();
                        isSuperActivated = false;
                        isSuperShooted = false;
                        chargeSuper = 0;
                        isChainedBulletActivated = false;
                    };
                    break;
            }
        }
        //else
        //{
        //    if (superFXActivated)
        //    {
        //        superFX.SetActive(false);
        //        superFXActivated = false;
        //    }
        //}

        if (imageRedHP.fillAmount > imageHP.fillAmount) DecreaseRed();
        else if (imageRedHP.fillAmount < imageHP.fillAmount) IncreaseRed();
       
    }
    public void CheckLife()
    {
        imageHP.fillAmount = ((float)health / healthMax);
    }
    public void IncreaseRed()
    {
        imageRedHP.fillAmount += (Time.deltaTime / timeDicreasing);
    }
    public void DecreaseRed()
    {
        imageRedHP.fillAmount -= (Time.deltaTime / timeDicreasing);
    }
    Transform GetClosestEnemy(List<Transform> enemiesTransform)
    {
        Transform nearestTarget = null; //Declarar variable donde albergar enemigo más cercano
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (Transform potentialTarget in enemiesTransform)
        {
            Vector3 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr) //Ir actualizando hasta conseguir el enemigo más cercano
            {
                closestDistanceSqr = dSqrToTarget;
                nearestTarget = potentialTarget;
            }
        }

        GameManager.instance.SetTargetCircunferencePosition(new Vector3(nearestTarget.position.x, 0.1f, nearestTarget.position.z));
        if (nearestTarget != null)
        {
            isEnemyOnRange = true;
            enemyNearest = nearestTarget;
            GameManager.instance.targetCircunference.gameObject.SetActive(true);
        }
        return nearestTarget;
    }//Detectar enemigo más cercano
    //TIPOS DE DISPAROS
    public void NormalShoot()
    {
        if (GameManager.instance.GetActualWeapon().IsCharged)
        {
            if (enemyNearest != null)
            {
                Vector3 dirAttack = enemyNearest.transform.position - this.transform.position;
                RaycastHit hit;
                if (Physics.Raycast(transform.position, dirAttack, out hit))
                {
                    if (hit.collider.CompareTag("Enemy"))
                    {
                        //EMITE SONIDO
                        anim.SetTrigger("Shoot");
                        SFXManager.instance.PlaySFX(playerAudioSource, actualWeapon.WeaponName);
                        GameManager.instance.targetCircunference.GetComponent<SpriteRenderer>().color = Color.red;
                        dirAttack.x += Random.Range(-GameManager.instance.GetActualWeapon().Dispersion, GameManager.instance.GetActualWeapon().Dispersion);
                        Quaternion rotation = Quaternion.Euler(Vector3.zero);
                        GameObject bulletGO = ObjectPooler.instance.SpawnFromPool(actualWeapon.Tag, transform.position, Quaternion.identity);
                        bulletGO.transform.forward = dirAttack.normalized;
                        //BulletMovement bullet = bulletGO.GetComponent<BulletMovement>();
                        //bullet.SetIsShootingPlayer(true);
                        //bullet.SetDirection(dirAttack);
                        //bullet.SetActualWeapon(actualWeapon);
                        //Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, radiusChainedBullet, enemyLayer); //MODIFICAR RADIO SEGUN ARMA
                        //if (hitColliders.Length > 0)
                        //{
                        //    var transformList = new List<Transform>(hitColliders.Length);
                        //    foreach (var hitCollider in hitColliders)
                        //    {
                        //        transformList.Add(hitCollider.transform);
                        //    }
                        //    //bullet.SetEnemiesTransform(transformList);
                        //}
                        //Girar hacia el enemigo la vista
                        dirAttack.y = 0; //HACIA DONDE MIRA EL PLAYER
                        this.transform.forward = dirAttack;
                        actualAmmo--;
                        Debug.Log("actualAmmo" + actualAmmo);
                        Debug.Log("ammoWeapon" + actualWeapon.Ammo);

                        if (actualAmmo != 0)
                        {
                            float actualWeaponAmmo = actualWeapon.Ammo;
                            float fillAmount =((actualAmmo/actualWeaponAmmo)-1)*-1;
                            UIManager.instance.ammoDict[weaponSelected].fillAmount = fillAmount;
                        }
                        else
                        {
                            UIManager.instance.ammoDict[weaponSelected].fillAmount = 0;
                        }
                        
                        WeaponSystem.instance.UpdateAmmo(weaponSelected, actualAmmo);
                        if (actualAmmo <= 0)
                        {
                            GameManager.instance.GetActualWeapon().IsCharged = false;
                            GameManager.instance.NextWeapon(weaponSelected);
                            isShooting = false;
                        }

                        chargeSuper++; //CARGAR SUPER
                    }
                    else
                    {
                        GameManager.instance.targetCircunference.color = Color.gray;
                    }
                }
            }
        }
        else
        {
            //Debug.Log("NO TIENE BALAS");
        }
    }
    public void ThrowingShoot()
    {
        if (GameManager.instance.GetActualWeapon().IsCharged)
        {
            //Girar hacia el enemigo la vista
            if (enemyNearest != null)
            {
                GameManager.instance.targetCircunference.color = Color.red;
                this.transform.forward = enemyNearest.position - transform.position;
                Vector3 distance = enemyNearest.position - this.transform.position;
                Vector3 distanceXZ = distance;
                distanceXZ.y = 0f;
                float Sy = distance.y;
                float Sxz = distanceXZ.magnitude;
                Debug.Log(Sxz);
                float Vxz = Sxz / timeThrowingShoot;

                float Vy = Sy / timeThrowingShoot + 0.5f * Mathf.Abs(Physics.gravity.y) * timeThrowingShoot;

                Vector3 result = distanceXZ.normalized;

                result *= Vxz;
                result.y = Vy;

                GameObject grenadeBullet = ObjectPooler.instance.SpawnFromPool("GrenadeBullet", this.transform.position + Vector3.forward, Quaternion.identity);
                Rigidbody rbBullet = grenadeBullet.GetComponent<Rigidbody>();
                rbBullet.velocity = result;

                chargeSuper++;
                actualAmmo--;
                WeaponSystem.instance.UpdateAmmo(weaponSelected, actualAmmo);
                if (actualAmmo <= 0)
                {
                    GameManager.instance.GetActualWeapon().IsCharged = false;
                    GameManager.instance.NextWeapon(weaponSelected);
                    isShooting = false;
                }
            }
        }
           



    }

    //ATAQUES ESPECIALES
    public void TrackingShoot()
    {
        /*
         * Crear una lista de todos los enemigos que tiene cercanos del rango de habilidad.
         * Atraves de esa lista pasarle a cada una de las balas que se cree, el gameobject del enemigo para que le persiga hasta su fin
         */

        if (checkEnemies)
        {
            Collider[] hitColliders= Physics.OverlapSphere(this.transform.position, radiusTrackingShoot, enemyLayer);
            int numberBullet = 0;
            if (hitColliders.Length > 0)
            {
                UIManager.instance.superButton.interactable = true;
                numberBullet++; 
                foreach (var enemyTracking in hitColliders)
                {
                    GameObject bulletTrack = Instantiate(bulletTrackingShot, transform.position, Quaternion.identity);
                    bulletTrack.GetComponent<BulletPlayer>().ActiveTrackingShoot(enemyTracking.gameObject);
                    bulletTrack.name = "Bullet Traking number " + numberBullet; //eliminar esto una vez que comprobemos que funciona sin ningun problema
                   
                }
            }

        }

        //BulletPlayer.instance.ActiveTrackingShoot(/*insertar aqui enemigo*/); 

        /*
         * Juan
         */
        //if (checkEnemies)
        //{
            
        //    Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, radiusTrackingShoot, enemyLayer);
        //    if (hitColliders.Length > 0)
        //    {
        //        UIManager.instance.superButton.interactable = true;
        //        for (int i = 0; i < hitColliders.Length; i++)
        //        {
        //            RaycastHit hit;

        //            if (Physics.Raycast(transform.position, hitColliders[i].transform.position, out hit)) //SOLO DISPARA CUANDO NO ATRAVIESA PAREDES
        //            {
        //                GameObject bulletGO = ObjectPooler.instance.SpawnFromPool("NormalBullet", transform.position, Quaternion.identity);
        //                BulletMovement bullet = bulletGO.GetComponent<BulletMovement>();
        //                bullet.SetIsShootingPlayer(true);
        //                bullet.SetDirection(hitColliders[i].transform.position - transform.position);
        //            }
        //        }
        //    }
        //}
       
        

    }
    public void Shield()
    {
        //Debug.Log("ESCUDO");
        //GameObject shield = Instantiate(shieldPrefab, transform.position, Quaternion.identity);
        //shield.transform.SetParent(this.transform);
        //Destroy(shield, durationShield);
        //isShieldActivated = true;
    }

    public void Chainedbullet()
    {
        
        if (checkEnemies)
        {
            GameObject bulletEnchaited = Instantiate(chainedBullet, transform.position, Quaternion.identity);
            Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, radiusChainedBullet, enemyLayer);
            if(hitColliders == null)
            {
                Debug.Log("Ningun enemigo detectado");
                return; 
            }
            else
            {
                List<GameObject> listEnemiesEnchain= new List<GameObject>(hitColliders.Length);

                foreach (var enemyEnchain in hitColliders)
                {
                    listEnemiesEnchain.Add(enemyEnchain.gameObject); 
                }
               
                bulletEnchaited.GetComponent<BulletPlayer>().ActiveEnchaitedbullet(listEnemiesEnchain, radiusChainedBullet);
            }

        }
            //bulletEnchaited.GetComponent<BulletPlayer>().ActiveEnchaitedbullet(); 

            /*
             * Parte Victor (Chained bullet)
             * 
             * crear lista de enemigos.
             * encontrar al enemigo mas cercano. Si hay uno o mas atacar al mas cercano. si no hay terminar habilidad (pasar lista a la bala)
             * la bala se encarga del resto de checking.
             * add enemigo a la lista de enemigos
             * rastrear de nuevo, de los enemigos encontrados, mirar si esta en la lista. 
             * viajar al enemigo mas cercano que no este en la lista (animación)
             * si ya estan todos en la lista desaparecer y terminar habilidad
             *
             *
             * mayor problema que encuentro a esto. lo hace de forma instantanea. es decir en la cola de procesos lo hace todo de seguido.
             * solucion: hacerlo o coroutine o dentro del codigo de la bala(si no hay crear uno), de esta forma ocurrira de forma simultanea mientras
             * que la cola de procesos sigue en curso.
            */

            //conclusion: aqui tiene que ir codigo que active la funcion en la bala y no en el player.

        }

    public void WeaponChanged(int weaponSelected)
    {
        
        CancelInvoke("NormalShoot");
        CancelInvoke("ThrowingShoot");
        actualWeapon = GameManager.instance.GetActualWeapon();
        this.weaponSelected = weaponSelected;
        isEnemyOnRange = false;
        radius = actualWeapon.Range;
        if (GameManager.instance.circleCircunference != null)
        {
            GameManager.instance.SetCircleCircunferenceRadius(new Vector3(radius, radius, 1));
        }
        timeToReload = actualWeapon.TimeReload;
        //COGER MUNICION ARMA
        actualAmmo = WeaponSystem.instance.ammoWeapon[weaponSelected];
        if (actualAmmo > 0)
        {
            actualWeapon.IsCharged = true;
        }
        else
        {
            Reload();
        }
        isShooting = false;
    } //Arma cambiada, cambiar atributos
    public void Hit(float damage)
    {
        SFXManager.instance.PlaySFX(playerAudioSource, "Hit");
        anim.SetTrigger("Hit");
        if (isShieldActivated)
        {
            damage -= (damage * shieldProtection);
        }
        GameObject popUp = ObjectPooler.instance.SpawnFromPool("DamagePopUp", transform.position, Quaternion.identity);
        popUp.GetComponent<TextMeshPro>().text = damage.ToString();
        health -= damage;
        CheckLife();

        if (health <= 0)
        {
            Die();
            PlayerController.instance.checkingEnemies = false;
            GameManager.instance.levelNumber = 0;
            SceneManager.LoadScene(2);
        }
        if (!isSuperActivated)
        {
            chargeSuper++;
        }
    }//damage al jugagor



    public void Die()
    {
        Debug.Log("Ha muerto player");
        //SetRagdollEnabled(true); 
    }

    //private void SetRagdollEnabled(bool enabled)
    //{
    //    bool isKinematic = !enabled;
    //    foreach (Rigidbody rigidbody in rigidbodies)
    //    {
    //        rigidbody.isKinematic = isKinematic;
    //    }
    //    anim.enabled = !enabled;

    //}

    //Para hacer que el ragdoll funcione, hay que hacer que no accione el kinematico del objecto padre y desactivar
    //todos los colliders que tiene la geometria para el ragdoll (excepto la del padre)
    //para luego activar el ragdoll activar todos los colliders de la geometria
    public void ActivateSuper()
    {
      

        if (GameManager.instance.GetPlayerType() == PlayerType.Tracking)
        {
            timerDurationTrackingShoot = durationTrackingShoot;
            isSuperActivated = true;
        }
        else if (GameManager.instance.GetPlayerType() == PlayerType.Shield)
        {
            timerDurationShield = durationShield;
            isSuperActivated = true;
            isShieldActivated = true;
        }
        else if (GameManager.instance.GetPlayerType() == PlayerType.ChainedBullet)
        {
            timerDurationChainedBullet = durationChainedBullet;
            isSuperActivated = true;
        }
    }

    public void Reload()
    {
        SFXManager.instance.PlaySFX(playerAudioSource, "Reload");
        CancelInvoke("NormalShoot");
        CancelInvoke("ThrowingShoot");
        reloadBar.fillAmount = 1;
        if (actualAmmo < GameManager.instance.GetActualWeapon().Ammo)
        {
            isReloading = true;
        }
        else
        {
            wasChosen = false;
        }
    }
    public void OnElevatorEnter()
    {
        PlayerController.instance.joystickMovement.enabled = false;
        PlayerController.instance.move.x = 0;
        PlayerController.instance.move.z = 0;
        anim.SetBool("isMoving", false);
    }
    public void OnElevatorExit()
    {
        PlayerController.instance.joystickMovement.enabled = true;
    }
    public void IncreaseHP()
    {

        health += medicalKitValue;
        if (health > healthMax)
        {
            health = healthMax;
        }
        sliderRedHealth.value = health;
        CheckLife();

    }
    private void OnDestroy()
    {
        GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }
}
