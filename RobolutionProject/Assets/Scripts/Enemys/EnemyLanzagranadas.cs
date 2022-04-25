using UnityEngine;

public class EnemyLanzagranadas : MonoBehaviour
{
    public EnemyBase enemyBase;
    public Transform canyon;
    // Start is called before the first frame update
    public void Shoot()
    {
        float timeThrowingShoot = 1.5f; // TODO: Parámetro de weapon
        Vector3 distance = enemyBase.follow.position - transform.position;
        this.transform.forward = distance;
        Vector3 distanceXZ = distance;
        distanceXZ.y = 0f;
        float Sy = distance.y;
        float Sxz = distanceXZ.magnitude;
        float Vxz = Sxz / timeThrowingShoot;

        float Vy = Sy / timeThrowingShoot + 0.5f * (Physics.gravity.y > 0 ? Physics.gravity.y : -Physics.gravity.y) * timeThrowingShoot;

        Vector3 result = distanceXZ.normalized;

        result *= Vxz;
        result.y = Vy;

        GameObject grenadeBullet = ObjectPooler.instance.SpawnFromPool("GrenadeEnemy", canyon.transform.position, Quaternion.identity);
        Rigidbody rbBullet = grenadeBullet.GetComponent<Rigidbody>();
        rbBullet.velocity = result;
    }
}
