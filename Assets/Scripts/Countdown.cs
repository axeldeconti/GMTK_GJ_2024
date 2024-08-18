using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;

namespace Avenyrh
{
	public class Countdown : MonoBehaviour
	{
        [Header("References")]
        [SerializeField] private GameObject _parent = null;
        [SerializeField] private GameObject _goParent = null;
        [SerializeField] private GameObject _countdownParent = null;
        [SerializeField] private TextMeshProUGUI _text = null;

        [Header("Feedbacks")]
        [SerializeField] private MMF_Player _countdownFeedback = null;
        [SerializeField] private MMF_Player _goFeedback = null;

        private int _currentTime = -1;

        private void Awake()
        {
            EventManager.Subscribe(Ev.OnStartCountdown, OnStartCountdown);

            _parent.SetActive(true);
            _countdownParent.SetActive(false);
            _goParent.SetActive(false);
        }

        private void OnStartCountdown(object[] args)
        {
            _currentTime = (int)args[0];

            _parent.SetActive(true);
            _countdownParent.SetActive(true);
            _goParent.SetActive(false);

            ShowCountdown();

            Invoke("CountDown", 1);
        }

        private void CountDown()
        {
            _currentTime--;

            if(_currentTime <= 0)
            {
                _countdownParent.SetActive(false);
                _goParent.SetActive(true);
                _goFeedback.PlayFeedbacks();

                Invoke("EndGo", 1);
            }
            else
            {
                ShowCountdown();
                Invoke("CountDown", 1);
            }
        }

        private void ShowCountdown()
        {
            _text.text = _currentTime.ToString();
            _countdownFeedback.PlayFeedbacks();
        }

        private void EndGo()
        {
            _parent.SetActive(false);
            EventManager.Trigger(Ev.OnEndCountdown);
        }
    }
}
