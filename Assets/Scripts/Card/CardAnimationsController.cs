using System;

using DG.Tweening;

using UnityEngine;

namespace Cyberspeed.CardMatch.Cards
{
    public class CardAnimationsController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Transform _frontVisual;
        [SerializeField] private Transform _backVisual;
        [Header("Animations Settings")]
        [SerializeField] private PunchScaleSettings _punchScaleSettings;
        [SerializeField] private FlipSettings _flipSettings;
        [SerializeField] private ShakeSettings _shakeSettings;
        [SerializeField] private DestroySettings _destroySettings;

        private Tween _currentTween;
        private Sequence _currentSequence;
        private Vector3 _initialScale;
        private Quaternion _initialRotation;
        private Vector3 _initialPosition;
        private bool _isPlayingAnimation = false;

        #region MonoBehaviour

        private void Awake()
        {
            CacheInitialState();
        }

        #endregion

        #region Public Methods

        public void ResetAnimations()
        {
            KillCurrentTween();

            _rectTransform.localScale = _initialScale;
            _rectTransform.localRotation = _initialRotation;
            _rectTransform.localPosition = _initialPosition;
        }

        public void PlayPunchScale()
        {
            if (_isPlayingAnimation || (_currentTween != null && _currentTween.IsActive())) return;

            _currentTween = _rectTransform.DOPunchScale(
                Vector3.one * _punchScaleSettings.PunchScale,
                _punchScaleSettings.Duration,
                _punchScaleSettings.Vibrato,
                _punchScaleSettings.Elasticity
            ).SetUpdate(true);
        }

        public void PlayReveal(Action onComplete = null) => PlayFlip(_backVisual, _frontVisual, _flipSettings, onComplete);

        public void PlayHide(Action onComplete = null) => PlayFlip(_frontVisual, _backVisual, _flipSettings, onComplete);

        private void PlayFlip(Transform from, Transform to, FlipSettings settings, Action onComplete = null)
        {
            KillCurrentTween();
            _isPlayingAnimation = true;

            from.gameObject.SetActive(true);
            to.gameObject.SetActive(false);

            from.localRotation = Quaternion.identity;
            to.localRotation = Quaternion.Euler(0, 90, 0);

            _currentSequence = DOTween.Sequence();

            _currentSequence.Append(from.DOLocalRotate(new Vector3(0, -90, 0), settings.FlipDuration / 2f)
                .SetEase(settings.EaseIn));

            _currentSequence.AppendCallback(() =>
            {
                from.gameObject.SetActive(false);
                to.gameObject.SetActive(true);
            });

            _currentSequence.Append(to.DOLocalRotate(Vector3.zero, settings.FlipDuration / 2f)
                .SetEase(settings.EaseOut));

            _currentSequence.OnComplete(() =>
            {
                _isPlayingAnimation = false;
                onComplete?.Invoke();
            });
        }

        public void PlayShake(Action onComplete = null)
        {
            KillCurrentTween();
            _isPlayingAnimation = true;

            _currentTween = _rectTransform.DOShakeAnchorPos(
                    _shakeSettings.Duration,
                    _shakeSettings.Strength,
                    _shakeSettings.Vibrato,
                    _shakeSettings.Randomness,
                    false,
                    _shakeSettings.FadeOut
                )
                .SetEase(Ease.Linear)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    onComplete?.Invoke();
                    _isPlayingAnimation = false;
                });
        }

        public void PlayDestroy(Action onComplete = null)
        {
            KillCurrentTween();
            _isPlayingAnimation = true;

            _currentSequence = DOTween.Sequence();

            _currentSequence.Append(_rectTransform.DOPunchScale(
                Vector3.one * _destroySettings.PunchScale,
                _destroySettings.PunchDuration,
                _destroySettings.Vibrato,
                _destroySettings.Elasticity
            ));

            _currentSequence.Append(_rectTransform.DOScale(Vector3.zero, _destroySettings.ShrinkDuration)
                .SetEase(_destroySettings.ShrinkEase));

            _currentSequence.OnComplete(() =>
            {
                _isPlayingAnimation = false;
                onComplete?.Invoke();
            });
        }

        #endregion

        #region Utils

        private void CacheInitialState()
        {
            _initialScale = _rectTransform.localScale;
            _initialRotation = _rectTransform.localRotation;
            _initialPosition = _rectTransform.localPosition;
        }

        private void KillCurrentTween()
        {
            _isPlayingAnimation = false;

            if (_currentTween != null && _currentTween.IsActive())
            {
                _currentTween.Kill();
                _currentTween = null;
            }

            if (_currentSequence != null && _currentSequence.IsActive())
            {
                _currentSequence.Kill();
                _currentSequence = null;
            }

            _rectTransform.localScale = _initialScale;
        }

        #endregion

        #region Animation Settings

        [System.Serializable]
        private class PunchScaleSettings
        {
            public float PunchScale = 0.2f;
            public float Duration = 0.3f;
            public int Vibrato = 10;
            public float Elasticity = 1f;
        }

        [System.Serializable]
        private class FlipSettings
        {
            public float FlipDuration = 0.2f;
            public Ease EaseIn = Ease.InQuad;
            public Ease EaseOut = Ease.OutQuad;
        }

        [System.Serializable]
        private class ShakeSettings
        {
            public float Duration = 0.3f;
            public float Strength = 10f;
            public int Vibrato = 10;
            public float Randomness = 90f;
            public bool FadeOut = true;
        }

        [System.Serializable]
        private class DestroySettings
        {
            public float PunchScale = 0.3f;
            public float PunchDuration = 0.25f;
            public int Vibrato = 10;
            public float Elasticity = 1f;
            public float ShrinkDuration = 0.3f;
            public Ease ShrinkEase = Ease.InBack;
        }

        #endregion
    }
}