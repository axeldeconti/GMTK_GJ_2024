using UnityEngine;
using UnityEngine.SceneManagement;

namespace Avenyrh
{
	public class MainMenuManager : MonoBehaviour
	{
		public void LoadGame()
		{
			SceneManager.LoadScene("Game");
		}
	}
}
