using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private readonly float BASE_DAMAGE = 1f;

    [SerializeField]
    private float _spped;
    private Rigidbody2D _rdg2d;

    private Vector2 _shotVelo = Vector2.zero;
    public Vector2 ShotVelo { get; set; } = Vector2.zero;
    public float Damage { get{ return BASE_DAMAGE * DamageMag; } }
    public float DamageMag { get; set; } = 1f;

    private void Awake()
    {
        _rdg2d = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        _rdg2d = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        _rdg2d.AddForce(_shotVelo * _spped);
        //_rdg2d.AddForce(ShotVelo * _spped);
    }

    public void Shot(Vector2 velo)
    {
        _shotVelo = velo;
        _rdg2d.velocity = Vector2.zero;
    }

    public void DisableBullet()
    {
        gameObject.SetActive(false);
    }
    public void EnableBullet()
    {
        gameObject.SetActive(true);
    }
}
