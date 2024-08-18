using UnityEngine;

namespace Avenyrh
{
	public class AudioManager : MonoBehaviour
	{
		[Header("Sources")]
		[SerializeField] private AudioSource _musicSource = null;
		[SerializeField] private AudioSource _sfxSource = null;
		[SerializeField] private AudioSource _stepSource = null;

		[Header("Clips")]
		[SerializeField] private AudioClip _stepClip = null;
		[SerializeField] private AudioClip _hardDropClip = null;
		[SerializeField] private AudioClip _storeClip = null;
		[SerializeField] private AudioClip _moveClip = null;
		[SerializeField] private AudioClip _rotateClip = null;
		[SerializeField] private AudioClip _destroyClip = null;

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                PlayDestroy();
            }
        }

        public void PlayMusic(AudioClip clip)
		{
			_musicSource.clip = clip;
			_musicSource.Play();
		}

		public void PlaySfx(AudioClip clip)
		{
			_sfxSource.pitch = 1;
			_sfxSource.clip = clip;
			_sfxSource.Play();
		}

		public void PlaySfxRandomPitch(AudioClip clip)
		{
            _sfxSource.pitch = Random.Range(0.8f, 1.2f);
            _sfxSource.clip = clip;
            _sfxSource.Play();
        }

        #region Specifics
		public void PlayStep()
		{
            _stepSource.pitch = Random.Range(0.8f, 1.2f);
            _stepSource.clip = _stepClip;
            _stepSource.Play();
        }

        public void PlayStore()
        {
            PlaySfxRandomPitch(_storeClip);
        }

        public void PlayMove()
        {
            PlaySfxRandomPitch(_moveClip);
        }

        public void PlayRotate()
        {
            PlaySfxRandomPitch(_rotateClip);
        }

        public void PlayDestroy()
        {
            PlaySfxRandomPitch(_destroyClip);
        }
        #endregion
    }
}
