using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackedBullet : MonoBehaviour, IPooledObject
{
    public float timeToDesactivate;
    public TrailRenderer trail;
    Weapon actualWeapon;
    public float speed = 10f;

    //habilidades
    //public static BulletPlayer instance; 
    private bool isTracking = false;

    private GameObject enemyTracking;
    private List<GameObject> enemiesToEnchaint;
    private List<GameObject> enemiesEnchainted;
    private float maxDistance;
    private bool isEnchaint = false;
    private bool isEnchaited = false;


    private void Awake()
    {
        GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;


    }
    private void OnGameStateChanged(GameState newGameState)
    {
        if (newGameState == GameState.Bonus)
        {
            DesactiveBullet();
        }
    }
    public void OnObjectSpawn()
    {
        Invoke("DesactiveBullet", timeToDesactivate); //DESTRUIR CUANDO CHOQUE CON ALGO O AL TIEMPO DE INSTANCIAR
    }
    public void DesactiveBullet()
    {
        this.gameObject.SetActive(false);

        if (trail != null) trail.Clear();
    }
    private void Update()
    {
        if (!isTracking && !isEnchaited) transform.Translate(Vector3.forward * speed * Time.deltaTime);
        else if (isTracking && !isEnchaited) transform.Translate(enemyTracking.transform.position * speed * Time.deltaTime);
      
        //meter aqui el codigo especial de las balas
        //para que la bala haga su efecto especiaal.


    }

   

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && !isEnchaited)
        {
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            enemy.Hit(GameManager.instance.GetActualWeapon().Damage);
            enemy.GetAgent().velocity = (other.transform.position - this.transform.position).normalized * 2;
            DesactiveBullet();
        }
        else if (other.CompareTag("Enemy") && isEnchaited && isEnchaint)
        {
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            enemy.Hit(GameManager.instance.GetActualWeapon().Damage);
            enemy.GetAgent().velocity = (other.transform.position - this.transform.position).normalized * 2;
            isEnchaint = false;
        }
        else
        {
            Debug.Log("ERROR al golpear enemigo");
        }
    }

    public void ActiveTrackingShoot(GameObject EnemyToTrack)
    {
        enemyTracking = EnemyToTrack;
        isTracking = true;
        //si no convence como queda, meterle animacion inicial y luego una vez que se acabe que se active el aim 
    }
    private void OnDestroy()
    {
        GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }
}
