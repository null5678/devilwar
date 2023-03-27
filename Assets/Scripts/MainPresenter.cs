using System.Threading;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MainPresenter : MonoBehaviour
{
    [SerializeField]
    private Player _player;
    [SerializeField]
    private Data _data;
    [SerializeField]
    private Spawner _spawner;
    [SerializeField]
    private MainView _view;
    [SerializeField]
    private Powerup _powerup;
    [SerializeField]
    private Title _title;

    // Start is called before the first frame update
    void Start()
    {
        Data.Instance.OwnMoney.WithoutCurrent().BindTo(_view.OwnGoldText);

        _powerup.BindLvText(Data.Instance.LvDamage, Data.DataType.Damage).Forget();
        _powerup.BindLvText(Data.Instance.LvSpeed, Data.DataType.Speed).Forget();
        _powerup.BindLvText(Data.Instance.LvFov, Data.DataType.Fov).Forget();

        _powerup.BindNeedGold(Data.Instance.NeedGoldDamgage, Data.DataType.Damage).Forget();
        _powerup.BindNeedGold(Data.Instance.NeedGoldSpeed, Data.DataType.Speed).Forget();
        _powerup.BindNeedGold(Data.Instance.NeedGoldFov, Data.DataType.Fov).Forget();

        _powerup.OnLvupDamageEvent(() => Data.Instance.DamageLevelup(Data.DataType.Damage));
        _powerup.OnLvdawnDamageEvent(() => Data.Instance.DamageLeveldawn(Data.DataType.Damage));
        _powerup.OnLvupSpeedEvent(() => Data.Instance.DamageLevelup(Data.DataType.Speed));
        _powerup.OnLvdawnSpeedEvent(() => Data.Instance.DamageLeveldawn(Data.DataType.Speed));
        _powerup.OnLvupFovEvent(() => Data.Instance.DamageLevelup(Data.DataType.Fov));
        _powerup.OnLvdawnFovEvent(() => Data.Instance.DamageLeveldawn(Data.DataType.Fov));

        _title.BtnEvent = _spawner.ReleaseCts;
    }

    private void OnDestroy()
    {
        Data.Instance.Dispose();
    }
}
