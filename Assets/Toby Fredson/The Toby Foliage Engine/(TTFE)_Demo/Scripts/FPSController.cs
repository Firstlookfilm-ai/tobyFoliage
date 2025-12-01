using UnityEngine;
using System.Collections.Generic; // Added for List

namespace TobyFredson.SceneScripts
{
	[AddComponentMenu("Toby Fredson/The Toby Foliage Engine/Scene/FPS Controller")]
	[RequireComponent(typeof(CharacterController))]
	public class FPSController : MonoBehaviour
	{
		[SerializeField] private Camera playerCamera;
		[SerializeField] private float walkSpeed = 6f;
		[SerializeField] private float runSpeed = 12f;
		[SerializeField] private float jumpPower = 7f;
		[SerializeField] private float gravity = 10f;
		[SerializeField] private float lookSpeed = 2f;
		[SerializeField] private float lookXLimit = 45f;

		[Header("Shooting Settings")]
		[SerializeField] private GameObject projectilePrefab;
		[SerializeField] private Transform projectileSpawnPoint;
		[SerializeField] private float projectileSpeed = 20f;
		[SerializeField] private float fireRate = 0.5f;

		private Vector3 moveDirection = Vector3.zero;
		private float rotationX = 0f;
		private bool canMove = true;
		private CharacterController characterController;
		private float nextFireTime = 0f;
		private List<GameObject> activeProjectiles = new List<GameObject>(); // Tracks active projectiles

		void Start()
		{
			characterController = GetComponent<CharacterController>();
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;

			if (projectileSpawnPoint == null && playerCamera != null)
			{
				projectileSpawnPoint = playerCamera.transform;
			}
		}

		void Update()
		{
			// Handle Movement
			Vector3 forward = transform.TransformDirection(Vector3.forward);
			Vector3 right = transform.TransformDirection(Vector3.right);
			bool isRunning = Input.GetKey(KeyCode.LeftShift);
			float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0f;
			float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0f;
			float movementDirectionY = moveDirection.y;
			moveDirection = (forward * curSpeedX) + (right * curSpeedY);

			// Handle Jumping
			if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
			{
				moveDirection.y = jumpPower;
			}
			else
			{
				moveDirection.y = movementDirectionY;
			}

			if (!characterController.isGrounded)
			{
				moveDirection.y -= gravity * Time.deltaTime;
			}

			// Handle Rotation
			characterController.Move(moveDirection * Time.deltaTime);

			if (canMove)
			{
				rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
				rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
				playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
				transform.rotation *= Quaternion.Euler(0f, Input.GetAxis("Mouse X") * lookSpeed, 0f);
			}

			// Handle Shooting
			if (Input.GetMouseButton(0) && Time.time >= nextFireTime && projectilePrefab != null)
			{
				Shoot();
				nextFireTime = Time.time + fireRate;
			}

			// Clear all projectiles on right mouse button
			if (Input.GetMouseButtonDown(1))
			{
				ClearProjectiles();
			}
		}

		private void Shoot()
		{
			GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
			activeProjectiles.Add(projectile); // Add to tracking list
			Rigidbody rb = projectile.GetComponent<Rigidbody>();
			if (rb != null)
			{
				rb.linearVelocity = projectileSpawnPoint.forward * projectileSpeed;
			}
		}

		private void ClearProjectiles()
		{
			foreach (GameObject projectile in activeProjectiles)
			{
				if (projectile != null)
				{
					Destroy(projectile);
				}
			}
			activeProjectiles.Clear(); // Clear the list
		}
	}
}