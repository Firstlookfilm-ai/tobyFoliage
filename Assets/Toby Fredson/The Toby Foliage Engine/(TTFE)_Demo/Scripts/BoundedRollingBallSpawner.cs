using UnityEngine;

namespace TobyFredson.SceneScripts
{
	[AddComponentMenu("Toby Fredson/The Toby Foliage Engine/Scene/Bounded Rolling Ball Spawner")]
	public class BoundedRollingBallSpawner : MonoBehaviour
	{
		[SerializeField] private GameObject ballPrefab; // Custom prefab to spawn
		[SerializeField] private int numberOfBalls = 50; // Number of balls to spawn
		[SerializeField] private float spawnRadius = 5f; // Radius of the spawn area
		[SerializeField] private float minBallRadius = 0.5f; // Minimum ball size
		[SerializeField] private float maxBallRadius = 2f; // Maximum ball size
		[SerializeField] private float initialForce = 5f; // Initial force for rolling
		[SerializeField] private float movementSpeed = 2f; // Consistent movement speed
		[SerializeField] private float directionChangeInterval = 2f; // Time between direction changes
		[SerializeField] private float smoothTurnTime = 0.5f; // Time to smoothly interpolate direction
		[SerializeField] private Material ballMaterial; // Optional: Assign a material in Inspector
		[SerializeField] private float groundHeight = 0f; // Height of the flat surface (e.g., Y = 0)

		void Start()
		{
			if (ballPrefab == null)
			{
				return; // Silently exit if prefab is not assigned
			}

			SpawnBalls();
		}

		void SpawnBalls()
		{
			for (int i = 0; i < numberOfBalls; i++)
			{
				// Instantiate the custom prefab at a random position within the spawn radius
				Vector3 randomPosition = transform.position + Random.insideUnitSphere * spawnRadius;
				randomPosition.y = groundHeight; // Set to ground height for flat surface
				GameObject ball = Instantiate(ballPrefab, randomPosition, Quaternion.identity);

				// Randomize scale
				float radius = Random.Range(minBallRadius, maxBallRadius);
				ball.transform.localScale = ball.transform.localScale * radius;

				// Get or add Rigidbody for physics
				Rigidbody rb = ball.GetComponent<Rigidbody>();
				if (rb == null)
				{
					rb = ball.AddComponent<Rigidbody>();
				}
				rb.mass = radius; // Mass scales with size
				rb.useGravity = true;
				rb.angularDamping = 0.5f; // Slight angular drag for smoother rolling

				// Apply initial random force (XZ plane) for non-kinematic Rigidbodies
				if (!rb.isKinematic)
				{
					Vector3 randomDirection = new Vector3(
						Random.Range(-1f, 1f),
						0f,
						Random.Range(-1f, 1f)
					).normalized;
					rb.AddForce(randomDirection * initialForce, ForceMode.Impulse);
				}

				// Add BoundedRoller component for constant motion and boundary
				BoundedRoller roller = ball.AddComponent<BoundedRoller>();
				roller.Initialize(movementSpeed, directionChangeInterval, smoothTurnTime, spawnRadius, groundHeight, transform.position);

				// Apply random color or material
				Renderer renderer = ball.GetComponent<Renderer>();
				if (renderer != null)
				{
					if (ballMaterial != null)
					{
						renderer.material = ballMaterial;
					}
					else
					{
						renderer.material.color = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
					}
				}
			}
		}

		// Nested class to apply constant motion with smooth direction changes and enforce boundary
		public class BoundedRoller : MonoBehaviour
		{
			private Rigidbody rb;
			private float movementSpeed;
			private float directionChangeInterval;
			private float smoothTurnTime;
			private float timer;
			private Vector3 currentDirection;
			private Vector3 targetDirection;
			private float turnProgress;
			private Vector3 centerPosition;
			private float maxDistance;
			private float groundHeight;
			private bool isKinematic;

			public void Initialize(float speed, float changeInterval, float turnTime, float spawnRadius, float groundLevel, Vector3 spawnCenter)
			{
				rb = GetComponent<Rigidbody>();
				movementSpeed = speed;
				directionChangeInterval = changeInterval;
				smoothTurnTime = turnTime;
				centerPosition = spawnCenter;
				maxDistance = spawnRadius;
				groundHeight = groundLevel;
				isKinematic = rb != null && rb.isKinematic; // Check if kinematic
				timer = Random.Range(0f, directionChangeInterval); // Randomize start time to desynchronize
				SetNewRandomDirection();
			}

			void FixedUpdate()
			{
				timer += Time.fixedDeltaTime;
				if (timer >= directionChangeInterval)
				{
					SetNewRandomDirection();
					timer = 0f;
				}

				// Smoothly interpolate direction
				turnProgress += Time.fixedDeltaTime / smoothTurnTime;
				currentDirection = Vector3.Slerp(currentDirection, targetDirection, turnProgress);
				if (turnProgress >= 1f)
				{
					turnProgress = 0f; // Reset for next turn
				}

				if (rb != null)
				{
					if (isKinematic)
					{
						// Kinematic: Move by updating transform position
						Vector3 targetVelocity = currentDirection * movementSpeed;
						Vector3 newPosition = transform.position + targetVelocity * Time.fixedDeltaTime;

						// Constrain within spawn radius
						Vector3 displacement = newPosition - centerPosition;
						displacement.y = 0f; // Ignore vertical displacement
						if (displacement.magnitude > maxDistance)
						{
							Vector3 correction = displacement.normalized * (displacement.magnitude - maxDistance);
							newPosition -= new Vector3(correction.x, 0f, correction.z);
						}

						// Ensure ground height
						newPosition.y = groundHeight;
						transform.position = newPosition;
					}
					else
					{
						// Non-kinematic: Apply force to maintain motion
						Vector3 targetVelocity = currentDirection * movementSpeed;
						Vector3 velocityDelta = targetVelocity - rb.linearVelocity;
						rb.AddForce(velocityDelta / Time.fixedDeltaTime, ForceMode.Force); // Continuous adjustment

						// Constrain movement within spawn radius
						Vector3 displacement = rb.position - centerPosition;
						displacement.y = 0f; // Ignore vertical displacement
						if (displacement.magnitude > maxDistance)
						{
							Vector3 correction = displacement.normalized * (displacement.magnitude - maxDistance);
							rb.position -= new Vector3(correction.x, 0f, correction.z);
							rb.linearVelocity = Vector3.ProjectOnPlane(rb.linearVelocity, Vector3.up); // Preserve horizontal velocity
						}

						// Ensure ball stays on ground
						if (rb.position.y != groundHeight)
						{
							rb.position = new Vector3(rb.position.x, groundHeight, rb.position.z);
							rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // Reset vertical velocity
						}
					}
				}
			}

			private void SetNewRandomDirection()
			{
				targetDirection = new Vector3(
					Random.Range(-1f, 1f),
					0f,
					Random.Range(-1f, 1f)
				).normalized;
				turnProgress = 0f; // Start smooth turn
			}
		}
	}
}