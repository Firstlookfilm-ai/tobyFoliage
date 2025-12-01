using UnityEngine;

namespace TobyFredson.SceneScripts
{
	[AddComponentMenu("Toby Fredson/The Toby Foliage Engine/Scene/Menu Manager")]
	public class MenuManager : MonoBehaviour
	{
		[SerializeField] private GameObject optionsMenu; // Menu GameObject to toggle
		[SerializeField] private GameObject forest;      // Forest GameObject to toggle
		[SerializeField] private GameObject hills;       // Hills GameObject to toggle

		void Update()
		{
			// Toggle options menu with H key
			if (Input.GetKeyDown(KeyCode.H))
			{
				optionsMenu.SetActive(!optionsMenu.activeSelf);
			}

			// Toggle forest with 1 key
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				forest.SetActive(!forest.activeSelf);
			}

			// Toggle hills with 2 key
			if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				hills.SetActive(!hills.activeSelf);
			}
		}
	}
}