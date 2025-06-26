using System;

using DG.Tweening;

using UnityEngine;

namespace Cyberspeed.CardMatch.Cards
{
    /// <summary>
    /// Controls all animations for a card, such as flipping, shaking, and scaling.
    /// It uses DOTween for smooth and efficient animations.
    /// </summary>
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

        /// <summary>
        /// Resets all animations and returns the card to its initial visual state.
        /// </summary>
        public void ResetAnimations()
        {
            KillCurrentTween();

            _rectTransform.localScale = _initialScale;
            _rectTransform.localRotation = _initialRotation;
            _rectTransform.localPosition = _initialPosition;
        }

        /// <summary>
        /// Plays a punch scale animation on the card, typically used for hover effects.
        /// </summary>
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

        /// <summary>
        /// Plays the reveal animation, flipping the card from back to front.
        /// </summary>
        /// <param name="onComplete">An optional action to invoke when the animation is complete.</param>
        public void PlayReveal(Action onComplete = null) => PlayFlip(_backVisual, _frontVisual, _flipSettings, onComplete);

        /// <summary>
        /// Plays the hide animation, flipping the card from front to back.
        /// </summary>
        /// <param name="onComplete">An optional action to invoke when the animation is complete.</param>
        public void PlayHide(Action onComplete = null) => PlayFlip(_frontVisual, _backVisual, _flipSettings, onComplete);

        /// <summary>
        /// Plays the flip animation, flipping the card from one side to the other.
        /// </summary>
        /// <param name="from">The transform of the card's front side.</param>
        /// <param name="to">The transform of the card's back side.</param>
        /// <param name="settings">The settings for the flip animation.</param>
        /// <param name="onComplete">An optional action to invoke when the animation is complete.</param>
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

        /// <summary>
        /// Plays a shake animation, typically used to indicate a failed match.
        /// </summary>
        /// <param name="onComplete">An optional action to invoke when the animation is complete.</param>
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

        /// <summary>
        /// Plays a destroy animation for when a card is successfully matched and removed from the board.
        /// </summary>
        /// <param name="onComplete">An optional action to invoke when the animation is complete.</param>
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

        /// <summary>
        /// Caches the initial state of the card, including its scale, rotation, and position.
        /// </summary>
        private void CacheInitialState()
        {
            _initialScale = _rectTransform.localScale;
            _initialRotation = _rectTransform.localRotation;
            _initialPosition = _rectTransform.localPosition;
        }

        /// <summary>
        /// Kills the current tween and sequence, and resets the card's scale to its initial value.
        /// </summary>
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

        /// <summary>
        /// Settings for the punch scale animation.
        /// </summary>
        [System.Serializable]
        private class PunchScaleSettings
        {
            public float PunchScale = 0.2f;
            public float Duration = 0.3f;
            public int Vibrato = 10;
            public float Elasticity = 1f;
        }

        /// <summary>
        /// Settings for the flip animation.
        /// </summary>
        [System.Serializable]
        private class FlipSettings
        {
            public float FlipDuration = 0.2f;
            public Ease EaseIn = Ease.InQuad;
            public Ease EaseOut = Ease.OutQuad;
        }

        /// <summary>
        /// Settings for the shake animation.
        /// </summary>
        [System.Serializable]
        private class ShakeSettings
        {
            public float Duration = 0.3f;
            public float Strength = 10f;
            public int Vibrato = 10;
            public float Randomness = 90f;
            public bool FadeOut = true;
        }

        /// <summary>
        /// Settings for the destroy animation.
        /// </summary>
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