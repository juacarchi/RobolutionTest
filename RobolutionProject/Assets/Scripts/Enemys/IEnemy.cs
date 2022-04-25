using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    public void Hit(int damage);
    public void Stun(float time);
}
