using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu (fileName = "EnemyData", menuName = "EnemyData")]
public class EnemyData : ScriptableObject
{
    [SerializeField] string nameEnemy;
    [SerializeField] int healthMax;
    [SerializeField] [Min(0)] float fovDepth;
    [SerializeField] [Min(0)] float fovAngle;
    [SerializeField] [Min(0)] float alertRange;
    [SerializeField] bool turnBackHome;
    [SerializeField] bool patrol;
    [SerializeField] float forgettingTime;
    [SerializeField] Weapon weapon;

    public string NameEnemy { get => nameEnemy; set => nameEnemy = value; }
    public int HealthMax { get => healthMax; set => healthMax = value; }
    public float FovDepth { get => fovDepth; set => fovDepth = value; }
    public float FovAngle { get => fovAngle; set => fovAngle = value; }
    public float AlertRange { get => alertRange; set => alertRange = value; }
    public bool TurnBackHome { get => turnBackHome; set => turnBackHome = value; }
    public bool Patrol { get => patrol; set => patrol = value; }
    public float ForgettingTime { get => forgettingTime; set => forgettingTime = value; }
    public Weapon Weapon { get => weapon; set => weapon = value; }
}
