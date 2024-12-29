using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    private float score;
    public TextMeshProUGUI scoreUI;
    private int highscore;

    public Transform player;
    public PlayerMovement movement;

    public List<GameObject> obstaclePrefabs; // List of obstacle prefabs
    public Transform obstacles;
    public float obstacleStartX = 100;
    public float obstacleStartY = 1;
    public float globalObstacleSpeed = 10f;
    public float speedMultiplier = 2f; // Speed multiplier when "W" is pressed

    public float initialSpawnInterval = 1.0f; // Spawn interval adjustable in Editor
    public float minSpawnInterval = 0.3f;    // Minimum spawn interval

    public void InitiateDeath()
    {
        // Reload the current scene to restart the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void UpdateHighscore()
    {
        highscore = PlayerPrefs.GetInt("Highscore" + 1);

        if (score > highscore)
        {
            highscore = (int)score;
            PlayerPrefs.SetInt("Highscore" + 1, highscore);
        }
    }

    private void Spawn()
    {
        for (int i = -7; i < 7; i += 7)
        {
            // Pick a random prefab from the list
            GameObject randomPrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count)];

            // Define the correct rotation for the prefab (facing forward)
            Quaternion correctRotation = Quaternion.Euler(0, 180, 0); // Rotate 180° around the Y-axis

            // Spawn the random prefab with the correct rotation
            GameObject spawnedObstacle = Instantiate(randomPrefab,
                new Vector3(Mathf.Floor(Random.Range(i, i + 7)), obstacleStartY, obstacleStartX),
                correctRotation, obstacles);
            
            spawnedObstacle.layer = LayerMask.NameToLayer("Obstacle");

            // Add or verify Rigidbody
            Rigidbody rb = spawnedObstacle.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = spawnedObstacle.AddComponent<Rigidbody>();
            }
            rb.mass = 1;
            rb.linearDamping = 0;
            rb.angularDamping = 0;
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            // Freeze rotation to prevent rolling
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

            // Add or verify ObstacleMovement script
            ObstacleMovement movementScript = spawnedObstacle.GetComponent<ObstacleMovement>();
            if (movementScript == null)
            {
                movementScript = spawnedObstacle.AddComponent<ObstacleMovement>();
            }
            movementScript.force = 500f;
            movementScript.collisionVelocity = new Vector3(25f, 5f, 10f);
        }
    }

    private void Update()
    {
        if (FindObjectOfType<PlayerMovement>().enabled)
        {
            // Update score
            score += Time.deltaTime * 10;
            scoreUI.text = "Score: " + (int)score;

            // Adjust obstacle speed when "W" is pressed
            if (Input.GetKey(KeyCode.W))
            {
                globalObstacleSpeed = 20f * speedMultiplier; // Speed up
            }
            else
            {
                globalObstacleSpeed = 20f; // Default speed
            }
        }
    }

    private IEnumerator SpawnObstacles()
    {
        float spawnInterval = initialSpawnInterval;

        while (true)
        {
            Spawn();

            // Dynamically adjust spawn interval but clamp to the minimum value
            spawnInterval = Mathf.Max(minSpawnInterval, spawnInterval - 0.005f);

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void Start()
    {
        // Start the spawn coroutine
        StartCoroutine(SpawnObstacles());
    }
}