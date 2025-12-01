using UnityEngine;

namespace TobyFredson.SceneScripts
{
	[AddComponentMenu("Toby Fredson/The Toby Foliage Engine/Scene/Extended Flycam")]
	public class ExtendedFlycam : MonoBehaviour
	{
		/*
            EXTENDED FLYCAM
                Desi Quintans (CowfaceGames.com), 17 August 2012.
                Based on FlyThrough.js by Slin (http://wiki.unity3d.com/index.php/FlyThrough), 17 May 2011.

            LICENSE
                Free as in speech, and free as in beer.

            FEATURES
                WASD/Arrows:    Movement
                Q:              Climb
                E:              Drop
                Shift:          Move faster
                Control:        Move slower
                End:            Toggle cursor locking to screen (you can also press Ctrl+P to toggle play mode on and off).
        */

		public float cameraSensitivity = 3f;
		public float climbSpeed = 4f;
		public float normalMoveSpeed = 10f;
		public float slowMoveFactor = 0.25f;
		public float fastMoveFactor = 3f;

		private float rotationX = 0f;
		private float rotationY = 0f;

		void Start()
		{
			Cursor.lockState = CursorLockMode.Locked;
		}

		void Update()
		{
			// Camera rotation based on mouse input
			rotationX += Input.GetAxis("Mouse X") * cameraSensitivity;
			rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity;
			rotationY = Mathf.Clamp(rotationY, -90f, 90f);

			if (Mathf.Abs(Input.GetAxis("Mouse X")) > 0f || Mathf.Abs(Input.GetAxis("Mouse Y")) > 0f)
			{
				transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up) * Quaternion.AngleAxis(rotationY, Vector3.left);
			}

			// Movement based on input
			float moveSpeed = normalMoveSpeed;
			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
			{
				moveSpeed *= fastMoveFactor;
			}
			else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
			{
				moveSpeed *= slowMoveFactor;
			}

			transform.position += transform.forward * moveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
			transform.position += transform.right * moveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;

			// Vertical movement (climb/drop)
			if (Input.GetKey(KeyCode.Q))
			{
				transform.position += transform.up * climbSpeed * Time.deltaTime;
			}
			if (Input.GetKey(KeyCode.E))
			{
				transform.position -= transform.up * climbSpeed * Time.deltaTime;
			}
		}
	}
}