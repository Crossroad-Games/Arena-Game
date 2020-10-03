using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKillable
{
    void CmdKill();
}

public interface IDamageable<T>
{
    void CmdTakeDamage(T damageTaken);
}