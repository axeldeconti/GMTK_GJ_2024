using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

namespace Avenyrh
{
	public class MainMenuManager : MonoBehaviour
	{
		[SerializeField] private PlayerSelection _player1 = null;
		[SerializeField] private PlayerSelection _player2 = null;

		[Header("Buttons")]
		[SerializeField] private GameObject _playButton = null;
		[SerializeField] private GameObject _playReturnButton = null;
		[SerializeField] private GameObject _howToPlayReturnButton = null;

		public void LoadGame()
		{
			GameData.player1Controls = _player1.Controls;
			GameData.player2Controls = _player2.Controls;

			SceneManager.LoadScene("Game");
		}

		public void QuitGame()
		{
			Application.Quit();
		}

		public void SetMainPage()
		{
			SetNewSelected(_playButton);
		}

        public void SetPlayPage()
        {
            SetNewSelected(_playReturnButton);
        }

        public void SetHowToPlayPage()
        {
            SetNewSelected(_howToPlayReturnButton);
        }

        private void SetNewSelected(GameObject go)
		{
            //Clear selected
            EventSystem.current.SetSelectedGameObject(null);

            //Set new selected
			EventSystem.current.SetSelectedGameObject(go);
        }
    }
}
