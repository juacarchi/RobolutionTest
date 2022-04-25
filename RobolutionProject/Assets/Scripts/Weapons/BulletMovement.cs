using UnityEngine;
using System.Collections.Generic;
public class BulletMovement : MonoBehaviour, IPooledObject
{
    public TrailRenderer trail;
    Weapon actualWeapon;
    Vector3 direction;
    float speed = 10f;
    float timeToDestroy = 3.5f;
    [SerializeField] bool isShootingPlayer;
    List<Transform> enemiesTransforms;
    int i;

    public void OnObjectSpawn()
    {
        if (PlayerController.instance.isChainedBulletActivated)
        {
            speed = 12;
        }
      
        Invoke("DesactiveBullet", timeToDestroy); //DESTRUIR CUANDO CHOQUE CON ALGO O AL TIEMPO DE INSTANCIAR
    }
    public void DesactiveBullet()
    {
        this.gameObject.SetActive(false);

        if(trail!=null) trail.Clear();
    }
    public void SetEnemiesTransform(List<Transform> enemiesTransforms)
    {
        this.enemiesTransforms = enemiesTransforms;
    }
    public void SetDirection(Vector3 direction)
    {
        this.transform.forward = direction.normalized;
    }
    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
    public void SetActualWeapon(Weapon actualWeapon)
    {
        this.actualWeapon = actualWeapon;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isShootingPlayer)
        {
            if (other.CompareTag("Enemy"))
            {
                if (PlayerController.instance.isChainedBulletActivated)
                {
                    for (int i = 0; i < enemiesTransforms.Count; i++)
                    {
                        if(other.transform == enemiesTransforms[i])
                        {
                            enemiesTransforms.Remove(enemiesTransforms[i]);
                        }
                    }
                    EnemyBase enemy = other.GetComponent<EnemyBase>();
                    enemy.Hit(actualWeapon.Damage);
                    
                    enemy.GetAgent().velocity = (other.transform.position - this.transform.position).normalized * 2;
                    if (i < enemiesTransforms.Count)
                    {
                        SetDirection(enemiesTransforms[i].transform.position - transform.position);
                        i++;
                    }
                    else
                    {
                        DesactiveBullet();
                    }
                   
                }
                else
                {
                    EnemyBase enemy = other.GetComponent<EnemyBase>();
                    enemy.Hit(GameManager.instance.GetActualWeapon().Damage);
                    enemy.GetAgent().velocity = (other.transform.position - this.transform.position).normalized * 2;
                    DesactiveBullet();
                }
                
            }
        }
        else
        {
            if (other.CompareTag("Player"))
            {
                PlayerController.instance.Hit(actualWeapon.Damage);
                DesactiveBullet();
            }
        }
    }
    public void SetIsShootingPlayer(bool active)
    {
        isShootingPlayer = active;
    }
}
