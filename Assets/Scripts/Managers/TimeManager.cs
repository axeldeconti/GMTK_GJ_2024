using TMPro;
using UnityEngine;

namespace Avenyrh
{
	public class TimeManager : MonoBehaviour
	{
        [Header("References")]
        [SerializeField] private AudioSource _audioSource = null;
        [SerializeField] private TextMeshProUGUI _timeText = null;
        [SerializeField] private GameObject _endGameUI = null;

        [Header("Times")]
        [SerializeField] private int _timeToStart = 3;
        [SerializeField] private float _gameTime = 4 * 60;

        [Header("Sounds")]
        [SerializeField] private AudioClip _beepClip = null;
        [SerializeField] private AudioClip _finalClip = null;

        private float _currentTime = -1;
        private int _nbOfBeeps = 5;

        private void Awake()
        {
            EventManager.Subscribe(Ev.OnEndCountdown, OnEndCountdown);
        }

        private void Start()
        {
            _endGameUI.SetActive(false);

            int min = Mathf.FloorToInt(_gameTime / 60);
            int sec = Mathf.FloorToInt(_gameTime % 60);
            _timeText.text = $"{min} : {sec}";

            Invoke("StartCountdown", 1);
        }

        private void Update()
        {
            if (_currentTime <= 0)
                return;

            _currentTime -= Time.deltaTime;
            _currentTime = Mathf.Max(0, _currentTime);
            WriteCurrentTime();

            if(_currentTime <= 0)
            {
                _audioSource.clip = _finalClip;
                _audioSource.Play();
                _endGameUI.SetActive(true);
                EventManager.Trigger(Ev.OnEndGame);
            }
            else
            {
                CheckBeeps();
            }
        }

        private void WriteCurrentTime()
        {
            int min = Mathf.FloorToInt(_currentTime / 60);
            int sec = Mathf.FloorToInt(_currentTime % 60);
            _timeText.text = $"{min} : {sec}";
        }

        private void CheckBeeps()
        {
            if(_currentTime < _nbOfBeeps)
            {
                _nbOfBeeps--;
                _audioSource.clip = _beepClip;
                _audioSource.Play();
            }
        }

        private void StartCountdown()
        {
            EventManager.Trigger(Ev.OnStartCountdown, _timeToStart);
        }

        private void OnEndCountdown(object[] args)
        {
            _currentTime = _gameTime;
            _nbOfBeeps = 5;
            EventManager.Trigger(Ev.OnStartGame);
        }
    } 
}