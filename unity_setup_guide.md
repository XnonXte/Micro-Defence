# Panduan Master Setup & Arsitektur Projek Unity 2D - Micro-Defense

Dokumen ini berisi panduan teknis paling baru dan super mendetail untuk membangun game **Micro-Defense: White Blood Cell Strategy** di Unity 2D (berdasarkan GDD v0.1.1).

---

## 🛠️ Ringkasan Arsitektur Kode yang Telah Dibuat

Proyek ini telah dilengkapi dengan arsitektur *data-driven* C# modular di `Assets/Scripts/`:

| Folder Script | Komponen Utama | Fungsi dalam Game |
| :--- | :--- | :--- |
| **`Data/`** | `TowerDataSO`, `EnemyDataSO`, `WaveDataSO` | Menyimpan atribut tower, statistik virus, & komposisi wave berbasis ScriptableObject. |
| **`Core/`** | `GameManager`, `PlasmaManager`, `LeakManager`, `WaveManager` | Mengatur game loop, ekonomi Plasma, kebocoran patogen, & spawner wave. |
| **`Towers/`** | `TowerBase`, `BCellTower`, `MacrophageTower`, `TCellTower` | Logika serangan B-Cell (*Slow*), Macrophage (*Block/Cleave/Regen*), & T-Cell (*AOE/Armor Shred/True Dmg*). |
| **`Enemies/`** | `EnemyBase`, `PathFollower`, `NavMeshPathFollower`, `SplitterEnemy`, `BossEnemy` | Pergerakan virus (Waypoint / NavMesh 2D), kalkulasi Flat Armor, efek *Slow/Shred*, & pembelahan sel. |
| **`Grid/`** | `PlacementGridManager`, `GridTile` | Raycasting 2D & validasi penempatan sel imun di atas petak grid. |
| **`UI/`** | `HUDController`, `TowerShopUI` | Menghubungkan tampilan Plasma, Wave Status, Leak Counter, dan tombol pembelian Tower. |

---

## 📍 TAHAP 1: Setting Kamera & Physics Layer 2D

1. **Main Camera Setup**:
   - Pilih **Main Camera** di Hierarchy.
   - Set **Projection**: `Orthographic`.
   - Set **Size**: `5.0` (sesuaikan dengan bidang pandang arena pembuluh darah).
2. **User Layer Placement**:
   - Klik menu **Layers** di pojok kanan atas Unity Editor $\rightarrow$ **Edit Layers...**
   - Di User Layer 8, isi nama: **`PlacementTile`**.

---

## 📍 TAHAP 2: Membuat Data ScriptableObject

Buat folder `Assets/DataAssets/Towers/`, `Assets/DataAssets/Enemies/`, dan `Assets/DataAssets/Waves/`:

### A. Data Sel Imun (Towers)
Klik kanan di `Assets/DataAssets/Towers/` $\rightarrow$ **Create** $\rightarrow$ **Micro-Defense** $\rightarrow$ **Tower Data**:
1. **`SO_BCell`**: Base Cost `100` | Type `BCell`
   - *Tier 1*: Cost `0`, Dmg `15`, AtkSpeed `1.8`, Range `4.5`, HP `120`.
   - *Tier 2*: Cost `75`, Dmg `25`, AtkSpeed `2.4`, Range `4.5`, HP `120`.
   - *Tier 3*: Cost `150`, Dmg `25`, AtkSpeed `2.4`, Range `4.5`, HP `120`, Ability `AgglutinationSlow` (Magnitude `0.3`, Duration `2.0`).
2. **`SO_Macrophage`**: Base Cost `150` | Type `Macrophage`
   - *Tier 1*: Cost `0`, Dmg `12`, AtkSpeed `1.0`, Range `1.0`, HP `1000`.
   - *Tier 2*: Cost `120`, Dmg `12`, AtkSpeed `1.0`, Range `1.0`, HP `1800`, Dmg Reduction `%` `15`.
   - *Tier 3*: Cost `230`, Dmg `12`, AtkSpeed `1.0`, Range `1.0`, HP `2800`, Dmg Reduction `%` `15`, Ability `ContinuousPhagocytosis`.
3. **`SO_TCell`**: Base Cost `250` | Type `TCell`
   - *Tier 1*: Cost `0`, Dmg `65`, AtkSpeed `0.6`, Range `2.5`, HP `350`.
   - *Tier 2*: Cost `150`, Dmg `110`, AtkSpeed `0.6`, Range `2.5`, HP `350`, Ability `ArmorShred` (Effect `0.25`, Duration `3.0`).
   - *Tier 3*: Cost `300`, Dmg `110`, AtkSpeed `0.6`, Range `2.5`, HP `350`, Ability `ApoptosisBlast`.

### B. Data Virus (Enemies)
Klik kanan di `Assets/DataAssets/Enemies/` $\rightarrow$ **Create** $\rightarrow$ **Micro-Defense** $\rightarrow$ **Enemy Data**:
1. `SO_CommonVirus`: Base Health `100`, Move Speed `2.0`, Flat Armor `0`, Damage To Cell `15`, Plasma Reward `10`.
2. `SO_MutantVirus`: Base Health `60`, Move Speed `3.5`, Flat Armor `0`, Damage To Cell `25`, Plasma Reward `15`.
3. `SO_ArmoredVirus`: Base Health `600`, Move Speed `0.8`, Flat Armor `20`, Damage To Cell `45`, Plasma Reward `50`.
4. `SO_MiniVirus`: Base Health `30`, Move Speed `2.5`, Flat Armor `0`, Damage To Cell `5`, Plasma Reward `2`.
5. `SO_SwarmVirus`: Base Health `120`, Move Speed `2.2`, Flat Armor `0`, Damage To Cell `20`, Plasma Reward `12`, **Is Splitter**: `true`, **Split Child Data**: `SO_MiniVirus`, **Split Count**: `3`.
6. `SO_BossOmega`: Base Health `3000`, Move Speed `1.2`, Flat Armor `25`, Damage To Cell `80`, Plasma Reward `300`, **Is Boss**: `true`, **Replication Minion Pool**: `SO_CommonVirus` & `SO_MutantVirus`.

### C. Data Wave (Gelombang)
Di `Assets/DataAssets/Waves/` $\rightarrow$ **Create** $\rightarrow$ **Micro-Defense** $\rightarrow$ **Wave Data** (`SO_Wave1`):
- **Spawn Groups**:
  - Element 0: Enemy `SO_CommonVirus`, Count `10`, Spawn Interval `1.5`, Delay `0`.
  - Element 1: Enemy `SO_MutantVirus`, Count `3`, Spawn Interval `2.0`, Delay `5.0`.
- **Completion Bonus Plasma**: `50`.

---

## 📍 TAHAP 3: Detail Setup Waypoint (Jalur Pergerakan Musuh)

### A. Hirarki Waypoint di Unity Editor:
```text
Hierarchy
├── _Waypoints (Parent Object Kosong - Transform Position X:0, Y:0, Z:0)
│   ├── Waypoint_0 (Child 1: Titik Spawn Virus)   [Posisi X: -8.0, Y:  2.0, Z: 0]
│   ├── Waypoint_1 (Child 2: Belokan Pertama)     [Posisi X: -2.0, Y:  2.0, Z: 0]
│   ├── Waypoint_2 (Child 3: Belokan Kedua)       [Posisi X: -2.0, Y: -3.0, Z: 0]
│   └── Waypoint_3 (Child 4: Inti Sel / Base)     [Posisi X:  8.0, Y: -3.0, Z: 0]
```

### B. Langkah Eksekusi Waypoint:
1. **Buat Parent**: Klik Kanan di Hierarchy $\rightarrow$ **Create Empty** $\rightarrow$ beri nama `_Waypoints`.
2. **Buat Child**: Klik Kanan pada `_Waypoints` $\rightarrow$ **Create Empty Child** $\rightarrow$ beri nama `Waypoint_0`.
3. Geser `Waypoint_0` di Scene View menggunakan Move Tool (`W`) ke titik awal spawn virus.
4. Ulangi untuk `Waypoint_1`, `Waypoint_2`, dan `Waypoint_3` sepanjang alur jalan pembuluh darah.
5. **Ikon Visual (Gizmos)**: Klik `Waypoint_0`, di Inspector klik **ikon kubus berwarna** (kiri atas) lalu pilih lingkaran merah/biru agar titik waypoint terlihat jelas di Scene View.
6. **Assign ke WaveManager**: Klik GameObject `_Managers`, cari `WaveManager`, ubah Size `Waypoints` menjadi `4`, lalu tarik `Waypoint_0` s/d `Waypoint_3` dari Hierarchy ke slot-slot Element. Set `Spawn Point` ke `Waypoint_0`.

---

## 📍 TAHAP 4: Alternatif System - NavMesh 2D (AI Pathfinding)

Jika Anda ingin musuh otomatis berjalan memutari tower yang dipasang di jalan:
1. Install package `com.unity.ai.navigation` via Package Manager.
2. Buat GameObject `_NavMeshSurface`, pasang `NavMeshSurface 2D`, lalu klik **Bake**.
3. Pada Prefab Virus: Pasang komponen `NavMeshAgent` (set `Update Rotation = false`, `Update Up Axis = false`) dan skrip [`NavMeshPathFollower.cs`](file:///c:/UnityGame/Micro-Defence/Assets/Scripts/Enemies/NavMeshPathFollower.cs).
4. Pada Prefab Tower: Pasang komponen `NavMeshObstacle` (centang `Carve = true`).

---

## 📍 TAHAP 5: Petak Penempatan 2D (Grid Tile)

1. Buat 2D Sprite (`Square`) untuk petak penempatan tower.
2. Pasang komponen **`BoxCollider2D`** dan skrip **`GridTile`**.
3. Ubah Layer objek menjadi **`PlacementTile`**.
4. Di `_Managers` (`PlacementGridManager`), pastikan field **Placement Tile Layer** memilih layer `PlacementTile`.

---

## 📍 TAHAP 6: Menyiapkan Canvas UI

1. Klik kanan Hierarchy $\rightarrow$ **UI** $\rightarrow$ **Canvas**.
2. **Top HUD Panel**: Pasang skrip `HUDController`, drag-and-drop Text untuk `PlasmaText`, `WaveText`, dan `LeaksText`.
3. **Bottom Shop Panel**: Pasang skrip `TowerShopUI`, drag-and-drop 4 UI Button (`BCellButton`, `MacrophageButton`, `TCellButton`, `StartWaveButton`) serta data `SO_BCell`, `SO_Macrophage`, `SO_TCell`.

---

## 💡 Strategi Kombinasi Sel Imun (GDD Tactic Guide)

- 🔴 **B-Cell (Shooter):** Pasang di area tengah untuk serangan jarak jauh. Upgrade ke Tier 3 (*Agglutination*) untuk memperlambat *Mutant Virus*.
- 🛡️ **Macrophage (Blocker Tank):** Pasang di *Chokepoint* (penyempitan jalan) untuk menahan laju kerumunan virus. Di Tier 3 (*Continuous Phagocytosis*), Macrophage memulihkan 5% HP setiap kali virus kecil mati saat ditahan!
- ⚔️ **T-Cell (AOE / Armor Shred):** Pasang tepat di samping Macrophage. Saat Macrophage menahan kerumunan virus, T-Cell melepaskan tebasan AOE untuk menghancurkan mereka bersamaan dan memotong Flat Armor (*Armor Shred*).
