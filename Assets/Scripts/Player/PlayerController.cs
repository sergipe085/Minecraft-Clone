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

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update() {
        CaptureInput();
        Move();
        Jump();
        camera.MouseLook(currentInput.xLook, currentInput.yLook);

        if (IsGrounded() && !lastIsGrounded) {
            Land();
        }
        lastIsGrounded = IsGrounded();
    }

    private void Move() {
        orientation.transform.rotation = Quaternion.Euler(0f, camera.transform.eulerAngles.y, 0f);

        Vector3 moveDirection = currentInput.xMove * orientation.transform.right + currentInput.zMove * orientation.transform.forward;
        moveDirection.Normalize();
        moveDirection *= moveSpeed;
        moveDirection *= Time.fixedDeltaTime;
        rig.velocity = new Vector3(moveDirection.x, rig.velocity.y, moveDirection.z);

        GetComponentInChildren<Animator>().SetBool("walking", currentInput.xMove != 0 || currentInput.zMove != 0);
    }

    private void Jump() {
        if (!currentInput.jump || !IsGrounded()) {
            return;
        }
        rig.velocity = new Vector3(rig.velocity.x, jumpSpeed * Time.fixedDeltaTime, rig.velocity.z);
    }

    private void Land() {
        
    }

    private bool IsGrounded() {
        bool BLchecker = Physics.Raycast(new Vector3(transform.position.x - 0.35f, transform.position.y, transform.position.z - 0.35f), Vector3.down, 1f, groundLayer);
        bool TLchecker = Physics.Raycast(new Vector3(transform.position.x - 0.35f, transform.position.y, transform.position.z + 0.35f), Vector3.down, 1f, groundLayer);
        bool BRchecker = Physics.Raycast(new Vector3(transform.position.x + 0.35f, transform.position.y, transform.position.z - 0.35f), Vector3.down, 1f, groundLayer);
        bool TRchecker = Physics.Raycast(new Vector3(transform.position.x + 0.35f, transform.position.y, transform.position.z + 0.35f), Vector3.down, 1f, groundLayer);
        bool Mchecker  = Physics.Raycast(transform.position, Vector3.down, 1f, groundLayer);

        return BLchecker || TLchecker || BRchecker || TRchecker || Mchecker;
    }

    private void CaptureInput() {
        currentInput.xMove  = Input.GetAxisRaw("Horizontal");
        currentInput.zMove  = Input.GetAxisRaw("Vertical");
        currentInput.xLook  = Input.GetAxis("Mouse X");
        currentInput.yLook  = Input.GetAxis("Mouse Y");
        currentInput.jump   = Input.GetButton("Jump");
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

    public void OnCreated() {
        transform.position = new Vector3(transform.position.x, 256, transform.position.z);
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer)) {
            transform.position = hit.point + Vector3.up;
        }
    }
}
