using System;
using UniRx;

namespace Devilwar.UI
{
    public class MyButton : UnityEngine.UI.Button
    {
        private Action _onTapEvent;

        private new void Awake()
        {
            this.OnClickAsObservable().Subscribe(_ => _onTapEvent());
        }

        public void RegisterTapEvent(Action action)
        {
            _onTapEvent = action;
        }
    }
}