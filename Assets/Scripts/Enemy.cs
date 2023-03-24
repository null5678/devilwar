using System.Threading;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum Type
    {
        Normal,
        Navi,
        Boss,
    }

    [SerializeField]
    private float _speed = 0f;
    [SerializeField]
    private GameObject _gold;
    [SerializeField]
    private Sprite _sprite;

    public Player Player { get; set; }
    public PoolGold PoolGold { get; set; }
    public Type EnemyType { get; set; }
    public Vector3 Pos { get { return transform.position; } }
    public Vector3 Velo { get; set; }
    public float Hp { get; set; } = 10f;
    public float SpeedMag { get; set; } = 1f;
    public bool isTitle { get; set; } = false;

    private CancellationTokenSource _cts;
    private Rigidbody2D _rgd2d;
    private Vector3 _pos = Vector3.zero;
    private bool _isDead = false;
    private float _outSideViewValue = 10f;



    private void Awake()
    {
        _rgd2d = GetComponent<Rigidbody2D>();    
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (!_isDead)
        {
            if(!isTitle) Velo = Player.Pos - Pos;

            _rgd2d.AddForce(Velo.normalized * (_speed * SpeedMag));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            Bullet b = collision.GetComponent<Bullet>();
            b.DisableBullet();
            Hp -= b.Damage;
            if (Hp <= 0)
            {
                Dead();
            }
        }
    }

    public async UniTask TitleAnimDeadWait()
    {
        _cts = new CancellationTokenSource();

        await UniTask.Delay(10 * 1000, cancellationToken: _cts.Token);

        Dead(true);
    }

    public void Spawn()
    {
        gameObject.SetActive(true);
        _isDead = false;

        var deg = Random.Range(0f, 360f);
        _pos.x = (_outSideViewValue * Mathf.Sin(deg * Mathf.PI / 180f)) + Player.Pos.x;
        _pos.y = (_outSideViewValue * Mathf.Cos(deg * Mathf.PI / 180f)) + Player.Pos.y;
        transform.position = _pos;

        Hp = 10;
    }
    public void Dead(bool is_goal = false)
    {
        gameObject.SetActive(false);
        _isDead = true;
        //Instantiate(_gold, transform.position, Quaternion.identity);
        if (is_goal) return;

        if(EnemyType == Type.Normal)
        {
            PoolGold.GenerateGold(Pos);
        }
        else if(EnemyType == Type.Boss)
        {
            PoolGold.GenerateGoldBoss(Pos);
        }
        else if(EnemyType == Type.Navi)
        {
            PoolGold.GenerateNavi(Pos);
        }
    }

    public void ChangeBossSp()
    {
        if (_sprite is not Sprite) return;

        GetComponent<SpriteRenderer>().sprite = _sprite;
    }

    public void ReleaseCts()
    {
        _cts?.Cancel();
        _cts?.Dispose();

        _cts = null;
    }
}
