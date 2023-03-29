using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using DG.Tweening.Core;

public class Powerup : MonoBehaviour
{
    private readonly string MAX_TEXT = "Max";

    [SerializeField]
    private Button _nextBtn;
    [SerializeField]
    private TextMeshProUGUI _lvDamageText;
    [SerializeField]
    private TextMeshProUGUI _lvDamageNeedGoldText;
    [SerializeField]
    private Button _lvupDamageBtn;
    [SerializeField]
    private Button _lvdawnDamageBtn;
    [SerializeField]
    private TextMeshProUGUI _lvSpeedText;
    [SerializeField]
    private TextMeshProUGUI _lvSpeedNeedGoldText;
    [SerializeField]
    private Button _lvupSpeedBtn;
    [SerializeField]
    private Button _lvdawnSpeedBtn;
    [SerializeField]
    private TextMeshProUGUI _lvFovText;
    [SerializeField]
    private TextMeshProUGUI _lvFovNeedGoldText;
    [SerializeField]
    private Button _lvupFovBtn;
    [SerializeField]
    private Button _lvdawnFovBtn;
    [SerializeField]
    private RectTransform _tutorial1;
    [SerializeField]
    private RectTransform _tutorial2;
    
    public TextMeshProUGUI LvDamageText { get { return _lvDamageText; } }
    public TextMeshProUGUI LvDamageNeedGoldText { get { return _lvDamageNeedGoldText; } }
    public TextMeshProUGUI LvSpeedText { get { return _lvSpeedText; } }
    public TextMeshProUGUI LvSpeedNeedGoldText { get { return _lvSpeedNeedGoldText; } }
    public TextMeshProUGUI LvFovText { get { return _lvFovText; } }
    public TextMeshProUGUI LvFovNeedGoldText { get { return _lvFovNeedGoldText; } }

    public bool isNext { get; set; } = false;

    private Sequence _seqTutorial;

    public void OnLvupDamageEvent(Action btn_event)
    {
        _lvupDamageBtn.onClick.AddListener(() => btn_event());
    }
    public void OnLvdawnDamageEvent(Action btn_event)
    {
        _lvdawnDamageBtn.onClick.AddListener(() => btn_event());
    }
    public void OnLvupSpeedEvent(Action btn_event)
    {
        _lvupSpeedBtn.onClick.AddListener(() => btn_event());
    }
    public void OnLvdawnSpeedEvent(Action btn_event)
    {
        _lvdawnSpeedBtn.onClick.AddListener(() => btn_event());
    }
    public void OnLvupFovEvent(Action btn_event)
    {
        _lvupFovBtn.onClick.AddListener(() => btn_event());
    }
    public void OnLvdawnFovEvent(Action btn_event)
    {
        _lvdawnFovBtn.onClick.AddListener(() => btn_event());
    }

    public void Setup(PowerViewModel p)
    {
        BindLvText(p.LvDamage, Data.DataType.Damage);
        BindLvText(p.LvSpeed, Data.DataType.Speed);
        BindLvText(p.LvFov, Data.DataType.Fov);

        BindNeedGold(p.NeedGoldDamage, Data.DataType.Damage);
        BindNeedGold(p.NeedGoldSpeed, Data.DataType.Speed);
        BindNeedGold(p.NeedGoldFov, Data.DataType.Fov);

        OnLvupDamageEvent(p.LvupDamageEvent);
        OnLvdawnDamageEvent(p.LvdawnDamageEvent);
        OnLvupSpeedEvent(p.LvupSpeedEvent);
        OnLvdawnSpeedEvent(p.LvdawnSpeedEvent);
        OnLvupFovEvent(p.LvupFovEvent);
        OnLvdawnFovEvent(p.LvdawnFovEvent);
    }
 
    public void EnablePowerup()
    {
        gameObject.SetActive(true);
        isNext = false;
    }
    public void DisablePowerup()
    {
        gameObject.SetActive(false);
    }

    public void StopAnimTutorial()
    {
        _seqTutorial.Pause();
    }
    public void PlayAnimTutorial()
    {
        _seqTutorial.Restart();
    }

    private void Start()
    {
        _nextBtn.onClick.AddListener(() => OnNextEvent());

        var seq1 = DOTween.Sequence();
        var seq2 = DOTween.Sequence();

        seq1.Append(_tutorial1.DOMoveY(0f, 0.1f));
        seq1.Append(_tutorial1.DOMoveY(_tutorial1.sizeDelta.y, 40f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart));
        seq2.Append(_tutorial2.DOMoveY(0f, 0.1f));
        seq2.Append(_tutorial2.DOMoveY(_tutorial2.sizeDelta.y, 30f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart));

        _seqTutorial = DOTween.Sequence().Append(seq1).Join(seq2);
    }

    private void OnDestroy()
    {
        RemoveEventButtons();
    }

    private void OnNextEvent()
    {
        isNext = true;
        DisablePowerup();
    }

    private void BindLvText(AsyncReactiveProperty<int> lv, Data.DataType type)
    {
        TextMeshProUGUI tex = null;
        switch (type)
        {
            case Data.DataType.Damage:
                tex = _lvDamageText;
                break;
            case Data.DataType.Speed:
                tex = _lvSpeedText;
                break;
            case Data.DataType.Fov:
                tex = _lvFovText;
                break;
        }

        tex.text = lv.Value.ToString();
        lv.BindTo(tex);

        AsyncLvMaxTextView(lv, tex).Forget();
    }

    private async UniTask AsyncLvMaxTextView(AsyncReactiveProperty<int> lv, TextMeshProUGUI tex)
    {
        await lv.WithoutCurrent().ForEachAsync(n =>
        {
            if (n == Data.MAX_LEVEL)
            {
                tex.text = MAX_TEXT;
            }
        });
    }

    private void BindNeedGold(AsyncReactiveProperty<int> gold, Data.DataType type)
    {
        TextMeshProUGUI tex = null;
        switch (type)
        {
            case Data.DataType.Damage:
                tex = LvDamageNeedGoldText;
                break;
            case Data.DataType.Speed:
                tex = LvSpeedNeedGoldText;
                break;
            case Data.DataType.Fov:
                tex = LvFovNeedGoldText;
                break;
        }

        tex.text = gold.Value.ToString();
        gold.BindTo(tex);

        AsyncNeedGoldTextView(gold, tex).Forget();
    }

    private async UniTask AsyncNeedGoldTextView(AsyncReactiveProperty<int> gold, TextMeshProUGUI tex)
    {
        await gold.WithoutCurrent().ForEachAsync(n =>
        {
            if (n == Data.UN_NEED_GOLD)
            {
                tex.text = "";
            }
        });
    }

    private void RemoveEventButtons()
    {
        _lvupDamageBtn.onClick.RemoveAllListeners();
        _lvdawnDamageBtn.onClick.RemoveAllListeners();
        _lvupSpeedBtn.onClick.RemoveAllListeners();
        _lvdawnSpeedBtn.onClick.RemoveAllListeners();
        _lvupFovBtn.onClick.RemoveAllListeners();
        _lvdawnFovBtn.onClick.RemoveAllListeners();
    }
}
