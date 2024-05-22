using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services.UI;
using ProjectConstants;
using UnityEngine;

public abstract class MiniGamePopup : UIPopup
{
    public class MiniGameArgs : UIPopup.Args
    {
        public readonly Action OnComplete;
        public readonly Action OnFail;
        
        public MiniGameArgs(UIPopupType popupType, Action onComplete, Action onFail) : base(popupType)
        {
            OnComplete = onComplete;
            OnFail = onFail;
        }
    }
}
