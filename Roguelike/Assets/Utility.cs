using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : MonoBehaviour
{
    public static BlackScreenController Transition() {
        var black = Instantiate(Resources.Load("Prefabs/Transition", typeof(GameObject)) as GameObject,
                                MasterCanvas.instance.transform);

        return black.GetComponent<BlackScreenController>();
    }

    public static Vector2 GetHomingDir(Vector2 currentDir, Vector2 currentPos, Vector2 targetPos, float rotateScale) {

        // Rotate amount is a value on [-1, 1]
        // -1 indicates the target direction is directly counterclockwise
        // 1 indicates the target direction is directly clockwise
        Vector2 targetDir = ((Vector2)targetPos - currentPos);
        float rotateAmount = Vector3.Cross(targetDir.normalized, currentDir.normalized).z;

        // Angle is calculated from travelDirection and incremented by deltaAngle,
        // which is an artificial scale of rotateAmount
        float deltaAngle = -rotateAmount * rotateScale;

        float currentAngle = Vector2.SignedAngle(new Vector2(1, 0), currentDir);
        if (currentAngle < 0) {
            currentAngle = currentAngle + 360;
        }

        float targetAngle = currentAngle + deltaAngle;

        // Resulting angle is converted back to travel direction
        return new Vector2(Mathf.Cos(targetAngle * Mathf.Deg2Rad),
                           Mathf.Sin(targetAngle * Mathf.Deg2Rad)).normalized;
    }

    public static void CreateDamageText(float damage, Vector2 origin, bool crit, StatusEffect.effects effect) {
        // Create damaged text
        var damageText = Resources.Load("Prefabs/Damage Text", typeof(GameObject)) as GameObject;
        Instantiate(damageText, origin, Quaternion.identity)
            .GetComponent<DamageTextController>().Init(damage, crit, effect);
    }
}
