using UnityEngine;

namespace TobyFredson.SceneScripts
{
	[AddComponentMenu("Toby Fredson/The Toby Foliage Engine/Scene/Show Hide")]
	public class ShowHide : MonoBehaviour
	{
		public void Show(GameObject obj)
		{
			if (obj != null)
			{
				obj.SetActive(true);
			}
		}

		public void Hide(GameObject obj)
		{
			if (obj != null)
			{
				obj.SetActive(false);
			}
		}
	}
}