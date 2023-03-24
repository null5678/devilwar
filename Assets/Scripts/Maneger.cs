using System.Collections;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;

public class Maneger : MonoBehaviour
{
    [SerializeField]
    private Player _player;
    [SerializeField]
    private Spawner _spawner;
    [SerializeField]
    private Goal _goal;
    [SerializeField]
    private Powerup _powerup;
    [SerializeField]
    private Data _data;
    [SerializeField]
    private PoolGold _poolGold;
    [SerializeField]
    private Title _title;
    [SerializeField]
    private GameObject _tutorialObj;

    private void Start()
    {
        InitRoutine().Forget();
    }

    private void Update()
    {
    }

    private async UniTask InitRoutine()
    {
        await SoundManeger.Instance.Init();

        await _player.Init();
        await _spawner.Init();

        _tutorialObj.SetActive(false);
        SoundManeger.Instance.BgmPlay(SoundManeger.BGM_01).Forget();

        _powerup.DisablePowerup();

        _goal.DisableActive();

        await UniTask.WhenAll(_title.TitleRoutine(), _spawner.TitleAnimSpawn());
        _spawner.AllDead();

        _player.EnablePlayer();

        _title.DisableActive();

        //_spawner.ReleaseCts();

        _tutorialObj.SetActive(true);
        MainRoutine().Forget();
    }

    private async UniTask MainRoutine()
    {
        var cts = new CancellationTokenSource();
        _goal.Init(_player.Pos);

        SoundManeger.Instance.BgmPlay(SoundManeger.BGM_04).Forget();
        await _player.GameStart(Data.Instance);

        _spawner.GameStart();
        _spawner.SpawnBoss(cts).Forget();

        await UniTask.WaitUntil(() => _goal.isGoal || _player.isDied);

        _spawner.ArrivedGoal();


        if (_goal.isGoal)
        {
            await _player.ArrivedGoal();
        }
        else if(_player.isDied)
        {
            await _player.DeadAnim();
        }

        _tutorialObj.SetActive(false);
        _poolGold.AllPickup();
        PowerupRoutine().Forget();

        cts.Cancel();
    }

    private async UniTask PowerupRoutine()
    {
        if(_player.isDied)
        {
            Data.Instance.ResetLv();
            //_data.ResetLv();
        }

        Data.Instance.PowerupInit();
        //_data.PowerupInit();
        _powerup.EnablePowerup();

        _powerup.PlayAnimTutorial();

        await UniTask.WaitUntil(() => _powerup.isNext);
        _powerup.StopAnimTutorial();
        _powerup.DisablePowerup();

        //_poolGold.AllPickup();
        _goal.isGoal = false;
        _tutorialObj.SetActive(true);
        MainRoutine().Forget();
    }
}
