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

    private void OnDestroy()
    {
        Data.Instance.Dispose();
    }

    public void Setup(MainViewModel view_model)
    {
        _view.Setup(view_model.OwnMoney);
        _powerup.Setup(view_model.PowerModel);
        _title.Setup(view_model.TitleBtnEvent);
    }
}
