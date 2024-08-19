using UnityEngine;
using UnityEngine.SceneManagement;

namespace Avenyrh
{
	public class GameManager : Singleton<GameManager>, IInitiable
	{
		[SerializeField] private GameObject _onePlayerModeParent = null;
		[SerializeField] private GameObject _twoPlayerModeParent = null;

		[Header("Boards")]
		[SerializeField] private Board _onePlayerBoard = null;
		[SerializeField] private Board _twoPlayerBoard1 = null;
		[SerializeField] private Board _twoPlayerBoard2 = null;

        private void Start()
        {
			if (GameData.IsOnePlayer)
			{
				_onePlayerBoard.SetControls(GameData.GetControls(GameData.player1Controls));
			}
			else
			{
				_twoPlayerBoard1.SetControls(GameData.GetControls(GameData.player1Controls));
				_twoPlayerBoard2.SetControls(GameData.GetControls(GameData.player2Controls));
			}
        }

        public void GoToMainMenu()
		{
			SceneManager.LoadScene("Menu");
		}
    }

	public static class GameData
	{
		public static EControl player1Controls = EControl.WASD;
		public static EControl player2Controls = EControl.NONE;

		public static Controls GetControls(EControl c)
		{
			switch (c)
			{
				case EControl.NONE:
					return null;
				case EControl.WASD:
					return new Controls_WASD();
				case EControl.IJKL:
                    return new Controls_IJKL();
				case EControl.CONTROLLER_1:
					return new Controls_Controller1();
				case EControl.CONTROLLER_2:
					return new Controls_Controller2();
				default:
					return null;
			}
		}

		public static bool IsOnePlayer => player2Controls == EControl.NONE;
	}
}
