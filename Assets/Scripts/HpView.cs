using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class HpView : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _hp;

    public void UpdateHp(int hp)
    {
        _hp[hp].SetActive(false);
    }

    public void AllEnable()
    {
        foreach (var v in _hp)
        {
            v.SetActive(true);
        }
    }
    public void AllDisable()
    {
        foreach (var v in _hp)
        {
            v.SetActive(false);
        }
    }
}
