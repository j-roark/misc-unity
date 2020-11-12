using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 6f;
    public float gravity = 14.0f;
    public float verticalVelocity = 0.0f;
    public float jumpForce = 10.0f;
    public float runSpeedModifier = 1.5f;
    float calculatedSpeed;

    void FixedUpdate()
    {
        if (controller.isGrounded)
        {
            if (Input.GetButtonDown("Jump")){
                verticalVelocity = jumpForce;
            }
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }
        if (Input.GetButton("Run")) { calculatedSpeed = speed * runSpeedModifier; }
        else { calculatedSpeed = speed; }
        if (controller.isGrounded && verticalVelocity < 0) { verticalVelocity = 0; }
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, verticalVelocity, vertical);
        if (direction.magnitude >= 0.1f)
        {
            controller.Move(direction * calculatedSpeed * Time.deltaTime);
        }
    }
}
