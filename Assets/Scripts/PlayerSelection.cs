using MoreMountains.Feedbacks;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Avenyrh
{
    public class PlayerSelection : MonoBehaviour
    {
        [SerializeField] private EControl _currentControls = EControl.WASD;
        [SerializeField] private PlayerSelection _otherPlayer = null;
        [SerializeField] private CanvasGroup _group = null;

        [Header("Control details")]
        [SerializeField] private GameObject _controlDetailsParent = null;
        [SerializeField] private GameObject _wasdDetails = null;
        [SerializeField] private GameObject _ijklDetails = null;
        [SerializeField] private GameObject _controllerDetails = null;

        [Header("Buttons")]
        [SerializeField] private Color _greyButtonColor = Color.white;
        [SerializeField] private TextMeshProUGUI _wasdButton = null;
        [SerializeField] private TextMeshProUGUI _ijklButton = null;
        [SerializeField] private TextMeshProUGUI _controller1Button = null;
        [SerializeField] private TextMeshProUGUI _controller2Button = null;

        [Header("Feedbacks")]
        [SerializeField] private MMF_Player _logoFeedback = null;
        [SerializeField] private MMF_Player _noneFeedback = null;
        [SerializeField] private MMF_Player _wasdFeedback = null;
        [SerializeField] private MMF_Player _ijklFeedback = null;
        [SerializeField] private MMF_Player _controller1Feedback = null;
        [SerializeField] private MMF_Player _controller2Feedback = null;

        private void Start()
        {
            StartCoroutine(ControllerCheck());
        }

        private IEnumerator ControllerCheck()
        {
            while (true)
            {
                string[] controllers = Input.GetJoystickNames();
                int nbOfControllersReal = 0;
                foreach (string controller in controllers)
                {
                    if(!string.IsNullOrEmpty(controller))
                        nbOfControllersReal++;
                }

                if(nbOfControllersReal == 0)
                {
                    //No controller connected
                    _controller1Button.gameObject.SetActive(false);
                    _controller2Button.gameObject.SetActive(false);

                    if (_currentControls == EControl.CONTROLLER_1 || _currentControls == EControl.CONTROLLER_2)
                        SelectFirstControlsAvailable();
                }
                else if(nbOfControllersReal == 1)
                {
                    //Only one controller connected
                    _controller1Button.gameObject.SetActive(true);
                    _controller2Button.gameObject.SetActive(false);

                    if (_currentControls == EControl.CONTROLLER_2)
                        SelectFirstControlsAvailable();
                }
                else
                {
                    //At least 2 controllers connected
                    _controller1Button.gameObject.SetActive(true);
                    _controller2Button.gameObject.SetActive(true);
                }

                yield return new WaitForSeconds(1f);
            }
        }

        private void SelectFirstControlsAvailable()
        {
            if (_otherPlayer.Controls != EControl.WASD)
                SelectWasd();
            else if(_otherPlayer.Controls != EControl.IJKL)
                SelectIjkl();
        }

        public void SelectNone()
        {
            _currentControls = EControl.NONE;
            _group.alpha = 0.2f;
            _noneFeedback.PlayFeedbacks();
            _logoFeedback.StopFeedbacks();
            _controlDetailsParent.SetActive(false);

            SetGreyButton(_otherPlayer.Controls);
            _otherPlayer.SetGreyButton(_currentControls);
        }

        public void SelectWasd()
        {
            if (_otherPlayer.Controls != EControl.WASD)
            {
                _currentControls = EControl.WASD;
                _group.alpha = 1f;
                _wasdFeedback.PlayFeedbacks();

                SetGreyButton(_otherPlayer.Controls);
                _otherPlayer.SetGreyButton(_currentControls);

                _controlDetailsParent.SetActive(true);
                _wasdDetails.SetActive(true);
                _ijklDetails.SetActive(false);
                _controllerDetails.SetActive(false);

                if (!_logoFeedback.IsPlaying)
                    _logoFeedback.PlayFeedbacks();
            }
        }

        public void SelectIjkl()
        {
            if (_otherPlayer.Controls != EControl.IJKL)
            {
                _currentControls = EControl.IJKL;
                _group.alpha = 1f;
                _ijklFeedback.PlayFeedbacks();

                SetGreyButton(_otherPlayer.Controls);
                _otherPlayer.SetGreyButton(_currentControls);

                _controlDetailsParent.SetActive(true);
                _wasdDetails.SetActive(false);
                _ijklDetails.SetActive(true);
                _controllerDetails.SetActive(false);

                if (!_logoFeedback.IsPlaying)
                    _logoFeedback.PlayFeedbacks();
            }
        }

        public void SelectController1()
        {
            if (_otherPlayer.Controls != EControl.CONTROLLER_1)
            {
                _currentControls = EControl.CONTROLLER_1;
                _group.alpha = 1f;
                _controller1Feedback.PlayFeedbacks();

                SetGreyButton(_otherPlayer.Controls);
                _otherPlayer.SetGreyButton(_currentControls);

                _controlDetailsParent.SetActive(true);
                _wasdDetails.SetActive(false);
                _ijklDetails.SetActive(false);
                _controllerDetails.SetActive(true);

                if (!_logoFeedback.IsPlaying)
                    _logoFeedback.PlayFeedbacks();
            }
        }

        public void SelectController2()
        {
            if (_otherPlayer.Controls != EControl.CONTROLLER_2)
            {
                _currentControls = EControl.CONTROLLER_2;
                _group.alpha = 1f;
                _controller2Feedback.PlayFeedbacks();

                SetGreyButton(_otherPlayer.Controls);
                _otherPlayer.SetGreyButton(_currentControls);

                _controlDetailsParent.SetActive(true);
                _wasdDetails.SetActive(false);
                _ijklDetails.SetActive(false);
                _controllerDetails.SetActive(true);

                if (!_logoFeedback.IsPlaying)
                    _logoFeedback.PlayFeedbacks();
            }
        }

        public void SetGreyButton(EControl other)
        {
            _wasdButton.color = other == EControl.WASD ? _greyButtonColor : Color.white;
            _ijklButton.color = other == EControl.IJKL ? _greyButtonColor : Color.white;
            _controller1Button.color = other == EControl.CONTROLLER_1 ? _greyButtonColor : Color.white;
            _controller2Button.color = other == EControl.CONTROLLER_2 ? _greyButtonColor : Color.white;
        }

        public EControl Controls => _currentControls;
    }
}
