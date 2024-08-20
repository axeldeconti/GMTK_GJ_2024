using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Avenyrh
{
    public class GameManager : Singleton<GameManager>, IInitiable
    {
        [SerializeField] private TimeManager _timeManager = null;
        [SerializeField] private GameObject _onePlayerModeParent = null;
        [SerializeField] private GameObject _twoPlayerModeParent = null;
        [SerializeField] private GameObject _endGameButton = null;

        [Header("One player mode")]
        [SerializeField] private Board _onePlayerBoard = null;
        [SerializeField] private GameObject _onePlayerTime = null;

        [Header("Two player mode")]
        [SerializeField] private Board _twoPlayerBoard1 = null;
        [SerializeField] private Board _twoPlayerBoard2 = null;
        [SerializeField] private GameObject _twoPlayerTime = null;
        [SerializeField] private GameObject _winParent = null;
        [SerializeField] private MMF_Player _player1WinFeedback = null;
        [SerializeField] private MMF_Player _player2WinFeedback = null;

        [Header("Pause")]
        [SerializeField] private GameObject _pauseMenu = null;
        [SerializeField] private GameObject _resumeButton = null;
        [SerializeField] private MMF_Player _resumeFeedback = null;
        [SerializeField] private MMF_Player _quitFeedback = null;

        private bool _canPauseGame = false;

        private void Start()
        {
            EventManager.Subscribe(Ev.OnEndCountdown, OnEndCountdown);
            EventManager.Subscribe(Ev.OnEndGame, OnEndGame);

            _canPauseGame = false;
            _pauseMenu.SetActive(false);
            _winParent.SetActive(false);

            if (GameData.IsOnePlayer)
            {
                _onePlayerModeParent.SetActive(true);
                _twoPlayerModeParent.SetActive(false);
                _onePlayerTime.SetActive(true);
                _twoPlayerTime.SetActive(false);
                _onePlayerBoard.SetControls(GameData.GetControls(GameData.player1Controls));
            }
            else
            {
                _onePlayerModeParent.SetActive(false);
                _twoPlayerModeParent.SetActive(true);
                _onePlayerTime.SetActive(false);
                _twoPlayerTime.SetActive(true);
                _twoPlayerBoard1.SetControls(GameData.GetControls(GameData.player1Controls));
                _twoPlayerBoard2.SetControls(GameData.GetControls(GameData.player2Controls));
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            EventManager.Unsubscribe(Ev.OnEndGame, OnEndGame);
        }

        private void Update()
        {
            if (!_canPauseGame)
                return;

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button7) || Input.GetKeyDown(KeyCode.Joystick2Button7))
            {
                TogglePauseMenu();
            }
        }

        public void TogglePauseMenu()
        {
            bool pause = !_pauseMenu.activeSelf;
            _pauseMenu.SetActive(pause);

            if (pause)
            {
                _timeManager.SetIsCounting(false);
                SetNewSelected(_resumeButton);

                if (GameData.IsOnePlayer)
                {
                    _onePlayerBoard.SetCanPlay(false);
                }
                else
                {
                    _twoPlayerBoard1.SetCanPlay(false);
                    _twoPlayerBoard2.SetCanPlay(false);
                }
            }
            else
            {
                _timeManager.SetIsCounting(true);
                EventSystem.current.SetSelectedGameObject(null);

                _resumeFeedback.RestoreInitialValues();
                _resumeFeedback.StopFeedbacks();
                _quitFeedback.RestoreInitialValues();
                _quitFeedback.StopFeedbacks();

                if (GameData.IsOnePlayer)
                {
                    _onePlayerBoard.SetCanPlay(true);
                }
                else
                {
                    _twoPlayerBoard1.SetCanPlay(true);
                    _twoPlayerBoard2.SetCanPlay(true);
                }
            }
        }

        private void OnEndCountdown(object[] args)
        {
            _canPauseGame = true;
        }

        private void OnEndGame(object[] args)
        {
            _canPauseGame = false;
            SetNewSelected(_endGameButton);
            if (_twoPlayerBoard1.CurrentScore > _twoPlayerBoard2.CurrentScore)
            {
                _winParent.SetActive(true);
                _player1WinFeedback.PlayFeedbacks();
            }
            else if (_twoPlayerBoard1.CurrentScore < _twoPlayerBoard2.CurrentScore)
            {
                _winParent.SetActive(true);
                _player2WinFeedback.PlayFeedbacks();
            }
        }

        public void GoToMainMenu()
        {
            SceneManager.LoadScene("Menu");
        }

        private void SetNewSelected(GameObject go)
        {
            //Clear selected
            EventSystem.current.SetSelectedGameObject(null);

            //Set new selected
            EventSystem.current.SetSelectedGameObject(go);
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
