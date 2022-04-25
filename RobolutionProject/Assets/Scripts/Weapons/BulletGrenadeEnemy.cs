using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletGrenadeEnemy : MonoBehaviour
{
    public int damage;
    public float timeToExplosion;
    public float radiusExplosion;
    public LayerMask playerMask;
    public GameObject fxExplosion;
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
        if (Physics.Raycast(this.transform.position, -this.transform.up, 0.2f, escenario)&&!markActivated)
        {
            ActivateMark();
        }
      
    }
    public void ActivateMark()
    {
        Invoke("Explosion", timeToExplosion);
        markActivated = true;
        Quaternion rotation = Quaternion.Euler(90, 0, 0);
        mark = null;
        mark = ObjectPooler.instance.SpawnFromPool("MarkGrenade", new Vector3(this.transform.position.x, 0.01f, this.transform.position.z), rotation);
    }
    public void Explosion()
    {
        if (Physics.CheckSphere(transform.position, radiusExplosion, playerMask))
        {
            PlayerController.instance.Hit(damage);
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
