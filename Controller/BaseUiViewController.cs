using System.Collections;
using DG.Tweening;
using UiViewController.Data;
using UnityEngine;

namespace UiViewController
{
    public class BaseUiViewController<TViewRefs> : MonoBehaviour where TViewRefs : BaseUiViewRefs
    {
        #region protected variables
        [SerializeField] protected TViewRefs _ViewRefs;
        #endregion

        #region private variables
        private Coroutine _closeViewCoroutine;
        private Coroutine _openViewCoroutine;
        #endregion

        #region public methods
        public virtual void Show()
        {
            StartOpenAnimation();
        }

        public virtual void ShowSlowly(object viewData)
        {
            StartOpenAnimation(0.5f);
        }

        
        public virtual void Show(object viewData)
        {
            StartOpenAnimation();
        }
        
        protected virtual void HideSlowly()
        {
            StartCloseAnimationCoroutine(0.5f);
        }
        #endregion

        #region protected methods
        protected virtual void OnShowComplete()
        {
            EnableButtons();
        }

        protected virtual void Hide()
        {
            if (!gameObject.activeSelf)
                return;
            
            DisableButtons();
            StartCloseAnimationCoroutine();
        }

        protected virtual void OnHideComplete()
        {
            var rect = _ViewRefs.Rect;
            gameObject.SetActive(false);
        }

        protected virtual void EnableButtons()
        {
            if (_ViewRefs.CloseButton == null) 
                return;
            
            _ViewRefs.CloseButton.onClick.AddListener(Hide);
        }
        
        protected virtual void DisableButtons()
        {
            if (_ViewRefs.CloseButton == null) 
                return;
            
            _ViewRefs.CloseButton.onClick.RemoveListener(Hide);
        }
        #endregion

        #region private methods
        private void StartCloseAnimationCoroutine(float startDelay = 0f)
        {
            if (_closeViewCoroutine != null)
            {
                StopCoroutine(_closeViewCoroutine);
                _closeViewCoroutine = null;
            }

            _closeViewCoroutine = StartCoroutine(CloseAnimation(startDelay));
        }
        
        private IEnumerator CloseAnimation(float startDelay = 0f)
        {
            yield return new WaitForSeconds(startDelay);
            var rect = _ViewRefs.Rect;
            if (rect.localScale == Vector3.zero)
                yield break;
            
            rect.localScale = Vector3.one;

            var sequence = rect.DOScale(Vector3.zero, 0.5f)
                .SetEase(Ease.InBack)
                .OnComplete(OnHideComplete);
        }

        private void StartOpenAnimation(float startDelay = 0f)
        {
            if (_openViewCoroutine != null)
            {
                StopCoroutine(_openViewCoroutine);
                _openViewCoroutine = null;
            }

            ResetView();
            _openViewCoroutine = StartCoroutine(OpenAnimation(startDelay));
        }

        private IEnumerator OpenAnimation(float startDelay)
        {
            yield return new WaitForSeconds(startDelay);
            
            var rect = _ViewRefs.Rect;
            var sequence = rect.DOScale(Vector3.one, 0.5f)
                .SetEase(Ease.OutBack)
                .OnComplete(OnShowComplete);
        }

        private void ResetView()
        {
            var rect = _ViewRefs.Rect;
            rect.localScale = Vector3.zero;
            gameObject.SetActive(true);
            rect.gameObject.SetActive(true);
        }
        #endregion
    }
}