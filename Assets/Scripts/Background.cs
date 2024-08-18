using UnityEngine;

namespace Avenyrh
{
	public class Background : MonoBehaviour
	{
        [SerializeField] private SpriteRenderer _sprite = null;
		[SerializeField] private Gradient _gradient = null;
		[SerializeField] private float _loopTime = 1f;

		private Material _mat = null;
        private float _currentTime = 0f;

        private void Start()
        {
            _mat = _sprite.material;
            _sprite.enabled = true;
        }

        private void Update()
        {
            _currentTime += Time.deltaTime;

            float value = Mathf.Clamp01(_currentTime / _loopTime);
            Color c = _gradient.Evaluate(value);

            _mat.SetColor("_Color", c);

            if (_currentTime > _loopTime)
                _currentTime = 0f;
        }
    }
}
