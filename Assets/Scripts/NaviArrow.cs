using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

public class NaviArrow : GeneralObject
{
    [SerializeField]
    private Goal _goal;

    private SpriteRenderer _sp;
    private Sequence _seq;
    private bool isfinish = true;


    private void Awake()
    {
        _sp = GetComponent<SpriteRenderer>();    
    }

    public void EnableArrow(Vector3 pos_player, float fov_mag)
    {
        var rad = Mathf.Atan2(_goal.transform.position.y - pos_player.y, _goal.transform.position.x - pos_player.x);
        Vector3 pos = Vector3.zero;

        pos.x = Mathf.Cos(rad) * (6f * fov_mag);
        pos.y = Mathf.Sin(rad) * (6f * fov_mag);

        transform.localPosition = pos;
        transform.rotation = Quaternion.Euler(0, 0, rad * Mathf.Rad2Deg + 180f);

        if(isfinish)
        {
            isfinish = false;
            _seq = DOTween.Sequence();
            _seq.SetDelay(3f);
            _seq.Append(_sp.DOFade(0f, 2f).SetEase(Ease.Flash, 10).OnComplete(() => isfinish = true));

            DisableDelay().Forget();
        }
        else
        {
            _seq.Restart();
        }

        _sp.color = Color.white;
        EnableObject();
    }

    public void DisableArrow()
    {
        gameObject.SetActive(false);
    }

    private async UniTask DisableDelay()
    {
        _seq.Play();

        await UniTask.WaitUntil(() => isfinish);

        DisableObject();
    }
}
