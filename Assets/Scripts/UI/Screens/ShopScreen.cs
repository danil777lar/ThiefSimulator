using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services.UI;
using ProjectConstants;
using UnityEngine;

public class ShopScreen : UIScreen
{
    public new class Args : UIScreen.Args
    {
        public Args() : base(UIScreenType.Shop)
        {
        }
    }
}
