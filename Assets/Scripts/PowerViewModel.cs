using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PowerViewModel
{
    public AsyncReactiveProperty<int> LvDamage { get; set; }
    public AsyncReactiveProperty<int> LvSpeed { get; set; }
    public AsyncReactiveProperty<int> LvFov { get; set; }
    public AsyncReactiveProperty<int> NeedGoldDamage { get; set; }
    public AsyncReactiveProperty<int> NeedGoldSpeed { get; set; }
    public AsyncReactiveProperty<int> NeedGoldFov { get; set; }
    public Action LvupDamageEvent { get; set; }
    public Action LvdawnDamageEvent { get; set; }
    public Action LvupSpeedEvent { get; set; }
    public Action LvdawnSpeedEvent { get; set; }
    public Action LvupFovEvent { get; set; }
    public Action LvdawnFovEvent { get; set; }
}
