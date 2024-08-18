using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Avenyrh
{
    public class MusicManager : Singleton<MusicManager>, IInitiable
    {
        [SerializeField] private AudioSource _musicSource = null;
        [SerializeField] private AudioClip[] _clips = null;

        [SerializeField] private float _timeToFade = 2;
        [SerializeField, Range(0, 1.0f)] private float _chanceToChangeMusic = 0.2f;

        private float _time = 0;
        private bool _shouldChange = false;
        private bool _isEnding = false;

        private void Start()
        {
            SetNewClip();
        }

        private void Update()
        {
            _time += Time.deltaTime;

            if(_time >= _musicSource.clip.length - _timeToFade && !_isEnding)
            {
                _isEnding = true;
                float random = Random.Range(0.0f, 1.0f);
                _shouldChange = random >= _chanceToChangeMusic;

                if (_shouldChange)
                {
                    StartCoroutine(FadeOut());
                }
            }

            if (_time >= _musicSource.clip.length)
            {
                if(_shouldChange)
                {
                    SetNewClip();
                }
                else
                {
                    _time = 0f;
                    _musicSource.Play();
                }

                _isEnding = false;
                _shouldChange = false;
            }
        }

        private void SetNewClip()
        {
            AudioClip clip = GetRandomClip();
            _time = 0f;
            _musicSource.clip = clip;
            _musicSource.Play();
            StartCoroutine(FadeIn());
        }

        private IEnumerator FadeIn()
        {
            float time = 0;
            while(time < _timeToFade)
            {
                time += Time.deltaTime;
                _musicSource.volume = time / _timeToFade;
                yield return null;
            }
        }

        private IEnumerator FadeOut()
        {
            float time = _timeToFade;
            while (time > 0)
            {
                time -= Time.deltaTime;
                _musicSource.volume = time / _timeToFade;
                yield return null;
            }
        }

        private AudioClip GetRandomClip()
        {
            List<AudioClip> clipList = new List<AudioClip>();
            foreach(AudioClip clip in _clips)
                clipList.Add(clip);

            if(_musicSource.clip != null && clipList.Contains(_musicSource.clip))
                clipList.Remove(_musicSource.clip);

            return clipList.GetRandom();
        }
    }
}
