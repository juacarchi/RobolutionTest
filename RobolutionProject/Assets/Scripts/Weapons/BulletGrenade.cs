using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletGrenade : MonoBehaviour
{
    public Weapon grenade;

    public float timeToExplosion=0.3f;
    public float radiusExplosion;
    public LayerMask enemyLayer;
    public LayerMask escenario;
    bool markActivated;
    GameObject mark;
    private void Awake()
    {
        timeToExplosion = PlayerController.instance.timeThrowingShoot;
        GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }
    private void OnGameStateChanged(GameState newGameState)
    {
        if (newGameState == GameState.Bonus)
        {
            DesactivateGrenade();
        }
    }
    private void Update()
    {
        if (Physics.Raycast(this.transform.position, -this.transform.up, 0.2f, escenario) && !markActivated)
        {
            ActivateMark();
        }
      
    }
    public void ActivateMark()
    {
        markActivated = true;
        Quaternion rotation = Quaternion.Euler(90, 0, 0);
        mark = null;
        mark = ObjectPooler.instance.SpawnFromPool("MarkGrenadePlayer", new Vector3(this.transform.position.x, 0.01f, this.transform.position.z), rotation);
        Invoke("Explosion", timeToExplosion);
        
    }
    public void Explosion()
    {
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, radiusExplosion, enemyLayer);
        if (hitColliders.Length > 0)
        {
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders != null)
                {
                    hitColliders[i].gameObject.GetComponent<IEnemy>().Hit(grenade.Damage);
                }
            }
        }

        ObjectPooler.instance.SpawnFromPool("FxExplosion", transform.position, Quaternion.identity);
        DesactivateGrenade();
    }
    public void DesactivateGrenade()
    {
        if (mark != null)
        {
            mark.SetActive(false);
            markActivated = false;
        }
        timeToExplosion = 2.2f; //Crear otra variable para no hacer asignacion directa
        this.gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }
}
