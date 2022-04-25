using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEnemy : MonoBehaviour, IPooledObject
{
    public float timeToDesactivate;
    public TrailRenderer trail;
    Weapon actualWeapon;
    public float speed = 10f;
    int damageBullet;
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
    public void SetDamageBullet(int damage)
    {
        damageBullet = damage;
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
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController.instance.Hit(damageBullet);
            DesactiveBullet();
        }
    }
    private void OnDestroy()
    {
        GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }
}
