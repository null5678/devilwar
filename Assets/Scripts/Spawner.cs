using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private Player _player;
    [SerializeField]
    private PoolItem _poolItem;

    public int MaxEnemy { get; set; } = 10;
    public int SpawnNum { get; set; } = 3;
    public int SpawnWaitSec { get; set; } = 3000;

    private List<Enemy> _listEnemys = new List<Enemy>();
    private CancellationTokenSource _cts;

    public async UniTask Init()
    {
        for (int i = 0; i < MaxEnemy; i++)
        {
            Enemy e = Instantiate(_enemyPrefab, transform).GetComponent<Enemy>();
            e.Player = _player;
            e.PoolItem = _poolItem;
            if(i == 0)
            {
                e.EnemyType = Enemy.Type.Navi;
            }
            else
            {
                e.EnemyType = Enemy.Type.Normal;
            }
            e.DisableObject();
            _listEnemys.Add(e);
        }       
    }

    public async UniTask TitleAnimSpawn()
    {
        _cts = new CancellationTokenSource();

        try
        {
            while (true)
            {
                await UniTask.Yield(_cts.Token);

                int i = 0;
                foreach (var v in _listEnemys)
                {
                    if (!v.isActive && v.EnemyType == Enemy.Type.Normal)
                    {
                        v.Spawn();
                        v.SpeedMag = 7f;
                        v.isTitle = true;
                        v.Velo = Vector3.zero - v.Pos;
                        v.TitleAnimDeadWait().Forget();

                        i++;
                    }

                    if (i >= SpawnNum)
                    {
                        break;
                    }
                }

                await UniTask.Delay(SpawnWaitSec);
            }
        }
        catch (OperationCanceledException e)
        {
            _cts.Dispose();
        }
    }

    private async UniTask Spawn()
    {
        _cts = new CancellationTokenSource();
        try
        {
            while (true)
            {
                await UniTask.Yield(_cts.Token);

                int i = 0;
                foreach (var v in _listEnemys)
                {
                    if (!v.isActive && v.EnemyType == Enemy.Type.Navi)
                    {
                        int r = UnityEngine.Random.Range(1, 10);
                        if (r == 1)
                        {
                            v.Spawn();
                            v.SpeedMag = 1f;
                            i++;
                        }
                    }
                    else if (!v.isActive && v.EnemyType == Enemy.Type.Normal)
                    {
                        v.Spawn();
                        if(i % 2 == 0)
                        {
                            v.SpeedMag = 1f;
                        }
                        else
                        {
                            v.SpeedMag = 0.7f;
                        }
                        i++;
                    }

                    if (i >= SpawnNum)
                    {
                        break;
                    }
                }

                await UniTask.Delay(SpawnWaitSec);
            }
        }
        catch (OperationCanceledException e)
        {

        }
    }

    public async UniTask SpawnBoss(CancellationTokenSource cts)
    {
        Enemy ene = null;
        try
        {
            await UniTask.Delay(3 * 60 * 1000, cancellationToken: cts.Token);

            ene = Instantiate(_enemyPrefab, transform).GetComponent<Enemy>();
            ene.Player = _player;
            ene.PoolItem = _poolItem;
            ene.EnemyType = Enemy.Type.Boss;
            ene.transform.localScale = Vector3.one;
            ene.ChangeBossSp();
            _listEnemys.Add(ene);

            ene.Spawn();
            ene.Hp = 500f;

            SoundManeger.Instance.BgmPlay(SoundManeger.BGM_02).Forget();

            await UniTask.WaitWhile(() => ene is Enemy && ene.gameObject.activeInHierarchy, cancellationToken: cts.Token);

            _listEnemys.Remove(ene);
            ene.RemoveObject();
        }
        catch(OperationCanceledException e)
        {

        }
    }

    public void AllDead()
    {
        foreach(var v in _listEnemys)
        {
            if(v.isActive)
            {
                v.Dead(true);
                v.isTitle = false;
                v.ReleaseCts();
            }
        }
    }

    public void GameStart()
    {
        Spawn().Forget();
    }

    public void ArrivedGoal()
    {
        AllDead();
        _cts.Cancel();
    }

    public void ReleaseCts()
    {
        _cts?.Cancel();
    }
}
