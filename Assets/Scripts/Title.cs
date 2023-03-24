using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Title : MonoBehaviour
{
    [SerializeField]
    private Button _nextBtn;
    [SerializeField]
    private GameObject _hpViewObj;
    [SerializeField]
    private GameObject _ownGoldObj;
    [SerializeField]
    private RectTransform _unmask;

    public Action BtnEvent { get; set; }
    private bool isEnd = false;

    private void Awake()
    {
        _nextBtn.onClick.AddListener(() => OnNextButton());
    }

    private void OnNextButton()
    {
        isEnd = true;
        BtnEvent.Invoke();
    }

    public async UniTask TitleRoutine()
    {
        _hpViewObj.SetActive(false);
        _ownGoldObj.SetActive(false);

        await UniTask.WaitUntil(() => isEnd);

        await FadeOutAnim();

        _hpViewObj.SetActive(true);
        _ownGoldObj.SetActive(true);
    }

    private async UniTask FadeOutAnim()
    {
        bool isfinish = false;
        var seq = DOTween.Sequence();
        seq.Append(_unmask.DOScale(Vector3.zero, 1f));
        seq.OnComplete(() => isfinish = true);

        seq.Play();

        await UniTask.WaitUntil(() => isfinish);
    }

    public void DisableActive()
    {
        gameObject.SetActive(false);
    }
}
