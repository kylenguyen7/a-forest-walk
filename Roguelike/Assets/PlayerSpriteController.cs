using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpriteController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        experimentalTilt();
    }

    private void experimentalTilt() {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        float targetZRot = 0f;
        if (horizontal != 0) {
            targetZRot = -horizontal * 3f;
        }

        float currentZRot = transform.eulerAngles.z < 180 ? transform.eulerAngles.z :
                                                            transform.eulerAngles.z - 360;

        float rotateAmountZ = (targetZRot - currentZRot) / 10;
        transform.Rotate(0, -transform.eulerAngles.y, rotateAmountZ, Space.World);
    }

    private void experimentalTiltIncludeX() {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        float targetZRot = 0f;
        float targetXRot = 0f;
        if (vertical < 0) {
            targetXRot = vertical * 15f;
        } else if (horizontal != 0) {
            targetZRot = -horizontal * 5f;
        } else if (vertical > 0) {
            targetXRot = vertical * 15f;
        }

        float currentZRot = transform.eulerAngles.z < 180 ? transform.eulerAngles.z :
                                                            transform.eulerAngles.z - 360;

        float currentXRot = transform.eulerAngles.x < 180 ? transform.eulerAngles.x :
                                                            transform.eulerAngles.x - 360;

        float rotateAmountZ = (targetZRot - currentZRot) / 10;
        float rotateAmountX = (targetXRot - currentXRot) / 10;
        transform.Rotate(rotateAmountX, -transform.eulerAngles.y, rotateAmountZ, Space.World);
    }
}
