using System;
using System.Linq; // ’²‚×‚é unirx‚à
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    [SerializeField]
    private Data _data;
    [SerializeField]
    private float _speed = 0f;
    [SerializeField]
    private int _goldMag = 1;

    [SerializeField]
    private Transform _poolsParent;
    [SerializeField]
    private GameObject _bulletPrefab;
    [SerializeField]
    private GameObject _fov;
    [SerializeField]
    private HpView _hpView;
    [SerializeField]
    private NaviArrow _navi;

    private readonly int MAX_BULLET = 5;

    public int Hp { get; set; } = 3;
    public Vector3 Pos { get { return transform.position; } }
    public bool isDied { get { return Hp <= 0; } }

    private Rigidbody2D _rgd2d;
    private SpriteRenderer _sp;
    private List<Bullet> _listBullets = new List<Bullet>();
    private CancellationTokenSource _cts;
    private Vector2 _vec = Vector2.zero;
    private bool isInput = true;
    private bool isDamage = false;
    private float _vecX = 0f;
    private float _vecY = 0f;
    private float _fovValue = 12f;
    private float _speedMag = 1f;
    private float _fovMag = 1f;
    private float _damageMag = 1f;

    public async UniTask Init()
    {
        _rgd2d = GetComponent<Rigidbody2D>();
        _sp = GetComponent<SpriteRenderer>();
        _navi.DisableArrow();

        for (int i = 0; i < MAX_BULLET; i++)
        {
            var bullet = Instantiate(_bulletPrefab, _poolsParent).GetComponent<Bullet>();
            bullet.gameObject.SetActive(false);
            _listBullets.Add(bullet);
        }

        gameObject.SetActive(false);
    }

    private void Update()
    {
        if(!isInput) return;

        _vecX = 0f;
        _vecY = 0f;
        if(Input.GetKey(KeyCode.W))
        {
            _vecY = 1f;
        }
        if(Input.GetKey(KeyCode.A))
        {
            _vecX = -1f;
        }
        if(Input.GetKey(KeyCode.S))
        {
            _vecY = -1f;
        }
        if(Input.GetKey(KeyCode.D))
        {
            _vecX = 1f;
        }
    }

    private void FixedUpdate()
    {
        if (!isInput) return;

        _vec.x = _vecX * (_speed * _speedMag);
        _vec.y = _vecY * (_speed * _speedMag);
        _rgd2d.AddForce(_vec);
    }

    private void OnDestroy()
    {
        
    }

    public async UniTask GameStart(Data data)
    {
        _speedMag = data.GetMagData(Data.DataType.Speed);
        _fovMag   = data.GetMagData(Data.DataType.Fov);
        _damageMag = data.GetMagData(Data.DataType.Damage);

        Hp = 3;
        _hpView.AllEnable();

        _sp.color = Color.white;

        await FovOpenAnim();

        isInput = true;
        ShotBullet().Forget();
    }

    public async UniTask FovOpenAnim()
    {
        bool isend = false;

        _fov.transform.DOScale(_fovValue * _fovMag, 1f)
                      .OnComplete(() => isend = true);

        await UniTask.WaitUntil(() => isend);
    }

    public async UniTask FovCloseAnim()
    {
        bool isend = false;
        var seq = DOTween.Sequence();

        seq.Append(_fov.transform.DOScale(3f, 0.5f));
        seq.Append(_fov.transform.DOScale(0f, 1f).SetDelay(1f));
        seq.OnComplete(() => isend = true);

        seq.Play();

        await UniTask.WaitUntil(() => isend);
    }

    public async UniTask ArrivedGoal()
    {
        isInput = false;

        _cts?.Cancel();
        _cts?.Dispose();

        await FovCloseAnim();

        transform.position = Vector3.zero;
        _hpView.AllDisable();
    }

    public async UniTask DeadAnim()
    {
        isInput = false;

        _cts?.Cancel();
        _cts?.Dispose();

        Color c = Color.black;
        bool isfinish = false;
        var seq = DOTween.Sequence();
        seq.Join(_sp.DOFade(0.5f, 0.8f).SetEase(Ease.Flash, 8));
        seq.Join(DOTween.To(value => 
        {
            c.r = value;
            c.g = value;
            c.b = value;
            c.a = _sp.color.a;
            _sp.color = c;
        }, 1, 0, 1f));
        seq.SetDelay(0.5f);
        seq.Append(_sp.DOFade(0f, 0.1f));
        seq.Append(_fov.transform.DOScale(0f, 1f).SetDelay(1f));
        seq.OnComplete(() => isfinish = true);

        seq.Play();

        await UniTask.WaitUntil(() => isfinish);
    }

    public void EnablePlayer()
    {
        gameObject.SetActive(true);
    }


    private async UniTask ShotBullet()
    {
        int index = 0;
        Vector2 velo = new Vector2(0, 1);
        _cts = new CancellationTokenSource();

        try
        {
            while (true)
            {
                await UniTask.Delay(100, cancellationToken: _cts.Token);


                velo = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);

                _listBullets[index].DamageMag = _damageMag;
                _listBullets[index].Shot(velo.normalized);
                _listBullets[index].EnableBullet();
                _listBullets[index].transform.position = transform.position + new Vector3(velo.normalized.x, velo.normalized.y, 0f);
                index++;
                if (index >= _listBullets.Count)
                {
                    index = 0;
                }
            }
        }
        catch (OperationCanceledException e)
        {

        }
    }

    private async UniTask DamageAnim()
    {
        bool isfinish = false;

        var seq = DOTween.Sequence();
        seq.Append(_sp.DOFade(0.5f, 0.9f).SetEase(Ease.Flash, 8));
        seq.Append(_sp.DOFade(1f, 0.1f).OnComplete(() => isfinish = true));

        seq.Play();

        await UniTask.WaitUntil(() => isfinish);

        isDamage = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isInput) return;

        if (collision.tag == "Gold")
        {
            Data.Instance.AddOwnMoney(collision.GetComponent<Gold>().Value * _goldMag);
            collision.GetComponent<Gold>().Pickup();
            SoundManeger.Instance.SePlay(SoundManeger.SE_01).Forget();
        }
        else if(collision.tag == "Navi")
        {
            _navi.EnableArrow(Pos, _fovMag);
            collision.GetComponent<Navi>().Pickup();
            SoundManeger.Instance.SePlay(SoundManeger.SE_02).Forget();
        }
        else if(collision.tag == "Goal")
        {
            collision.GetComponent<Goal>().isGoal = true;
            return;
        }

        if (collision.tag == "Enemy")
        {
            if (!isDamage)
            {
                Hp--;
                if (Hp <= 0) Hp = 0;
                _hpView.UpdateHp(Hp);
                isDamage = true;
                SoundManeger.Instance.SePlay(SoundManeger.SE_03).Forget();

                DamageAnim().Forget();
            }
        }
    }
}
