using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyException : Exception
{
    public EnemyException() { }

    public EnemyException(string mesage) : base(mesage) { }

    public EnemyException(string mesage, Exception inner) : base(mesage, inner) { }
}
