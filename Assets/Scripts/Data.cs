using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Data : IDisposable
{
    public static readonly int MAX_LEVEL = 5;
    public static readonly int UN_NEED_GOLD = -1;

    public enum DataType
    {
        Damage,
        Speed,
        Fov
    }

    public static Data Instance 
    {
        get
        {
            if(_instance is not Data)
            {
                _instance = new Data();
            }

            return _instance;
        }
    }

    public AsyncReactiveProperty<int> OwnMoney { get; private set; } = new AsyncReactiveProperty<int>(0);

    public AsyncReactiveProperty<int> LvDamage { get; private set; } = new AsyncReactiveProperty<int>(1);
    public AsyncReactiveProperty<int> NeedGoldDamgage { get; private set; } = new AsyncReactiveProperty<int>(0);
    public AsyncReactiveProperty<int> LvSpeed { get; private set; } = new AsyncReactiveProperty<int>(1);
    public AsyncReactiveProperty<int> NeedGoldSpeed { get; private set; } = new AsyncReactiveProperty<int>(0);
    public AsyncReactiveProperty<int> LvFov { get; private set; } = new AsyncReactiveProperty<int>(1);
    public AsyncReactiveProperty<int> NeedGoldFov { get; private set; } = new AsyncReactiveProperty<int>(0);

    private static Data _instance = null;
    private float[] _damageMag = new float[] { 1f, 2f, 3f, 4f, 5f };
    private float[] _speedMag = new float[] { 1f, 1.2f, 1.4f, 1.6f, 1.8f };
    private float[] _fovMag = new float[] { 1f, 1.2f, 1.3f, 1.4f, 1.5f };
    private int[] _needGold = new int[] { 10, 20, 30, 40, UN_NEED_GOLD };
    private int _lvDamageBefore = 0;
    private int _lvSpeedBefore = 0;
    private int _lvFovBefore = 0;

    public Data()
    {
        NeedGoldDamgage.Value = _needGold[LvDamage.Value - 1];
        NeedGoldSpeed.Value = _needGold[LvSpeed.Value - 1];
        NeedGoldFov.Value = _needGold[LvFov.Value - 1];
    }

    public void ResetLv()
    {
        LvDamage.Value = 1;
        LvSpeed.Value = 1;
        LvFov.Value = 1;
    }

    public float GetMagData(DataType type)
    {
        switch(type)
        {
            case DataType.Damage:
                if (LvDamage.Value == 0) break;

                return _damageMag[LvDamage.Value - 1];
            case DataType.Speed:
                if (LvSpeed.Value == 0) break;

                return _speedMag[LvSpeed.Value - 1];
            case DataType.Fov:
                if (LvFov.Value == 0) break;

                return _fovMag[LvFov.Value - 1];
        }

        return 1f;
    }

    public void PowerupInit()
    {
        _lvDamageBefore = LvDamage.Value;
        _lvSpeedBefore = LvSpeed.Value;
        _lvFovBefore = LvFov.Value;

        SoundManeger.Instance.BgmPlay(SoundManeger.BGM_03).Forget();
    }

    public void DamageLevelup(DataType type)
    {
        AsyncReactiveProperty<int> lvProperty = null;
        AsyncReactiveProperty<int> needGold = null;

        switch(type)
        {
            case DataType.Damage:
                lvProperty = LvDamage;
                needGold = NeedGoldDamgage;
                break;
            case DataType.Speed:
                lvProperty = LvSpeed;
                needGold = NeedGoldSpeed;
                break;
            case DataType.Fov:
                lvProperty = LvFov;
                needGold = NeedGoldFov;
                break;
        }

        if (lvProperty.Value < MAX_LEVEL &&
            OwnMoney.Value >= _needGold[lvProperty.Value - 1])
        {
            OwnMoney.Value -= _needGold[lvProperty.Value - 1];
            lvProperty.Value++;
        }

        needGold.Value = _needGold[lvProperty.Value - 1];
    }
    public void DamageLeveldawn(DataType type)
    {
        AsyncReactiveProperty<int> lvProperty = null;
        AsyncReactiveProperty<int> needGold = null;
        int _before = 0;

        switch (type)
        {
            case DataType.Damage:
                lvProperty = LvDamage;
                needGold = NeedGoldDamgage;
                _before = _lvDamageBefore;
                break;
            case DataType.Speed:
                lvProperty = LvSpeed;
                needGold = NeedGoldSpeed;
                _before = _lvSpeedBefore;
                break;
            case DataType.Fov:
                lvProperty = LvFov;
                needGold = NeedGoldFov;
                _before = _lvFovBefore;
                break;
        }

        if (lvProperty.Value > _before)
        {
            lvProperty.Value--;
            OwnMoney.Value += _needGold[lvProperty.Value - 1];
        }

        needGold.Value = _needGold[lvProperty.Value - 1];
    }

    public void AddOwnMoney(int value)
    {
        OwnMoney.Value += value;
    }

    public void Dispose()
    {
        OwnMoney.Dispose();
        LvDamage.Dispose();
        NeedGoldDamgage.Dispose();
        LvSpeed.Dispose();
        NeedGoldSpeed.Dispose();
        LvFov.Dispose();
        NeedGoldFov.Dispose();
    }
}
