using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    public static BuffManager instance;
    private List<Buff> globalBuffs = new List<Buff>();

    private void Awake() {
        if (instance != null) return;
        instance = this;
    }

    private void Start() {
        // AddBuff(ScriptableObject.CreateInstance<BuffMagicMissile>());
        // AddBuff(ScriptableObject.CreateInstance<BuffLifesteal>());
    }

    public void AddBuff(Buff buff) {
        buff.Init();
        globalBuffs.Add(buff);
    }

    public void RemoveBuff(Buff buff) {
        globalBuffs.Remove(buff);
    }

    public void OnAttack(Vector2 position, Vector2 direction) {
        foreach(Buff buff in globalBuffs) {
            buff.OnAttack(position, direction);
        }

        // Debug.Log("OnAttack()");
    }

    public void OnHit(Vector2 position, Vector2 direction, Enemy enemy, float damage) {
        foreach (Buff buff in globalBuffs) {
            buff.OnHit(position, direction, enemy, damage);
        }

        // Debug.Log("OnHit()");
    }

    public virtual void OnKill(Vector2 position, Enemy enemy) {
        foreach (Buff buff in globalBuffs) {
            buff.OnKill(position, enemy);
        }

        // Debug.Log("OnKill()");
    }

    public void OnRoomEnd() {
        foreach (Buff buff in globalBuffs) {
            buff.OnRoomEnd();
        }

        // Debug.Log("OnRoomEnd()");
    }
}
