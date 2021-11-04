using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Buff : ScriptableObject
{
    public virtual void OnAttack(Vector2 position, Vector2 direction) { }
    public virtual void OnHit(Vector2 position, Vector2 direction, Enemy enemy, float damage) { }
    public virtual void OnKill(Vector2 position, Enemy enemy) { }
    public virtual void OnRoomEnd() { }

    public abstract void Init();
}
