# Panduan Step-by-Step Setup Unity 2D Editor - Micro-Defense

Panduan ini disesuaikan khusus untuk **Unity 2D** (Sprite 2D, Tilemap 2D, dan Physics 2D) sesuai GDD v0.1.1.

---

## 📍 TAHAP 1: Setting Kamera & Layer 2D

### A. Setting Kamera 2D
1. Pilih **Main Camera** di Hierarchy.
2. Atur di Inspector:
   - **Projection**: `Orthographic`
   - **Size**: `5.0` (sesuaikan jangkauan view pembuluh darah 2D).

### B. Menyiapkan Physics 2D Layer
1. Klik menu **Layers** di pojok kanan atas Unity Editor $\rightarrow$ **Edit Layers...**
2. Buat User Layer 8 bernama: `PlacementTile`.

---

## 📍 TAHAP 2: Membuat Asset ScriptableObject (Data Games)

Di jendela **Project** Unity, buat folder `Assets/DataAssets/Towers/`, `Assets/DataAssets/Enemies/`, dan `Assets/DataAssets/Waves/`:

### A. Data Sel Imun (Tower)
Klik kanan di `Assets/DataAssets/Towers/` $\rightarrow$ **Create** $\rightarrow$ **Micro-Defense** $\rightarrow$ **Tower Data**:
1. `SO_BCell`: Base Cost `100`, Tier 1 (Cost `0`, Dmg `15`, AtkSpeed `1.8`, Range `4.5`, HP `120`), Tier 2 (Cost `75`, Dmg `25`, AtkSpeed `2.4`, Range `4.5`, HP `120`), Tier 3 (Cost `150`, Dmg `25`, AtkSpeed `2.4`, Range `4.5`, HP `120`, Ability `AgglutinationSlow`, Effect Magnitude `0.3`, Duration `2.0`).
2. `SO_Macrophage`: Base Cost `150`, Tier 1 (Cost `0`, Dmg `12`, AtkSpeed `1.0`, Range `1.0`, HP `1000`), Tier 2 (Cost `120`, Dmg `12`, AtkSpeed `1.0`, Range `1.0`, HP `1800`, Dmg Reduction `%` `15`), Tier 3 (Cost `230`, Dmg `12`, AtkSpeed `1.0`, Range `1.0`, HP `2800`, Dmg Reduction `%` `15`, Ability `ContinuousPhagocytosis`).
3. `SO_TCell`: Base Cost `250`, Tier 1 (Cost `0`, Dmg `65`, AtkSpeed `0.6`, Range `2.5`, HP `350`), Tier 2 (Cost `150`, Dmg `110`, AtkSpeed `0.6`, Range `2.5`, HP `350`, Ability `ArmorShred`, Effect `0.25`, Duration `3.0`), Tier 3 (Cost `300`, Dmg `110`, AtkSpeed `0.6`, Range `2.5`, HP `350`, Ability `ApoptosisBlast`).

### B. Data Virus (Enemy)
Klik kanan di `Assets/DataAssets/Enemies/` $\rightarrow$ **Create** $\rightarrow$ **Micro-Defense** $\rightarrow$ **Enemy Data**:
1. `SO_CommonVirus`: Base Health `100`, Move Speed `2.0`, Flat Armor `0`, Damage To Cell `15`, Plasma Reward `10`.
2. `SO_MutantVirus`: Base Health `60`, Move Speed `3.5`, Flat Armor `0`, Damage To Cell `25`, Plasma Reward `15`.
3. `SO_ArmoredVirus`: Base Health `600`, Move Speed `0.8`, Flat Armor `20`, Damage To Cell `45`, Plasma Reward `50`.
4. `SO_MiniVirus`: Base Health `30`, Move Speed `2.5`, Flat Armor `0`, Damage To Cell `5`, Plasma Reward `2`.
5. `SO_SwarmVirus`: Base Health `120`, Move Speed `2.2`, Flat Armor `0`, Damage To Cell `20`, Plasma Reward `12`, **Is Splitter**: `true`, **Split Child Data**: Tarik `SO_MiniVirus`, **Split Count**: `3`.
6. `SO_BossOmega`: Base Health `3000`, Move Speed `1.2`, Flat Armor `25`, Damage To Cell `80`, Plasma Reward `300`, **Is Boss**: `true`, **Replication Minion Pool**: Isi `SO_CommonVirus` & `SO_MutantVirus`.

---

## 📍 TAHAP 3: Membuat Prefab 2D (Sprites & Colliders 2D)

### A. Prefab Musuh 2D
1. Di **Hierarchy**, klik kanan $\rightarrow$ **2D Object** $\rightarrow$ **Sprites** $\rightarrow$ **Circle** (atau gunakan asset Sprite Virus 2D Anda).
2. Pasang komponen Physics 2D:
   - `CircleCollider2D` (atau `BoxCollider2D`).
   - `EnemyBase` (Assign `EnemyDataSO` masing-masing).
   - `PathFollower`.
   - Khusus Swarm Virus: pasang `SplitterEnemy`.
   - Khusus Boss Omega: pasang `BossEnemy`.
3. Tarik objek ke folder `Assets/Prefabs/Enemies/` untuk menjadikannya **Prefab 2D**.

### B. Prefab Tower 2D
1. Di **Hierarchy**, klik kanan $\rightarrow$ **2D Object** $\rightarrow$ **Sprites** $\rightarrow$ **Square** (atau Sprite Sel Imun B-Cell/Macrophage/T-Cell).
2. Pasang komponen:
   - B-Cell: Attach `BCellTower`, assign `SO_BCell`.
   - Macrophage: Attach `MacrophageTower`, assign `SO_Macrophage`.
   - T-Cell: Attach `TCellTower`, assign `SO_TCell`.
3. Tarik objek ke `Assets/Prefabs/Towers/` untuk menjadikannya **Prefab 2D**.
4. Masukkan Prefab 2D Tower masing-masing ke field `Tower Prefab` pada file ScriptableObject (`SO_BCell`, `SO_Macrophage`, `SO_TCell`).

---

## 📍 TAHAP 4: Hierarchy Manager & Waypoints 2D

1. Buat GameObject Kosong di Hierarchy: `_Managers`.
2. Pasang skrip:
   - `GameManager`
   - `PlasmaManager` (Starting Plasma: `300`)
   - `LeakManager` (Max Leaks: `5`)
   - `WaveManager`
   - `PlacementGridManager` (Set **Placement Tile Layer**: `PlacementTile`)
3. Buat GameObject Kosong: `_Waypoints`.
4. Buat child Transform 2D: `Waypoint_0` (Spawn), `Waypoint_1`, `Waypoint_2`, `Waypoint_3` (Inti Sel). Atur posisi (X, Y) sepanjang pembuluh darah 2D.
5. Pada `_Managers` (`WaveManager`), drag-and-drop `Waypoint_0` s/d `Waypoint_3` ke list **Waypoints** dan set `Spawn Point` = `Waypoint_0`.

---

## 📍 TAHAP 5: Petak Penempatan 2D (Grid Tile 2D)

1. Buat 2D Sprite (`Square`) atau 2D Tilemap untuk lokasi penempatan sel darah putih.
2. Pasang komponen **`BoxCollider2D`** pada petak tersebut.
3. Pasang komponen **`GridTile`**.
4. Ubah Layer objek petak grid menjadi **`PlacementTile`**.

---

## 📍 TAHAP 6: Menyiapkan Canvas UI (Top HUD & Shop 2D)

1. Klik kanan Hierarchy $\rightarrow$ **UI** $\rightarrow$ **Canvas**.
2. **Top HUD Panel**: Attach `HUDController`, drag-and-drop komponen Text (`PlasmaText`, `WaveText`, `LeaksText`).
3. **Bottom Shop Panel**: Attach `TowerShopUI`, drag-and-drop 4 UI Button (`BCellButton`, `MacrophageButton`, `TCellButton`, `StartWaveButton`) serta data `SO_BCell`, `SO_Macrophage`, `SO_TCell`.

---

## 🟢 Play Mode (Uji Coba 2D)

1. Klik **Play** di Unity.
2. Klik tombol **B-Cell** di Shop UI $\rightarrow$ Klik salah satu **Grid Tile 2D** di layar $\rightarrow$ B-Cell 2D akan muncul pada koordinat Z=0.
3. Klik tombol **Start Wave** $\rightarrow$ Virus 2D akan bergerak menyusuri titik waypoint 2D.
