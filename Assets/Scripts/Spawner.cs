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
    private PoolGold _poolGold;

    public int MaxEnemy { get; set; } = 10;
    public int SpawnNum { get; set; } = 3;
    public int SpawnWaitSec { get; set; } = 3000;

    private List<GameObject> _listEnemys = new List<GameObject>();
    private CancellationTokenSource _cts;

    public async UniTask Init()
    {
        for (int i = 0; i < MaxEnemy; i++)
        {
            GameObject obj = Instantiate(_enemyPrefab, transform);
            obj.GetComponent<Enemy>().Player = _player;
            obj.GetComponent<Enemy>().PoolGold = _poolGold;
            if(i == 0)
            {
                obj.GetComponent<Enemy>().EnemyType = Enemy.Type.Navi;
            }
            else
            {
                obj.GetComponent<Enemy>().EnemyType = Enemy.Type.Normal;
            }
            obj.SetActive(false);
            _listEnemys.Add(obj);
        }       
    }

    public void TitleAnimSpawnStart()
    {
        TitleAnimSpawn().Forget();
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
                    if (!v.activeInHierarchy && v.GetComponent<Enemy>().EnemyType == Enemy.Type.Normal)
                    {
                        v.GetComponent<Enemy>().Spawn();
                        v.GetComponent<Enemy>().SpeedMag = 7f;
                        v.GetComponent<Enemy>().isTitle = true;
                        v.GetComponent<Enemy>().Velo = Vector3.zero - v.GetComponent<Enemy>().Pos;
                        v.GetComponent<Enemy>().TitleAnimDeadWait().Forget();

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
                    if (!v.activeInHierarchy && v.GetComponent<Enemy>().EnemyType == Enemy.Type.Navi)
                    {
                        int r = UnityEngine.Random.Range(1, 10);
                        if (r == 1)
                        {
                            v.GetComponent<Enemy>().Spawn();
                            v.GetComponent<Enemy>().SpeedMag = 1f;
                            i++;
                        }
                    }
                    else if (!v.activeInHierarchy && v.GetComponent<Enemy>().EnemyType == Enemy.Type.Normal)
                    {
                        v.GetComponent<Enemy>().Spawn();
                        if(i % 2 == 0)
                        {
                            v.GetComponent<Enemy>().SpeedMag = 1f;
                        }
                        else
                        {
                            v.GetComponent<Enemy>().SpeedMag = 0.7f;
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
            //await UniTask.Delay(5 * 1000, cancellationToken: cts.Token);
            await UniTask.Delay(3 * 60 * 1000, cancellationToken: cts.Token);

            ene = Instantiate(_enemyPrefab, transform).GetComponent<Enemy>();
            ene.Player = _player;
            ene.PoolGold = _poolGold;
            ene.EnemyType = Enemy.Type.Boss;
            ene.transform.localScale = Vector3.one;
            ene.ChangeBossSp();
            _listEnemys.Add(ene.gameObject);

            ene.Spawn();
            ene.Hp = 500f;

            SoundManeger.Instance.BgmPlay(SoundManeger.BGM_02).Forget();

            await UniTask.WaitWhile(() => ene is Enemy && ene.gameObject.activeInHierarchy, cancellationToken: cts.Token);

            _listEnemys.Remove(ene.gameObject);
            Destroy(ene.gameObject);
        }
        catch(OperationCanceledException e)
        {

        }
    }

    public void AllDead()
    {
        foreach(var v in _listEnemys)
        {
            if(v.activeInHierarchy)
            {
                v.GetComponent<Enemy>().Dead(true);
                v.GetComponent<Enemy>().isTitle = false;
                v.GetComponent<Enemy>().ReleaseCts();
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
        //_cts?.Dispose();
    }

    private void Update()
    {
        
    }
}
