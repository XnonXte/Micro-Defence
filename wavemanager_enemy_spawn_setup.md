# ⚙️ PANDUAN LENGKAP SETUP WAVEMANAGER & SPAWN ENEMY

Panduan step-by-step untuk configure WaveManager agar virus bisa spawn dengan sempurna.

---

## 📋 PREREQUISITE

Sebelum setup WaveManager, pastikan sudah ada:

- ✅ 4 virus prefab dengan NavMeshAgent + NavMeshPathFollower
- ✅ Enemy data assets (Common Virus, Armored Virus, dll)
- ✅ SpawnPoint & TargetCoreBase GameObject di scene
- ✅ _Managers GameObject dengan GameManager component
- ✅ NavMeshSurface sudah di-bake (area biru visible)

---

## 🔍 STRUCTURE: WaveDataSO vs EnemyDataSO

### EnemyDataSO (Enemy Configuration)
**Apa itu:** Data configuration untuk 1 tipe enemy

**File Location:** `Assets/Data/VirusData/` (atau folder kamu)

**Struktur:**
```
EnemyDataSO
├─ enemyType: enum (CommonVirus, MutantVirus, dll)
├─ enemyName: string (display name)
├─ enemyPrefab: GameObject (reference prefab virus)
├─ baseHealth: 100
├─ moveSpeed: 2.0 (grid units/sec)
├─ flatArmor: 0 (damage reduction)
├─ damageToCell: 15 (DPS when leak)
├─ plasmaReward: 20 (currency on death)
├─ isSplitter: true/false (SwarmVirus?)
├─ splitChildData: EnemyDataSO (untuk mini virus)
└─ isBoss: true/false (Boss Omega?)
```

**Contoh File Names:**
```
Common Virus.asset
Armored Virus.asset
Swarm Virus.asset
Mini Virus.asset (child dari Swarm)
Boss Omega.asset
```

---

### WaveDataSO (Wave Configuration)
**Apa itu:** Data konfigurasi untuk 1 gelombang/wave, berisi spawn groups

**File Location:** `Assets/Data/WaveData/` (atau folder kamu)

**Struktur:**
```
WaveDataSO
├─ waveNumber: 1
├─ waveName: "Wave 1 - Common Virus Rush"
├─ spawnGroups: List<EnemySpawnGroup>
│  ├─ [0] CommonVirus × 5, interval 0.5s, delay 1.0s
│  ├─ [1] ArmoredVirus × 2, interval 1.0s, delay 3.0s
│  └─ [2] SwarmVirus × 1, interval 0.5s, delay 5.0s
└─ completionBonusPlasma: 50
```

**Contoh File Names:**
```
Wave 1 - Early Game.asset
Wave 2 - Armor Challenge.asset
Wave 3 - Swarm Invasion.asset
```

---

## 🎯 STEP-BY-STEP SETUP

### STEP 1: Pastikan Enemy Data Assets Exist

Di Unity, check apakah sudah ada enemy data files:

1. **Navigate ke folder data**
   - Di Project panel, cari folder `Assets/Data/` atau `Assets/ScriptableObjects/`
   - Lihat subfolder `VirusData/` atau `EnemyData/`

2. **List enemy data yang harus ada:**
   ```
   ✅ Common Virus.asset
   ✅ Armored Virus.asset
   ✅ Swarm Virus.asset
   ✅ Mini Virus.asset
   ✅ Boss Omega.asset
   ```

3. **Jika belum ada, buat:**
   - Right-click di folder data
   - **Create** → **Micro-Defense** → **Enemy Data**
   - Beri nama sesuai enemy type
   - Configure setiap field di Inspector

**Contoh Common Virus.asset Setup:**
```
Enemy Type: CommonVirus
Enemy Name: "Common Virus"
Enemy Prefab: [Drag Common Virus.prefab]
Base Health: 50
Move Speed: 2.0
Flat Armor: 0
Damage To Cell: 10
Plasma Reward: 20
Is Splitter: false
Is Boss: false
```

---

### STEP 2: Buat Minimal 1 WaveDataSO

1. **Create WaveDataSO file**
   - Di Project, right-click di folder `Assets/Data/`
   - **Create** → **Micro-Defense** → **Wave Data**
   - Beri nama: `Wave 1 - Easy.asset`

2. **Open asset di Inspector**
   - Double-click `Wave 1 - Easy.asset` (atau select + lihat Inspector)

3. **Configure WaveDataSO fields:**
   ```
   Wave Number: 1
   Wave Name: "Wave 1 - Easy Intro"
   Completion Bonus Plasma: 50
   Spawn Groups: 1 (set size)
   ```

4. **Configure Spawn Group [0]:**
   ```
   Element 0:
   ├─ Enemy Data: [Drag Common Virus.asset]
   ├─ Count: 5
   ├─ Spawn Interval: 0.5 (detik antar spawn)
   └─ Initial Delay: 1.0 (detik tunggu sebelum spawn group ini mulai)
   ```

**Visual Example:**
```
Wave 1 - Easy Inspector:
┌─────────────────────────────────────┐
│ Wave Number: 1                      │
│ Wave Name: Wave 1 - Easy Intro      │
│ Completion Bonus Plasma: 50         │
│ Spawn Groups:                       │
│  Size: 1                            │
│  [0] Element 0                      │
│      Enemy Data: Common Virus       │
│      Count: 5                       │
│      Spawn Interval: 0.5            │
│      Initial Delay: 1.0             │
└─────────────────────────────────────┘
```

---

### STEP 3: Assign WaveDataSO ke WaveManager

1. **Navigate ke _Managers GameObject**
   - Di Hierarchy, cari GameObject `_Managers`
   - Select it

2. **Find WaveManager Component**
   - Di Inspector, cari komponen `Wave Manager (Script)`

3. **Configure Wave Configuration section:**
   ```
   Wave Manager (Inspector)
   ├─ Wave Configuration
   │  └─ Waves: [size = 1]
   │     ├─ [0]: [Drag Wave 1 - Easy.asset]
   └─ ...
   ```

4. **Configure Pathing System section:**
   ```
   Pathing System (NavMesh vs Waypoints):
   ├─ Use Nav Mesh: ✅ true
   ├─ Spawn Point: [Drag SpawnPoint GameObject]
   ├─ Target Core Base: [Drag TargetCoreBase GameObject]
   └─ Waypoints: (empty - tidak dipakai)
   ```

**Complete WaveManager Setup:**
```
WaveManager Component Inspector:
┌──────────────────────────────────────────────────┐
│ Wave Manager (Script)                            │
├──────────────────────────────────────────────────┤
│ Wave Configuration:                              │
│  Waves: [1 item]                                 │
│   [0]: Wave 1 - Easy.asset                       │
├──────────────────────────────────────────────────┤
│ Pathing System (NavMesh vs Waypoints):           │
│  Use Nav Mesh: ✅ TRUE                           │
│  Target Core Base: TargetCoreBase (Transform)    │
│  Spawn Point: SpawnPoint (Transform)             │
│  Waypoints: (empty list)                         │
├──────────────────────────────────────────────────┤
│ Runtime Status:                                  │
│  Current Wave Index: 0                           │
│  Active Enemy Count: 0                           │
│  Is Wave Spawning: false                         │
└──────────────────────────────────────────────────┘
```

---

## 🎮 TESTING: Pastikan Setup Bekerja

### TEST 1: Verify Asset References
```
Checklist:
☐ WaveManager.Waves[0] tidak null
☐ Wave 1 - Easy.asset loaded
☐ Spawn Group [0].enemyData == Common Virus.asset
☐ Common Virus.asset.enemyPrefab == Common Virus.prefab
☐ SpawnPoint assigned
☐ TargetCoreBase assigned
```

### TEST 2: Play & Check Spawn

1. **Play scene** (klik ▶ button)
2. **Console check** - Tidak ada error
3. **Klik "Start Wave"** di UI
4. **Observe:**
   - Virus spawn di SpawnPoint position
   - Virus count = 5
   - Spawn spacing = 0.5 detik antar virus
   - Virus bergerak ke TargetCoreBase

**Expected Console Output:**
```
(kosong atau debug logs, TIDAK ada error)
```

**Expected Hierarchy:**
```
Saat wave spawn:
Hierarchy:
├─ _Managers
├─ SpawnPoint
├─ TargetCoreBase
├─ Common Virus(Clone) #1  ← spawn saat t=1.0s
├─ Common Virus(Clone) #2  ← spawn saat t=1.5s
├─ Common Virus(Clone) #3  ← spawn saat t=2.0s
├─ Common Virus(Clone) #4  ← spawn saat t=2.5s
└─ Common Virus(Clone) #5  ← spawn saat t=3.0s
```

---

## 🔧 COMMON ISSUES & FIXES

### Issue #1: Enemy tidak spawn

**Symptoms:**
- Klik Start Wave, tapi tidak ada virus di scene
- Console: No error

**Checklist:**
```
☐ WaveManager.useNavMesh = true?
☐ WaveManager.Waves list tidak empty?
☐ Wave 1 - Easy.asset.spawnGroups list tidak empty?
☐ enemyData reference valid?
☐ enemyPrefab reference valid?
☐ spawnInterval > 0?
```

**Fixes:**
1. Assign WaveDataSO ke WaveManager.Waves[0]
2. Di WaveDataSO, set Spawn Groups size > 0
3. Assign enemyData ke spawn group
4. Verify enemyPrefab di EnemyDataSO tidak null

---

### Issue #2: Virus spawn tapi tidak bergerak

**Symptoms:**
- Virus ada di SpawnPoint
- Tapi stuck (tidak move ke TargetCoreBase)

**Checklist:**
```
☐ NavMeshPathFollower component ada di prefab?
☐ NavMeshAgent component ada di prefab?
☐ TargetCoreBase assigned ke WaveManager?
☐ TargetCoreBase position valid (di NavMesh area)?
☐ Use Nav Mesh = true?
```

**Fixes:**
1. Verify prefab punya NavMeshAgent + NavMeshPathFollower
2. Assign TargetCoreBase di WaveManager Inspector
3. Cek TargetCoreBase posisi di area biru (NavMesh walkable)

---

### Issue #3: Spawn interval terlalu cepat/lambat

**Symptoms:**
- Virus spawn semua sekaligus (tidak sesuai interval)
- Atau spawn terlalu slow

**Fix:**
- Edit WaveDataSO, setiap spawn group:
  ```
  Spawn Interval: 0.5 (detik)
  Initial Delay: 1.0 (detik mulai spawn)
  ```
  - Interval 0.5 = spawn tiap 0.5 detik
  - Interval 1.0 = spawn tiap 1.0 detik

---

### Issue #4: "This prefab has not been migrated to a prefab instance"

**Symptoms:**
- Console error saat spawn
- Virus tidak spawn

**Fix:**
- Pastikan enemyPrefab valid prefab asset (bukan scene instance)
- Jika perlu, re-save prefab

---

## 📊 WAVE CONFIGURATION EXAMPLES

### Example 1: Simple Early Wave
```
Wave 1 - Easy Intro

Spawn Groups:
├─ [0] Common Virus
│  └─ Count: 5, Interval: 0.5s, Delay: 1.0s
│     (Total spawn time: 1.0 + 0.5*4 = 3.0s)

Completion Bonus: 50 plasma

Difficulty: Easy (slow enemies, low armor)
```

---

### Example 2: Mixed Wave
```
Wave 2 - Armor Challenge

Spawn Groups:
├─ [0] Common Virus
│  └─ Count: 3, Interval: 0.5s, Delay: 1.0s
├─ [1] Armored Virus
│  └─ Count: 2, Interval: 1.0s, Delay: 3.0s
└─ [2] Common Virus
   └─ Count: 3, Interval: 0.5s, Delay: 5.0s

Completion Bonus: 75 plasma

Difficulty: Medium (tanky enemies mixed with fast)
Timeline:
├─ t=1.0-3.0s: Common Virus wave 1 (3 viruses)
├─ t=3.0-5.0s: Armored Virus wave (2 viruses)
└─ t=5.0-7.0s: Common Virus wave 2 (3 viruses)
```

---

### Example 3: Boss Wave
```
Wave 3 - Boss Invasion

Spawn Groups:
├─ [0] Minions (Common Virus)
│  └─ Count: 2, Interval: 0.5s, Delay: 1.0s
├─ [1] Boss Omega
│  └─ Count: 1, Interval: 0.0s, Delay: 3.0s
└─ [2] Minions (Armored Virus)
   └─ Count: 2, Interval: 0.5s, Delay: 5.0s

Completion Bonus: 200 plasma

Difficulty: Hard (boss spawns minions every 8-10s)
Timeline:
├─ t=1.0-2.0s: Pre-wave minions
├─ t=3.0s: Boss spawn (starts replication cycle)
├─ t=5.0-6.0s: Additional minions
└─ Throughout: Boss spawns random minions
```

---

## 🚀 CARA BUAT MULTIPLE WAVES

Jika mau main 3 gelombang, steps:

1. **Buat 3 WaveDataSO files:**
   ```
   Wave 1 - Easy.asset
   Wave 2 - Medium.asset
   Wave 3 - Hard.asset
   ```

2. **Configure masing-masing dengan spawn groups berbeda**

3. **Di WaveManager, set Waves size = 3:**
   ```
   Wave Manager Inspector:
   Waves: [3 items]
   ├─ [0]: Wave 1 - Easy.asset
   ├─ [1]: Wave 2 - Medium.asset
   └─ [2]: Wave 3 - Hard.asset
   ```

4. **Game flow:**
   ```
   Start Wave 1 → Complete → Preparation
   Start Wave 2 → Complete → Preparation
   Start Wave 3 → Complete → VICTORY
   ```

---

## 📝 SPAWNING MECHANICS (Technical Detail)

### Spawn Coroutine Flow
```csharp
WaveManager.SpawnWaveRoutine(waveData):
├─ isWaveSpawning = true
├─ gameState = WaveInProgress
└─ Untuk setiap spawnGroup di waveData.spawnGroups:
   ├─ Wait(spawnGroup.initialDelay)
   ├─ Untuk i = 0 sampai spawnGroup.count - 1:
   │  ├─ SpawnEnemy(spawnGroup.enemyData)
   │  │  ├─ Instantiate prefab di spawnPoint.position
   │  │  ├─ Initialize dengan EnemyBase
   │  │  ├─ Register OnEnemyDied callback
   │  │  ├─ Call NavMeshPathFollower.InitializeNavMesh(targetCoreBase)
   │  │  └─ activeEnemyCount++
   │  └─ Wait(spawnGroup.spawnInterval)
   └─ isWaveSpawning = false
└─ Jika activeEnemyCount = 0: CompleteWave()
```

### Contoh Timeline Actual
```
t=0.0s:    StartWave() → SpawnWaveRoutine() mulai
t=1.0s:    Initial delay selesai → Spawn virus #1
t=1.5s:    Spawn interval 0.5s → Spawn virus #2
t=2.0s:    Spawn interval 0.5s → Spawn virus #3
t=2.5s:    Spawn interval 0.5s → Spawn virus #4
t=3.0s:    Spawn interval 0.5s → Spawn virus #5 (selesai spawn)
t=3.0-60s: Gameplay - virus bergerak, tower attack, etc
t=45-60s:  Virus mulai reach target atau mati
t=60s:     activeEnemyCount = 0 → CompleteWave()
           Wave bonus plasma ditambah
           Persiapan Wave 2
```

---

## ✅ FINAL CHECKLIST: READY TO PLAY

Sebelum main, pastikan ALL ini sudah done:

### Asset Files
- [ ] Enemy data assets exist (Common, Armored, Swarm, Mini, Boss)
- [ ] WaveDataSO created dengan spawn groups
- [ ] Enemy prefabs created dengan proper components

### WaveManager Configuration
- [ ] WaveManager.useNavMesh = true
- [ ] WaveManager.Waves list punya reference ke WaveDataSO
- [ ] WaveManager.spawnPoint = SpawnPoint GameObject
- [ ] WaveManager.targetCoreBase = TargetCoreBase GameObject

### Scene Setup
- [ ] SpawnPoint GameObject at spawn location
- [ ] TargetCoreBase GameObject at target location
- [ ] NavMeshSurface baked (area biru visible)
- [ ] _Managers GameObject dengan semua managers

### Prefabs
- [ ] Enemy prefabs dengan NavMeshAgent + NavMeshPathFollower
- [ ] Tower prefabs dengan tower scripts
- [ ] All prefabs punya valid colliders

### UI
- [ ] HUD display connected
- [ ] Start Wave button wired
- [ ] Tower shop buttons wired

**Jika semua ✅, siap PLAY GAME!**

---

## 🎓 TROUBLESHOOTING FLOW CHART

```
Game tidak bisa dimainkan?
├─ Virus tidak spawn?
│  ├─ WaveManager.Waves null? → Assign WaveDataSO
│  ├─ WaveDataSO.spawnGroups empty? → Add spawn groups
│  ├─ enemyData null? → Assign EnemyDataSO
│  └─ enemyPrefab null? → Assign prefab
├─ Virus spawn tapi stuck?
│  ├─ NavMeshAgent component? → Add component
│  ├─ TargetCoreBase assigned? → Assign GameObject
│  └─ NavMesh baked? → Bake NavMesh
├─ Tower placement error?
│  └─ PlacementGridManager error → Check GridTile setup
├─ UI tidak update?
│  └─ HUDController listeners → Check OnEnable subscriptions
└─ Console error?
   └─ Read error message, fix accordingly
```
