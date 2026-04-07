# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

ThiefSimulator is a 3D stealth/theft mobile game built in **Unity 6 (6000.3.11f1)** using the **Universal Render Pipeline (URP)**. It targets Android, iOS, and WebGL (deployed as a Telegram Mini App). The player steals loot from guarded locations, carries it to a sell point (a van), and upgrades their thief equipment.

## Building

This project must be built through the Unity Editor — there are no CLI build scripts. Open with Unity Hub using version `6000.3.11f1`. The two main scenes are:
- `Assets/Scenes/Game.unity` — mobile build (Android/iOS)
- `Assets/Scenes/Game Web.unity` — WebGL/Telegram build

## Architecture

### Core Framework (`Larje.Core`)

All major systems are **Services** registered via `[BindService(typeof(MyService))]` on a MonoBehaviour that extends `Service`. Dependencies between services are injected with `[InjectService]`. Non-service MonoBehaviours call `DIContainer.InjectTo(this)` in `Start()` to receive injections.

```csharp
[BindService(typeof(MyService))]
public class MyService : Service {
    [InjectService] private IDataService _dataService;
    public override void Init() { /* called after injection */ }
}

public class SomeMonoBehaviour : MonoBehaviour {
    [InjectService] private IGameStateService _gameStateService;
    private void Start() { DIContainer.InjectTo(this); }
}
```

Key services: `ThiefGameService` (main game loop), `MiniGameLauncherService`, `UpgradesService`. Framework services (interfaces): `IDataService`, `IGameStateService`, `ILevelManagerService`, `ICurrencyService`, `IAdsService`, `IAnalyticsService`, `UIService`.

### Game Loop (`ThiefGameService`)

Game states flow: `Loading → Menu → Cutscene → Playing → Win/Fail`. `ThiefGameService` orchestrates level loading, UI screen transitions, and ad display. It listens to `IGameStateService.EventGameStateChanged`.

### Level System

`ThiefLevel : LevelProcessor` is instantiated per level. On `Start()` it builds the NavMesh, grabs all `Character` and `Sellable` components in children, and tracks loot sold vs total. When 100% loot is sold, it fires `LevelEventProgressComplete` via `GameEventService`. `PlayerSpawner` (on the Van prefab) listens for that event to activate the van escape trigger.

Level win condition: player returns to van after selling all loot → `PlayerSpawner` calls `ILevelManagerService.TryStopCurrentLevel(StopData(true, LevelStopType.Win))`.

### Character / Ability System (`Larje.Character`)

Characters use a composable ability system. Abilities extend `CharacterAbility` and are initialized via `character.FindAbility<T>()`. Key abilities used in-game:
- `CharacterCarry3D` — pick up/stack `Carryable` items; weight-based speed penalty; implements `IPlayerActionSource` to expose contextual action buttons
- `CharacterFOV` — line-of-sight detection
- `CharacterHealth` / `Health` — damage/death

### Enemy AI

`EnemyAttention : CharacterAbility` drives AI state: **Idle → Suspicious → Aggressive**. Each state maps to a `StateBrain` (an `AIBrain` GameObject). AI actions are in `Scripts/Character/AI/Actions/` (patrol, aim to player/seek area) and decisions in `Scripts/Character/AI/Decisions/`.

### Items & Upgrades

`ThiefItem` (serializable) defines item type, quality, display name, and which `UpgradeType` enums it carries. `UpgradesService` spawns `UpgradeProcessor` MonoBehaviours onto the player at runtime. Upgrade types: `MoreWeigth`, `LessSound`, `FasterAttack`, `Invisibility`, `FasterMovement`, `MoreMoney`, `LockPicking`.

### UI System

`UIService` exposes `UIScreenProcessor` (one screen at a time) and `UIPopupProcessor` (stacked popups). Open UI via:
```csharp
_uiService.GetProcessor<UIScreenProcessor>().OpenScreen(new MenuScreen.Args());
_uiService.GetProcessor<UIPopupProcessor>().OpenPopup(new RevivePopup.Args(onRevive, onFail));
```
Screens: `Loading`, `Menu`, `Play`, `Win`, `Fail`, `Shop`. Popups: `Revive`, `Pause`, `Settings`, `Item`, `Upgrades`, `MiniGame` variants.

### ProjectConstants

`Assets/Plugins/ProjectConstants/ProjectConstants.cs` is **auto-generated** from a config asset — do not edit by hand. It contains all shared enums: `CurrencyType`, `UIScreenType`, `UIPopupType`, `UpgradeType`, `MiniGameType`, `ItemType`, `GameStates`-related types, etc.

### Telegram / WebGL

`TelegramBridge` (singleton MonoBehaviour) wraps JS interop (`[DllImport("__Internal")]`) for the Telegram Mini App: alerts, bot data, invoice payments, user ID. All calls are no-ops in Editor and non-WebGL builds.

## Key Third-Party Packages

| Package | Purpose |
|---|---|
| DOTween (Demigiant) | Tweening — used everywhere for animations/delays |
| Dreamteck Splines | Spline-based movement (van trajectory, patrol paths) |
| Firebase | Analytics + backend |
| Google Mobile Ads | AdMob interstitials/rewarded |
| Addressables | Asset loading |
| Cinemachine | Camera rigs |
| AI Navigation | Runtime NavMesh baking |
