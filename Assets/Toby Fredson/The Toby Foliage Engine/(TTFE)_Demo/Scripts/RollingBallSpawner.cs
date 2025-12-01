using UnityEngine;

namespace TobyFredson.SceneScripts
{
	[AddComponentMenu("Toby Fredson/The Toby Foliage Engine/Scene/Rolling Ball Spawner")]
	public class RollingBallSpawner : MonoBehaviour
	{
		[SerializeField] private GameObject ballPrefab; // Custom prefab to spawn
		[SerializeField] private int numberOfBalls = 50; // Number of balls to spawn
		[SerializeField] private float minBallScale = 0.5f; // Minimum scale multiplier
		[SerializeField] private float maxBallScale = 2f; // Maximum scale multiplier
		[SerializeField] private float explosionForce = 5f; // Initial explosion force at spawn
		[SerializeField] private float randomForceStrength = 3f; // Strength of random forces for continuous rolling
		[SerializeField] private float forceInterval = 2f; // Time between random force applications

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
				// Instantiate the custom prefab at the GameObject's position
				GameObject ball = Instantiate(ballPrefab, transform.position, Quaternion.identity);

				// Randomize scale
				float scale = Random.Range(minBallScale, maxBallScale);
				ball.transform.localScale = ball.transform.localScale * scale;

				// Get or add Rigidbody for physics
				Rigidbody rb = ball.GetComponent<Rigidbody>();
				if (rb == null)
				{
					rb = ball.AddComponent<Rigidbody>();
				}
				rb.mass = scale; // Mass scales with size
				rb.useGravity = true;
				rb.angularDamping = 0.5f; // Slight angular drag for smoother rolling

				// Apply initial explosion force (random direction in XZ plane)
				Vector3 randomDirection = new Vector3(
					Random.Range(-1f, 1f),
					0f,
					Random.Range(-1f, 1f)
				).normalized;
				rb.AddForce(randomDirection * explosionForce, ForceMode.Impulse);

				// Add RandomRoller component for continuous random movement
				ball.AddComponent<RandomRoller>().Initialize(randomForceStrength, forceInterval);
			}
		}
	}

	public class RandomRoller : MonoBehaviour
	{
		private Rigidbody rb;
		private float forceStrength;
		private float interval;
		private float timer;

		public void Initialize(float strength, float forceInterval)
		{
			rb = GetComponent<Rigidbody>();
			forceStrength = strength;
			interval = forceInterval;
			timer = Random.Range(0f, interval); // Randomize start time to desynchronize
		}

		void Update()
		{
			timer += Time.deltaTime;
			if (timer >= interval)
			{
				// Apply random force in XZ plane
				Vector3 randomDirection = new Vector3(
					Random.Range(-1f, 1f),
					0f,
					Random.Range(-1f, 1f)
				).normalized;
				if (rb != null)
				{
					rb.AddForce(randomDirection * forceStrength, ForceMode.Impulse);
				}
				timer = 0f;
			}
		}
	}
}