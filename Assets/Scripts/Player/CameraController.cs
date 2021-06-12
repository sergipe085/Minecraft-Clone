using UnityEngine;

public class CameraController : MonoBehaviour {
    [SerializeField] private float sensitivity = 5.0f;

    float xTotal = 0.0f, yTotal = 0.0f;

    public void MouseLook(float xInput, float yInput) {
        xTotal += xInput * Time.fixedDeltaTime * sensitivity;
        yTotal += yInput * Time.fixedDeltaTime * sensitivity;
        yTotal  = Mathf.Clamp(yTotal, -90, 90);

        transform.rotation = Quaternion.Euler(-yTotal, xTotal, 0.0f);
    }
}