using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class EnemyBase : MonoBehaviour, IEnemy
{
    // Customization Variables
    [Header("Customization Variables")]
    [SerializeField] EnemyData enemyData; 
    int health;

    [SerializeField]
    private Transform[] patrolingPoints;
   
    // Config Variables
    [Header("Config Variables")]
    [SerializeField]
    private NavMeshAgent agent;
    [SerializeField]
    private float near;
    [SerializeField]
    private Transform hand;
    [SerializeField]
    LayerMask playerMask;
    [SerializeField]
    AudioSource audioSource;
    [SerializeField]
    AudioSource audioSourceWalk;
    // Internal variables
    [HideInInspector]
    public Transform follow = null;
    private Vector3 home;
    private float blindTime = 0;
    private float stunTime = 0;
    private float atackTime = 0;
    private float reloadTime = 0;
    private int ammo = 0;
    private int patrolingPointIndex = 0;
    Image spriteEnemy;
    public Image imageHP;
    public Image imageRedHP;
    public Animator anim;
    GameObject playerGO;
    [Tooltip("If you increase this number, the velocity of dicrease will dicrease")]
    public float velocityDicrease = 10;

    public EnemyData EnemyData { get => enemyData; set => enemyData = value; }

    private void Awake()
    {
        GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }
    private void OnGameStateChanged(GameState newGameState)
    {

        if (newGameState == GameState.Gameplay)
        {
            enabled = true;
            anim.enabled = true;
            anim.SetBool("isMoving", false);
            
        }
        else
        {
            anim.enabled = false;
            agent.SetDestination(agent.transform.position);
            enabled = false;
        }

    }
    void Start()
    {
        GameplayManager.instance.AddEnemy();
        spriteEnemy = UIManager.instance.InstantiateMarkEnemy();
        health = enemyData.HealthMax;
        home = transform.position;
        ammo = enemyData.Weapon.Ammo;
       
        if (enemyData.Weapon.Range < near) { Debug.LogWarning("If weapon range is lower than near, enemy will never atack", this); }
        if (enemyData.Patrol && patrolingPoints.Length < 2) { throw new EnemyException("If enemy patrol is set to True, patrolingPoints length should be grater than 2"); }
        spriteEnemy.enabled = false;
        audioSourceWalk.enabled = false;
        //AL SER LA VIDA ROJA SE PONE AL MAXIMO EN UN PRINCIPIO
        imageRedHP.fillAmount = enemyData.HealthMax;
        Invoke("AssignPlayer", 0.5f);
    }
    void AssignPlayer()
    {
        playerGO = GameManager.instance.GetPlayer();
    }

    void Update()
    {
        if (GameManager.instance.GetGameStart() && playerGO != null)
        {
            Look();
        }
        //MARCA ENEMIGO

        //if (IsInView(transform.position))
        //{
        //    spriteEnemy.enabled = false;
        //    instantiated = false;
        //    Debug.Log("En CAMARA");
        //}
        //else
        //{
        //    Debug.Log("FUERA DE CAMARA");
        //    if (!instantiated)
        //    {
        //        spriteEnemy.enabled = true;
        //        instantiated = true;
        //    }
        //    if (isAttacking)
        //    {
        //        //HACER EFECTO POR ANIMACIÓN
        //    }
        //}
        //MOVIMIENTO MARCA ENEMIGO
        //if (spriteEnemy.enabled)
        //{
        //    Vector3 screenCenter = new Vector3(Screen.width, UIManager.instance.panelIndicators.sizeDelta.y, 0) / 2;
        //    Vector3 targetPositionScreenPoint = Camera.main.WorldToScreenPoint(this.transform.position);
        //    Vector3 dir = (targetPositionScreenPoint - screenCenter).normalized;
        //    float angle = Mathf.Atan2(dir.y, dir.x);
        //    spriteEnemy.transform.position = screenCenter + new Vector3(Mathf.Cos(angle) * screenCenter.x * 1f, Mathf.Sin(angle) * screenCenter.y * 0.5f, 0);
        //    spriteEnemy.transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
        //}

        if (imageRedHP.fillAmount >= imageHP.fillAmount) DecreaseRed();
        if (GameManager.instance.navMeshCreated)
        {
            if (agent.destination == this.transform.position)
            {
                anim.SetBool("isMoving", false);
            }
        }
        if (GameManager.instance.GetGameStart())
        {
            Move();
        }
    }
    private void FixedUpdate()
    {
       
    }

    private void DecreaseRed()
    {
        imageRedHP.fillAmount -= (Time.deltaTime / velocityDicrease); 
    }

    public void Hit(int damage)
    {
        SFXManager.instance.PlaySFX(audioSource, "Hit");
        anim.SetTrigger("Hit");
        Follow(GameManager.instance.GetPlayer().transform);
        BroadcastAlert();
        GameObject popUp = ObjectPooler.instance.SpawnFromPool("DamagePopUp", transform.position, Quaternion.identity);
        popUp.GetComponent<TextMeshPro>().text = damage.ToString();
        ObjectPooler.instance.SpawnFromPool("Hit", transform.position, Quaternion.identity);
        
        health -= damage;
        if (health <= 0)
        {
            Die();
            GameplayManager.instance.RemoveEnemy();
        }
        CheckLife();
    }
    public void CheckLife()
    {
        imageHP.fillAmount = ((float) health / enemyData.HealthMax);
    }

    private void BroadcastAlert()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, enemyData.AlertRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.transform.CompareTag("Enemy"))
            {
                hitCollider.transform.GetComponent<EnemyBase>().Follow(GameManager.instance.GetPlayer().transform);
            }
        }
    }

    public void Stun(float time)
    {
        stunTime = time;
    }

    private void Die()
    {
        DropLoot();
        GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
        Destroy(this.gameObject);
    }

    public void DropLoot()
    {
        int rnd = Random.Range(0, LootManager.instance.lootItems.Count);
        ObjectPooler.instance.SpawnFromPool(LootManager.instance.lootItems[rnd], transform.position, Quaternion.Euler(-33, 180, 0));
    }
    private void Look()
    {
        bool v = Physics.CheckSphere(transform.position, enemyData.FovDepth ,playerMask);
        
        blindTime += Time.deltaTime;
        if (v)
        {
            Vector3 forward = transform.rotation * Vector3.forward;
            Vector3 playerPos = playerGO.transform.position - transform.position;

            Vector3 right = Quaternion.Euler(0, enemyData.FovAngle / 2, 0) * forward;
            Vector3 left = Quaternion.Euler(0, -enemyData.FovAngle / 2, 0) * forward;

            float area = Vector3.Angle(left, right);
            if (Vector3.Angle(left, playerPos) < area && Vector3.Angle(right, playerPos) < area)
            {
                RaycastHit hit;
                Physics.Raycast(transform.position, playerGO.transform.position - transform.position, out hit);
                if (hit.transform == playerGO.transform)
                {
                    Follow(playerGO.transform);
                }
            }
        }
          
        if (blindTime > enemyData.ForgettingTime)
        {
            StopFollowing();
        }
    }

    private void Follow(Transform target)
    {
        blindTime = 0;

        if (follow == target) { return; }
        follow = target;
        anim.SetBool("isMoving", true);
        audioSourceWalk.enabled = true;
    }

    private void StopFollowing()
    {
        anim.SetBool("isMoving", false);
        audioSourceWalk.enabled = false;
        follow = null;
        agent.stoppingDistance = 0;

        if (enemyData.TurnBackHome)
        {
            if (enemyData.Patrol)
            {
                agent.SetDestination(patrolingPoints[patrolingPointIndex].position);
            }
            else
            {
                agent.SetDestination(home);
            }
        }
        else { agent.SetDestination(transform.position); }
    }

    private void Move()
    {
        agent.isStopped = stunTime > 0 || atackTime > 0 || reloadTime > 0;

        if (stunTime > 0)
        {
            stunTime -= Time.deltaTime;
            return;
        }
        else if (atackTime > 0)
        {
            atackTime -= Time.deltaTime;
            return;
        }
        else if (reloadTime > 0)
        {
            reloadTime -= Time.deltaTime;
            return;
        }

        if (follow != null)
        {
            agent.SetDestination(follow.position);
            agent.isStopped = agent.remainingDistance <= near;

            if (agent.remainingDistance <= enemyData.Weapon.Range)
            {
                Atack();
                //isAttacking = true;
            }
            else
            {
                //isAttacking = false;
                anim.SetBool("isMoving", true);
                audioSourceWalk.enabled = true;
            }
        }
        else if (enemyData.TurnBackHome && enemyData.Patrol)
        {
            if (!agent.pathPending && agent.remainingDistance < near)
            {
                patrolingPointIndex = (patrolingPointIndex + 1) % patrolingPoints.Length;
                agent.SetDestination(patrolingPoints[patrolingPointIndex].position);
            }
        }
    }

    private void Atack()
    {
        atackTime = enemyData.Weapon.Cadence;
        
        switch (enemyData.Weapon.TypeWeapon)
        {
            case WeaponType.NormalWeapon:
                RangedAtack();
                break;
            case WeaponType.ThrowingWeapon:
                ThrowingAtack();
                break;
            case WeaponType.MeleeWeapon:
                MeleeAtack();
                break;
        }
    }

    private void RangedAtack()
    {
        //DIFERENCIAR POR TIPO DE ARMA
        //EN CASO DE METRALLETA INSTANCIAR MÁS BALAS
        SFXManager.instance.PlaySFX(audioSource, enemyData.Weapon.WeaponName);
        anim.SetTrigger("Shoot");
        anim.SetBool("isMoving", false);
        audioSourceWalk.enabled = false;
        if (enemyData.Weapon.WeaponName != "MachineGun")
        {
            Vector3 dirAttack = follow.transform.position - transform.position;
            this.transform.forward = dirAttack; //Orientar al enemigo hacia el jugador cuando dispara
            dirAttack.x += Random.Range(-enemyData.Weapon.Dispersion, enemyData.Weapon.Dispersion);
            GameObject instance = ObjectPooler.instance.SpawnFromPool(enemyData.Weapon.Tag, transform.position+Vector3.forward, Quaternion.identity);
            instance.GetComponent<BulletEnemy>().SetDamageBullet(enemyData.Weapon.Damage);
            instance.transform.forward = dirAttack.normalized;
        }

        ammo--;
        if (ammo <= 0)
        {
            reloadTime = enemyData.Weapon.TimeReload;
            ammo = enemyData.Weapon.Ammo;
        }
    }

    private void ThrowingAtack()
    {
        SFXManager.instance.PlaySFX(audioSource, enemyData.Weapon.WeaponName);
        anim.SetTrigger("Shoot");
        anim.SetBool("isMoving", false);
        audioSourceWalk.enabled = false;
    }

    private void MeleeAtack()
    {
        SFXManager.instance.PlaySFX(audioSource, enemyData.Weapon.WeaponName);
        anim.SetTrigger("Shoot");
        anim.SetBool("isMoving", false);
        audioSourceWalk.enabled = false;
    }
    public NavMeshAgent GetAgent()
    {
        return agent;
    }
    //FUNCION PARA SABER SI EL ENEMIGO ESTA EN PANTALLA O NO

    //public bool IsInView(Vector3 worldPos)
    //{
    //    Transform camTransform = Camera.main.transform;
    //    Vector2 viewPos = Camera.main.WorldToViewportPoint(worldPos);
    //    Vector3 dir = (worldPos - camTransform.position).normalized;
    //    float dot = Vector3.Dot(camTransform.forward, dir); // Determinar si el objeto está frente a la cámara  

    //    if (dot > 0 && viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
    //        return true;
    //    else
    //        return false;
    //}

    
//#if DEBUG
//    private void OnDrawGizmos()
//    {
//        // Field Of View
//        Vector3 forward = transform.rotation * Vector3.forward * fovDepth;

//        Vector3 right = Quaternion.Euler(0, fovAngle / 2, 0) * forward;
//        Vector3 left = Quaternion.Euler(0, -fovAngle / 2, 0) * forward;

//        Gizmos.color = Color.red;
//        Gizmos.DrawLine(transform.position, transform.position + forward);

//        Gizmos.color = Color.green;
//        Gizmos.DrawLine(transform.position, transform.position + right);
//        Gizmos.DrawLine(transform.position, transform.position + left);


//        Gizmos.color = new Color(0, 0, 1, 0.1f);
//        Gizmos.DrawSphere(transform.position, alertRange);


//        // Patroling
//        if (patrol)
//        {
//            Gizmos.color = Color.blue;
//            for (int i = 0; i < patrolingPoints.Length; i++)
//            {
//                Gizmos.DrawSphere(patrolingPoints[i].position, 0.1f);

//                Gizmos.DrawLine(patrolingPoints[i].position, patrolingPoints[(i + 1) % patrolingPoints.Length].position);
//            }
//        }

//        // Stun
//        if (stunTime > 0)
//        {
//            Gizmos.DrawIcon(transform.position + Vector3.up * 2, "d_WaitSpin00");
//        }

//        // Atack
//        if (atackTime > 0)
//        {
//            Gizmos.DrawIcon(transform.position + Vector3.up * 2, "console.erroricon");
//        }
//    }
//#endif
}
