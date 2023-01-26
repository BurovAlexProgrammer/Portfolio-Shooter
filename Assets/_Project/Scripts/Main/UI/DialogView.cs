﻿using System;
using _Project.Scripts.Extension;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(RectTransform))]
    public class DialogView : MonoBehaviour
    {
        [SerializeField] private Button _buttonOk;
        [SerializeField] private Button _buttonCancel;
        [SerializeField] private Image _background;
        [SerializeField] private CanvasGroup _canvasGroup;

        public Action<bool> Confirm;

        private RectTransform _rectTransform;

        private const float _fadeDuration = 0.3f;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _buttonOk.onClick.AddListener(() => Confirm?.Invoke(true));
            _buttonCancel.onClick.AddListener(() => Confirm?.Invoke(false));
        }

        private void OnDestroy()
        {
            _buttonOk.onClick.RemoveAllListeners();
            _buttonCancel.onClick.RemoveAllListeners();
        }

        public async UniTask Show()
        {
            gameObject.SetActive(true);
            await _canvasGroup
                .DOFade(1f, _fadeDuration)
                .From(0f)
                .SetUpdate(true)
                .SetEase(Ease.InOutQuad)
                .AsyncWaitForCompletion();
        }
        
        public async UniTask Close()
        {
            await _canvasGroup
                .DOFade(0f, _fadeDuration)
                .SetUpdate(true)
                .SetEase(Ease.InOutQuad)
                .AsyncWaitForCompletion();
            Debug.Log("Close");
            gameObject.SetActive(false);
        }

        public void Disable()
        {
            _canvasGroup.interactable = false;
        }
        
        public void Enable()
        {
            _canvasGroup.interactable = true;
        }
    }
}