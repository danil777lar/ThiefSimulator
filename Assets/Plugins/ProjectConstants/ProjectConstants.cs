using System;
namespace ProjectConstants
{
//SCRIPT IS GENERATED AUTOMATICALLY
//TO CHANGE THE CONTENT YOU SHOULD USE THE CONFIG
	public enum CurrencyType
	{
		Coins = 1 << 0,
	}

	public enum CurrencyPlacementType
	{
		Global = 1 << 0,
		Level = 1 << 1,
	}

	[Flags]
	public enum CurrencyPlacementTypes
	{
		Global = 1 << 0,
		Level = 1 << 1,
	}

	public enum LevelStartType
	{
		Start = 1 << 0,
		Restart = 1 << 1,
	}

	public enum LevelStopType
	{
		Fail = 1 << 0,
		Win = 1 << 1,
	}

	public enum SoundType
	{
		UI_Click = 1 << 0,
	}

	public enum TimeScaleAnimationType
	{
		Start = 1 << 0,
		Stop = 1 << 1,
		StopSmooth = 1 << 2,
	}

	public enum TimeScaleLayerType
	{
		Global = 1 << 0,
		Game = 1 << 1,
	}

	public enum UIScreenType
	{
		Loading = 1 << 0,
		Menu = 1 << 1,
		Play = 1 << 2,
		Win = 1 << 3,
		Fail = 1 << 4,
		Shop = 1 << 5,
	}

	[Flags]
	public enum UIScreenTypes
	{
		Loading = 1 << 0,
		Menu = 1 << 1,
		Play = 1 << 2,
		Win = 1 << 3,
		Fail = 1 << 4,
		Shop = 1 << 5,
	}

	public enum UIPopupType
	{
		TestMiniGame = 1 << 0,
		LockMiniGame = 1 << 1,
		Item = 1 << 2,
		Pause = 1 << 3,
		Settings = 1 << 4,
		Revive = 1 << 5,
		Upgrades = 1 << 6,
	}

	public enum UIToastType
	{
		Info = 1 << 0,
	}

	public enum MiniGameType
	{
		TestMiniGame = 1 << 0,
		LockMiniGame = 1 << 1,
	}

	public enum ItemType
	{
		Hats = 1 << 0,
		Jackets = 1 << 1,
		Pants = 1 << 2,
	}

	[Flags]
	public enum ItemTypes
	{
		Hats = 1 << 0,
		Jackets = 1 << 1,
		Pants = 1 << 2,
	}

	public enum UpgradeType
	{
		MoreWeigth = 1 << 0,
		LessSound = 1 << 1,
		FasterAttack = 1 << 2,
		Invisibility = 1 << 3,
		FasterMovement = 1 << 4,
		MoreMoney = 1 << 5,
		LockPicking = 1 << 6,
	}

	[Flags]
	public enum UpgradeTypes
	{
		MoreWeigth = 1 << 0,
		LessSound = 1 << 1,
		FasterAttack = 1 << 2,
		Invisibility = 1 << 3,
		FasterMovement = 1 << 4,
		MoreMoney = 1 << 5,
		LockPicking = 1 << 6,
	}

}
