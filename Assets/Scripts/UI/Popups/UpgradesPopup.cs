using Larje.Core.Services.UI;
using ProjectConstants;
using UnityEngine;

public class UpgradesPopup : UIPopup
{

    public new class Args : UIPopup.Args
    {
        public Args() : base(UIPopupType.Upgrades)
        {
        }

        public Args(UIPopupCombinationType combinationType) : base(UIPopupType.Upgrades, combinationType)
        {
        }
    }
}
