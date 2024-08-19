using UnityEngine;
using UnityEngine.SceneManagement;

namespace Avenyrh
{
	public class MainMenuManager : MonoBehaviour
	{
		[SerializeField] private PlayerSelection _player1 = null;
		[SerializeField] private PlayerSelection _player2 = null;

		public void LoadGame()
		{
			GameData.player1Controls = _player1.Controls;
			GameData.player2Controls = _player2.Controls;

			SceneManager.LoadScene("Game");
		}
	}
}
