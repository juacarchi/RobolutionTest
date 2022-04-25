using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rafagax3 : MonoBehaviour
{
    public EnemyBase enemyBase;
    public Transform rightCanyon;
    public Transform leftCanyon;

    public void FirstShoot()
    {
        Vector3 dirAttack = enemyBase.follow.transform.position - transform.position;
        this.transform.forward = dirAttack; //Orientar al enemigo hacia el jugador cuando dispara
        dirAttack.x += Random.Range(-enemyBase.EnemyData.Weapon.Dispersion, enemyBase.EnemyData.Weapon.Dispersion);
        GameObject instance = ObjectPooler.instance.SpawnFromPool(enemyBase.EnemyData.Weapon.Tag, transform.position, Quaternion.identity);
        instance.transform.forward = dirAttack.normalized;
        instance.GetComponent<BulletEnemy>().SetDamageBullet(enemyBase.EnemyData.Weapon.Damage);
    }
    public void SecondShoot()
    {
        Vector3 dirAttack = enemyBase.follow.transform.position - transform.position;
        GameObject instance2 = ObjectPooler.instance.SpawnFromPool(enemyBase.EnemyData.Weapon.Tag, new Vector3(rightCanyon.transform.position.x, 1, rightCanyon.transform.position.z), Quaternion.identity);
        instance2.transform.forward = dirAttack.normalized;
        instance2.GetComponent<BulletEnemy>().SetDamageBullet(enemyBase.EnemyData.Weapon.Damage);
    }
}
