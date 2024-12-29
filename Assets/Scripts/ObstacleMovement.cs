using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
	public float force = 500f;
	// Collision velocity (X - angular multiplier)
	public Vector3 collisionVelocity = new Vector3(25f, 5f, 10f);

	private void OnCollisionEnter(Collision collision)
	{
		switch (collision.gameObject.tag)
		{
			case "Player":
				// Throw obstacle into the air for dramatic effect
				Rigidbody obstacle = GetComponent<Rigidbody>();
				obstacle.linearVelocity = new Vector3(
					obstacle.linearVelocity.x,
					collisionVelocity.y,
					collisionVelocity.z
				);
				obstacle.angularVelocity *= collisionVelocity.x;

				FindObjectOfType<GameManager>().InitiateDeath();
				break;

			case "BackWall":
				Destroy(gameObject);
				break;
		}
	}

	private void Update()
	{
		Rigidbody obstacle = GetComponent<Rigidbody>();
		float adjustedSpeed = FindObjectOfType<GameManager>().globalObstacleSpeed;
		obstacle.linearVelocity = new Vector3(0f, 0f, -adjustedSpeed);
	}
}
