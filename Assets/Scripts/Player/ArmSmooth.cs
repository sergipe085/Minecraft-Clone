using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmSmooth : MonoBehaviour
{
    Vector3 initialPosition = Vector3.zero;

    private void Start() {
        initialPosition = transform.localPosition;
    }

    private void LateUpdate() {
        float xInput = -Input.GetAxis("Mouse X") * Time.deltaTime;
        float yInput = -Input.GetAxis("Mouse Y") * Time.deltaTime;

        Vector3 finalPosition = new Vector3(xInput, yInput);

        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + initialPosition, 5f * Time.deltaTime);
    }
}
