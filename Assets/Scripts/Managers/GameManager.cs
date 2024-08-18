using UnityEngine;
using UnityEngine.SceneManagement;

namespace Avenyrh
{
	public class GameManager : Singleton<GameManager>, IInitiable
	{
		public void GoToMainMenu()
		{
			SceneManager.LoadScene("Menu");
		}
    }
}
