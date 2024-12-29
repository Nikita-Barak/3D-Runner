using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float force = 200f; // Force for horizontal movement
    public float jumpForce = 300f; // Force for jumping
    private bool isGrounded = true; // To check if the player is on the ground

    private void FixedUpdate()
    {
        Rigidbody player = GetComponent<Rigidbody>();

        // Move player on the X axis (sideways)
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            player.AddForce(force * Time.deltaTime, 0f, 0f, ForceMode.VelocityChange);
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            player.AddForce(-force * Time.deltaTime, 0f, 0f, ForceMode.VelocityChange);
        }

        // Jump on Space if grounded
        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            player.AddForce(0f, jumpForce * Time.deltaTime, 0f, ForceMode.Impulse);
            isGrounded = false; // Prevent jumping until grounded again
        }

        // Add additional downward force when falling
        if (!isGrounded)
        {
            player.AddForce(0f, -9.81f * 2f, 0f); // Apply extra gravity (tweak the multiplier as needed)
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the player is on the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}