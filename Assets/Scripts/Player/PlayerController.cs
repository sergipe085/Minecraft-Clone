using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("MOVEMENT")]
    [SerializeField] private float     moveSpeed   = 5.0f;
    [SerializeField] private float     jumpSpeed   = 4.0f;
    [SerializeField] private LayerMask groundLayer;
    private InputStructure currentInput;
    private bool lastIsGrounded = false;
    private bool isGrounded     = true;

    [Header("COMPONENTS")]
    private Rigidbody           rig        = null;
    private CameraController    camera     = null;

    [Header("HELPER")]
    private GameObject orientation;

    private void Awake() {
        rig          = GetComponent<Rigidbody>();
        camera       = GetComponentInChildren<CameraController>();

        currentInput = new InputStructure();
        orientation  = new GameObject("Player Orientation");
        orientation.transform.SetParent(transform);
    }

    private void Update() {
        CaptureInput();
        Move();
        Jump();
        camera.MouseLook(currentInput.xLook, currentInput.yLook);

        if (isGrounded && !lastIsGrounded) {
            Land();
        }
        lastIsGrounded = isGrounded;
    }

    private void Move() {
        orientation.transform.rotation = Quaternion.Euler(0f, camera.transform.eulerAngles.y, 0f);

        Vector3 moveDirection = currentInput.xMove * orientation.transform.right + currentInput.zMove * orientation.transform.forward;
        moveDirection.Normalize();
        moveDirection *= moveSpeed;
        moveDirection *= Time.fixedDeltaTime;
        rig.velocity = new Vector3(moveDirection.x, rig.velocity.y, moveDirection.z);
    }

    private void Jump() {
        if (!currentInput.jump || !isGrounded) {
            return;
        }
        rig.velocity = new Vector3(rig.velocity.x, jumpSpeed * Time.fixedDeltaTime, rig.velocity.z);
    }

    private void Land() {
        
    }

    private void CaptureInput() {
        currentInput.xMove  = Input.GetAxisRaw("Horizontal");
        currentInput.zMove  = Input.GetAxisRaw("Vertical");
        currentInput.xLook  = Input.GetAxis("Mouse X");
        currentInput.yLook  = Input.GetAxis("Mouse Y");
        currentInput.jump   = Input.GetButton("Jump");
    }

    private void OnCollisionStay(Collision other) {
        if (IsGround(other)) {
            isGrounded = true;
        }
        else {
            isGrounded = false;
        }
    }

    private void OnCollisionExit(Collision other) {
        isGrounded = false;
    }

    private bool IsGround(Collision other) {
        int groundLayerIndex = (int)Mathf.Log(LayerMask.GetMask("Ground"), 2);

        bool isLayer  = other.collider.transform.gameObject.layer == groundLayerIndex;
        if (!isLayer || other.contactCount == 0) return false;

        for (int i = 0; i < other.contactCount; i++) {
            if (other.contacts[i].normal == Vector3.up) {
                return true;
            }
        }

        return false;
    }
}
