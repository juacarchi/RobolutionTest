using UnityEngine;
using System.Collections.Generic;
public class ChainedBullet : MonoBehaviour, IPooledObject
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
        else if (!isTracking && isEnchaited && !isEnchaint)
        {
            if (enemiesToEnchaint == null)
            {
                Debug.Log("Error al pasar la lista");
                return;
            }
            enemyTracking = EnchainEnemies();
        }
        else if (!isTracking && isEnchaited && isEnchaint)
        {
            transform.Translate(enemyTracking.transform.position * speed * Time.deltaTime);
        }
        //meter aqui el codigo especial de las balas
        //para que la bala haga su efecto especiaal.


    }

    private GameObject EnchainEnemies()
    {
        if (enemiesEnchainted.Capacity == enemiesToEnchaint.Capacity)
        {
            Destroy(gameObject);
        }


        GameObject enemyNearest = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (var enemy in enemiesToEnchaint)
        {
            Vector3 directionToTarget = enemy.GetComponent<Transform>().position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;

            if (dSqrToTarget < closestDistanceSqr && !enemiesEnchainted.Contains(enemy)) //si no esta en la lista se actualiza
            {

                closestDistanceSqr = dSqrToTarget;
                enemyNearest = enemy;
            }
        }
        if (closestDistanceSqr > maxDistance) // si la distancia mas pequeña que detecta es mayor que la distancia maxima que puede saltar la bala se destruye.
        {
            Destroy(gameObject);
        }

        enemiesEnchainted.Add(enemyNearest);
        isEnchaint = true;
        return (enemyNearest);

        //aqui tengo que ir comparando todo el rato los enemigos en ambas listas para no pasar dos veces por el mismo 
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
            Debug.Log("ERROR al golpear enemigo, he golpeado a "+other.name);
        }
    }

    public void ActiveEnchaitedbullet(List<GameObject> listEnemies, float range)
    {

        enemiesToEnchaint = listEnemies;
        maxDistance = range;
        isEnchaited = true;

    }
    
    private void OnDestroy()
    {
        GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

}
