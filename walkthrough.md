# Walkthrough - Micro-Defense Engine & Core System Implementation

We have built the entire Data-Driven Architecture, Game Loop Engine, Tower Mechanics, Enemy Behaviors, Wave Systems, Grid Placement, and UI Controllers for **Micro-Defense: Cellular Tower Defense Strategy (v0.1.1 GDD)**.

---

## 🛠️ Created System Components

### 1. Data Architecture (`Assets/Scripts/Data/`)
- [`TowerDataSO.cs`](file:///c:/UnityGame/Micro-Defence/Assets/Scripts/Data/TowerDataSO.cs): ScriptableObject containing stats for B-Cell, Macrophage, and T-Cell across Tier 1, Tier 2, and Tier 3. Includes Special Ability definitions (*Agglutination Slow*, *Continuous Phagocytosis*, *Armor Shred*, *Apoptosis Blast*).
- [`EnemyDataSO.cs`](file:///c:/UnityGame/Micro-Defence/Assets/Scripts/Data/EnemyDataSO.cs): ScriptableObject for enemy stats (Base HP, Speed, Flat Armor, Damage To Cell, Plasma Reward, Splitter rules, and Boss Replication Pool).
- [`WaveDataSO.cs`](file:///c:/UnityGame/Micro-Defence/Assets/Scripts/Data/WaveDataSO.cs): ScriptableObject for defining spawn groups, delays, and wave completion rewards.

---

### 2. Core Game Loop Engine (`Assets/Scripts/Core/`)
- [`GameManager.cs`](file:///c:/UnityGame/Micro-Defence/Assets/Scripts/Core/GameManager.cs): Singleton for overall game state transitions (`Preparation`, `WaveInProgress`, `Paused`, `Defeat`, `Victory`).
- [`PlasmaManager.cs`](file:///c:/UnityGame/Micro-Defence/Assets/Scripts/Core/PlasmaManager.cs): Economy manager handling Plasma spending/rewards with C# event delegate `OnPlasmaChanged`.
- [`LeakManager.cs`](file:///c:/UnityGame/Micro-Defence/Assets/Scripts/Core/LeakManager.cs): Tracks leak counter (default 5 max leaks) and triggers defeat event.
- [`WaveManager.cs`](file:///c:/UnityGame/Micro-Defence/Assets/Scripts/Core/WaveManager.cs): Handles async enemy wave spawning, active enemy tracking, and victory conditions.

---

### 3. Tower Defense Units (`Assets/Scripts/Towers/`)
- [`TowerBase.cs`](file:///c:/UnityGame/Micro-Defence/Assets/Scripts/Towers/TowerBase.cs): Abstract base handling targeting, range checks, attack speed timers, and tier upgrades.
- [`BCellTower.cs`](file:///c:/UnityGame/Micro-Defence/Assets/Scripts/Towers/BCellTower.cs): Ranged single-target shooter with Tier 3 *Agglutination* (30% slow for 2s).
- [`MacrophageTower.cs`](file:///c:/UnityGame/Micro-Defence/Assets/Scripts/Towers/MacrophageTower.cs): Melee chokepoint blocker with Cleave damage, Tier 2 damage reduction, and Tier 3 *Continuous Phagocytosis* (5% Max HP heal on enemy kill).
- [`TCellTower.cs`](file:///c:/UnityGame/Micro-Defence/Assets/Scripts/Towers/TCellTower.cs): AOE Slash attacker with Tier 2 *Armor Shred* (-25% armor) and Tier 3 *Apoptosis Blast* (50% True Damage ignoring armor).

---

### 4. Enemy AI & Behaviors (`Assets/Scripts/Enemies/`)
- [`EnemyBase.cs`](file:///c:/UnityGame/Micro-Defence/Assets/Scripts/Enemies/EnemyBase.cs): Manages health, flat armor reduction calculation, status effect timers (slow and armor shred), and Plasma reward dispatch.
- [`PathFollower.cs`](file:///c:/UnityGame/Micro-Defence/Assets/Scripts/Enemies/PathFollower.cs): Waypoint movement script for smooth grid navigation.
- [`SplitterEnemy.cs`](file:///c:/UnityGame/Micro-Defence/Assets/Scripts/Enemies/SplitterEnemy.cs): Cell Division logic for Swarm Virus spawning 3 Mini Viruses on death.
- [`BossEnemy.cs`](file:///c:/UnityGame/Micro-Defence/Assets/Scripts/Enemies/BossEnemy.cs): Boss Omega *Replication Call* skill spawning minions every 8–10 seconds.

---

### 5. Grid Placement & UI (`Assets/Scripts/Grid/` & `Assets/Scripts/UI/`)
- [`PlacementGridManager.cs`](file:///c:/UnityGame/Micro-Defence/Assets/Scripts/Grid/PlacementGridManager.cs): Grid raycasting, valid placement checks, plasma deduction, and tower instantiation.
- [`HUDController.cs`](file:///c:/UnityGame/Micro-Defence/Assets/Scripts/UI/HUDController.cs): Connects Plasma, Wave status, and Leak counters directly to the UI elements.
- [`TowerShopUI.cs`](file:///c:/UnityGame/Micro-Defence/Assets/Scripts/UI/TowerShopUI.cs): Tower shop selection buttons and Wave Start trigger.

---

## 🎮 How to Setup in Unity Editor

1. **Create ScriptableObject Assets**:
   - Right-click in Unity Project window $\rightarrow$ `Create` $\rightarrow$ `Micro-Defense` $\rightarrow$ `Tower Data` (Create Assets for B-Cell, Macrophage, T-Cell).
   - Right-click $\rightarrow$ `Create` $\rightarrow$ `Micro-Defense` $\rightarrow$ `Enemy Data` (Create Assets for Common, Mutant, Armored, Swarm, Mini, Boss).
   - Right-click $\rightarrow$ `Create` $\rightarrow$ `Micro-Defense` $\rightarrow$ `Wave Data` (Configure waves).
2. **Setup Scene GameObjects**:
   - Add an Empty GameObject named `_Managers` and attach `GameManager`, `PlasmaManager`, `LeakManager`, `WaveManager`, and `PlacementGridManager`.
   - Setup Grid Tiles with Collider and `GridTile` component on Layer `PlacementTile`.
