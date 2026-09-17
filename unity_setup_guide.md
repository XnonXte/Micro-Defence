# 📘 Panduan Master Setup Unity 2D (Pure NavMesh) - Micro-Defense

Dokumen ini adalah panduan teknis paling baru dan 100% fokus pada **System NavMesh 2D AI Pathfinding** (tanpa Waypoints) untuk game **Micro-Defense: White Blood Cell Strategy** (GDD v0.1.1).

---

## 🛑 ATURAN PENEMPATAN TOWER (Placement Rules)

Game ini menggunakan aturan penempatan petak grid yang spesifik dan realistis:

1. **Macrophage (Sel Blocker / Pertahanan)** 🛡️:
   - **HANYA BISA DITARUH DI DALAM JALUR PEMBULUH DARAH (`OnTrackPath`)**.
   - Dipasang di tengah jalan untuk menahan pergerakan virus (`agent.isStopped = true`).
2. **B-Cell & T-Cell (Sel Penyerang / Attackers)** 🏹⚔️:
   - **HANYA BISA DITARUH DI PINGGIR/SISI JALUR (`OffTrackSide`)**.
   - Dipasang di tepi pembuluh darah untuk menembak/menebas tanpa mengganggu rute NavMesh 2D.

---

## 🧭 CARA KERJA & SETUP NAVMESH 2D (PURE AI PATHFINDING)

Dengan sistem **Pure NavMesh 2D**:
- Anda **TIDAK PERLU** membuat titik-titik Waypoint kaku secara manual.
- Anda **HANYA Butuh 2 Objek Point** di scene:
  1. **`SpawnPoint`**: Titik asal tempat virus bermunculan (misal di sebelah kiri arena).
  2. **`TargetCoreBase`**: Titik sasaran inti sel yang ingin dicapai virus (misal di sebelah kanan arena).

---

### 🚀 Step-by-Step Setup NavMesh 2D di Unity Editor:

#### 🔹 Langkah 1: Install Package "AI Navigation"
1. Buka **Window** $\rightarrow$ **Package Manager**.
2. Klik ikon `+` di pojok kiri atas $\rightarrow$ **Add package by name...**
3. Ketik: `com.unity.ai.navigation` lalu klik **Add**.

#### 🔹 Langkah 2: Bake NavMesh Surface 2D pada Jalur Pembuluh Darah
1. Buat GameObject kosong di Hierarchy $\rightarrow$ Beri nama **`_NavMeshSurface`**.
2. Klik `_NavMeshSurface`, tambahkan komponen **`NavMeshSurface`** (bawaan package AI Navigation).
3. Di Inspector:
   - **Collect Objects**: `All Objects` / `Volume`.
   - Pastikan area jalan/pembuluh darah 2D Anda memiliki Collider 2D atau Sprite Renderer.
4. Klik tombol **Bake**. 
   - Area **biru terang** (NavMesh Walkable) akan muncul memetakan alur jalan pembuluh darah 2D di Scene View.

#### 🔹 Langkah 3: Setup pada Prefab Virus (Enemies)
1. Buka Prefab Virus (Common, Mutant, Armored, Swarm, Mini, Boss).
2. Tambahkan komponen **`NavMeshAgent`**:
   - Set **`Update Rotation = false`** (OFF).
   - Set **`Update Up Axis = false`** (OFF).
3. Tambahkan komponen skrip **`NavMeshPathFollower`** ([`NavMeshPathFollower.cs`](file:///c:/UnityGame/Micro-Defence/Assets/Scripts/Enemies/NavMeshPathFollower.cs)).

#### 🔹 Langkah 4: Setup pada Prefab Macrophage (Blocker)
1. Buka Prefab Macrophage.
2. Tambahkan komponen skrip **`MacrophageTower`** ([`MacrophageTower.cs`](file:///c:/UnityGame/Micro-Defence/Assets/Scripts/Towers/MacrophageTower.cs)).
3. *(Opsional)* Tambahkan **`NavMeshObstacle`** (centang `Carve = true`) jika ingin Macrophage melubangi NavMesh dan memaksa virus memutarinya saat ada rute bercabang.

#### 🔹 Langkah 5: Hubungkan di Manager (`_Managers`)
1. Klik GameObject **`_Managers`** di Hierarchy.
2. Pada komponen **Wave Manager (Script)**:
   - Centang **`Use Nav Mesh = true`**.
   - Drag & drop objek `SpawnPoint` ke slot **Spawn Point**.
   - Drag & drop objek `TargetCoreBase` ke slot **Target Core Base**.

---

## 🗺️ TABEL PEMETAAN: Skrip C# -> Dipasang di Mana?

| Nama Skrip C# | File Path Link | Dipasang di GameObject Mana? | Fungsi & Slot Inspector yang Harus Diisi |
| :--- | :--- | :--- | :--- |
| **`PlacementGridManager`** | [`PlacementGridManager.cs`](file:///c:/UnityGame/Micro-Defence/Assets/Scripts/Grid/PlacementGridManager.cs) | GameObject **`_Managers`** | Mengendalikan validasi aturan penempatan & raycast 2D klik mouse. Set **Placement Tile Layer** = `PlacementTile`. |
| **`GridTile`** | [`PlacementGridManager.cs`](file:///c:/UnityGame/Micro-Defence/Assets/Scripts/Grid/PlacementGridManager.cs#L112) | **Setiap Petak Kotak Grid (Tile)** | Menentukan jenis petak (**Tile Type**): `OffTrackSide` (Pinggir) atau `OnTrackPath` (Tengah jalan). |
| **`GameManager`** | [`GameManager.cs`](file:///c:/UnityGame/Micro-Defence/Assets/Scripts/Core/GameManager.cs) | GameObject **`_Managers`** | Mengontrol status game (`Preparation`, `WaveInProgress`, `Defeat`, `Victory`). |
| **`PlasmaManager`** | [`PlasmaManager.cs`](file:///c:/UnityGame/Micro-Defence/Assets/Scripts/Core/PlasmaManager.cs) | GameObject **`_Managers`** | Mengatur uang Plasma. Set **Starting Plasma** = `300`. |
| **`LeakManager`** | [`LeakManager.cs`](file:///c:/UnityGame/Micro-Defence/Assets/Scripts/Core/LeakManager.cs) | GameObject **`_Managers`** | Menghitung batas kebocoran virus. Set **Max Leaks** = `5`. |
| **`WaveManager`** | [`WaveManager.cs`](file:///c:/UnityGame/Micro-Defence/Assets/Scripts/Core/WaveManager.cs) | GameObject **`_Managers`** | Mengatur spawning virus. Centang **Use Nav Mesh = true**, masukkan `SpawnPoint` & `TargetCoreBase`. |
| **`NavMeshPathFollower`** | [`NavMeshPathFollower.cs`](file:///c:/UnityGame/Micro-Defence/Assets/Scripts/Enemies/NavMeshPathFollower.cs) | **Semua Prefab Virus** | Pergerakan musuh berbasis NavMesh 2D AI. Otomatis berhenti saat ditahan Macrophage (`agent.isStopped = true`). |
| **`BCellTower`** | [`BCellTower.cs`](file:///c:/UnityGame/Micro-Defence/Assets/Scripts/Towers/BCellTower.cs) | **Prefab Tower B-Cell** | Dipasang di `OffTrackSide`. Menembak dari pinggir jalan + efek *Slow (Agglutination)*. |
| **`MacrophageTower`** | [`MacrophageTower.cs`](file:///c:/UnityGame/Micro-Defence/Assets/Scripts/Towers/MacrophageTower.cs) | **Prefab Tower Macrophage** | Dipasang di `OnTrackPath`. Menahan pergerakan virus & regenerasi HP (*Phagocytosis*). |
| **`TCellTower`** | [`TCellTower.cs`](file:///c:/UnityGame/Micro-Defence/Assets/Scripts/Towers/TCellTower.cs) | **Prefab Tower T-Cell** | Dipasang di `OffTrackSide`. Menebas AOE & memotong armor (*Armor Shred* & *True Damage*). |
| **`HUDController`** | [`HUDController.cs`](file:///c:/UnityGame/Micro-Defence/Assets/Scripts/UI/HUDController.cs) | UI Panel **Top HUD** | Menampilkan Text Plasma, Wave Status, & Kebocoran. |
| **`TowerShopUI`** | [`TowerShopUI.cs`](file:///c:/UnityGame/Micro-Defence/Assets/Scripts/UI/TowerShopUI.cs) | UI Panel **Bottom Shop** | Menghubungkan Tombol B-Cell, Macrophage, T-Cell, & Start Wave. |

---

## 📍 PETUNJUK MEMBUAT DUA JENIS PETAK GRID (TILES)

Di Unity Editor, pisahkan petak grid 2D menjadi **2 jenis**:

1. **Petak Pinggir Jalan (`OffTrackSide`)**:
   - Pasang skrip `GridTile`, set **Tile Type** = `OffTrackSide`.
   - Hanya **B-Cell** dan **T-Cell** yang bisa dipasang di sini.
2. **Petak Tengah Jalan (`OnTrackPath`)**:
   - Pasang skrip `GridTile`, set **Tile Type** = `OnTrackPath`.
   - Hanya **Macrophage (Blocker)** yang bisa dipasang di sini.
