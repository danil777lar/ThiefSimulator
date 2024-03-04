using System;
namespace ProjectConstants
{
//SCRIPT IS GENERATED AUTOMATICALLY
//TO CHANGE THE CONTENT YOU SHOULD USE THE CONFIG
	public enum CurrencyType
	{
		Coins = 1,
	}

	public enum CurrencyPlacementType
	{
		Global = 1,
		Level = 2,
	}

	[Flags]
	public enum CurrencyPlacementTypes
	{
		Global = 1,
		Level = 2,
	}

	public enum LevelStartType
	{
		Start = 1,
		Restart = 2,
	}

	public enum LevelStopType
	{
		Fail = 1,
		Win = 2,
	}

	public enum SoundType
	{
	}

	public enum TimeScaleAnimationType
	{
		Start = 1,
		Stop = 2,
	}

	public enum TimeScaleLayerType
	{
		Global = 1,
		Game = 2,
	}

	public enum UIScreenType
	{
		Loading = 1,
		Menu = 2,
		Play = 4,
		Win = 8,
		Fail = 16,
	}

	public enum UIPopupType
	{
		TestMiniGame = 1,
	}

	public enum UIToastType
	{
		Info = 1,
	}

	public enum MiniGameType
	{
		TestMiniGame = 1,
	}

}
