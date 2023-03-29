using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MainViewModel
{
    public PowerViewModel PowerModel { get; set; }
    public AsyncReactiveProperty<int> OwnMoney { get; set; }
    public Action TitleBtnEvent { get; set; }
}
