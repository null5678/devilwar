using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using TMPro;

public class MainView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _ownMoneyText;

    public TextMeshProUGUI OwnMoneyText { get { return _ownMoneyText; } }
    
    public void Setup(AsyncReactiveProperty<int> gold)
    {
        gold.WithoutCurrent().BindTo(OwnMoneyText);
    }
}
