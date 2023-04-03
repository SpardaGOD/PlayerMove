using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Variáveis de movimento
    public float moveSpeed = 5.0f;
    public float runSpeed = 10.0f;
    public float jumpForce = 7.5f;
    public float gravity = 20.0f;
    public float crouchSpeed = 2.0f;

    // Variáveis de câmera
    public Transform cam;
    public float camDistance = 5.0f;
    public float camHeight = 2.0f;
    public float camSensitivity = 3.0f;

    // Outras variáveis
    private CharacterController charController;
    private Vector3 moveDirection = Vector3.zero;
    private bool isRunning = false;
    private bool isCrouching = false;
    private float rotationSpeed = 0.0f;

    void Start()
    {
        charController = GetComponent<CharacterController>();

        // Esconder e travar o cursor do mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Movimento básico
        float horiz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");

        Vector3 forward = cam.forward;
        Vector3 right = cam.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = (forward * vert + right * horiz).normalized;

        if (charController.isGrounded)
        {
            moveDirection = moveDir * moveSpeed;

            // Pulo
            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpForce;
            }

            // Correr
            if (Input.GetKey(KeyCode.LeftShift))
            {
                isRunning = true;
                moveDirection *= runSpeed;
            }
            else
            {
                isRunning = false;
            }

            // Agachar
            if (Input.GetKeyDown(KeyCode.C))
            {
                isCrouching = !isCrouching;
                charController.height = isCrouching ? 1.0f : 2.0f;
                moveSpeed = isCrouching ? crouchSpeed : 5.0f;
            }
        }
        else
        {
            // Aplicar gravidade
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Movimentar o personagem
        charController.Move(moveDirection * Time.deltaTime);

        // Rotacionar o personagem
        float mouseX = Input.GetAxis("Mouse X") * camSensitivity;
        transform.Rotate(Vector3.up, mouseX);

        // Movimentar a câmera
        float mouseY = Input.GetAxis("Mouse Y") * camSensitivity;
        Vector3 camAngle = cam.rotation.eulerAngles;
        camAngle.x -= mouseY;

        if (camAngle.x > 180)
        {
            camAngle.x -= 360;
        }

        camAngle.x = Mathf.Clamp(camAngle.x, -80.0f, 80.0f);
        camAngle.y = transform.rotation.eulerAngles.y;

        Quaternion camRotation = Quaternion.Euler(camAngle);
        Vector3 camPosition = transform.position - camRotation * Vector3.forward * camDistance;
        camPosition.y += camHeight;

        cam.rotation = camRotation;
        cam.position = camPosition;
    }
}