# Omega Virus Boss - Setup Guide

Omega Virus adalah boss enemy yang dapat spawn virus minion (Common, Swarm, Armored, Mutant) secara random di sekitarnya dalam area NavMesh.

---

## 📋 Fitur Omega Virus

### 1. **Spawn System**
- Spawn minion virus secara periodik (interval adjustable)
- Spawn minion saat HP turun ke threshold tertentu (75%, 50%, 25%)
- Spawn extra minion saat mati (death spawn)
- Jenis virus yang di-spawn: **RANDOM** dari list
- Posisi spawn: **Random dalam radius** di sekitar Omega (dalam NavMesh)

### 2. **Configurable Settings**
- Jumlah minion per spawn (adjustable)
- Interval spawn (detik)
- Radius spawn area
- Health thresholds untuk trigger spawn
- List virus types yang bisa di-spawn

---

## 🛠️ Setup Steps

### Step 1: Create Omega Virus Prefab

1. **Create Empty GameObject** di Hierarchy
   - Nama: `Omega Virus`

2. **Add Components:**
   - `SpriteRenderer` - Untuk visual boss
   - `CircleCollider2D` - Untuk collision
   - `NavMeshAgent` - Untuk movement
   - `OmegaVirus` script (extends EnemyBase)
   - `NavMeshPathFollower` script

3. **Setup SpriteRenderer:**
   ```
   Sprite: [Pilih sprite untuk Omega Boss - seharusnya lebih besar]
   Color: [Bisa pakai warna khusus untuk boss, misal merah/ungu gelap]
   Sorting Layer: Default
   Order in Layer: 10
   ```

4. **Setup CircleCollider2D:**
   ```
   Radius: 0.5 (sesuaikan dengan sprite size)
   Is Trigger: FALSE
   ```

5. **Setup NavMeshAgent:**
   ```
   Agent Type: Humanoid
   Base Offset: 0.5
   Speed: 2.5 (lebih lambat dari minion karena boss)
   Angular Speed: 120
   Acceleration: 8
   Stopping Distance: 1
   Auto Braking: TRUE
   Radius: 1 (lebih besar dari minion)
   Height: 1
   Obstacle Avoidance Type: No Obstacle Avoidance
   ```

---

### Step 2: Create Omega Virus Data (ScriptableObject)

1. **Buat EnemyDataSO:**
   - Klik kanan di `Assets/Resources/Data/EnemyData/`
   - Create → ScriptableObjects → EnemyDataSO
   - Nama: `OmegaVirusData`

2. **Configure Data:**
   ```
   Enemy Name: Omega Virus
   Enemy Prefab: [Drag Omega Virus prefab]
   
   Base Stats:
   - Base Health: 500 (Boss HP tinggi!)
   - Move Speed: 2.5 (Lebih lambat)
   - Flat Armor: 10 (Boss punya armor)
   - Damage To Cell: 10 (Damage besar ke player)
   
   Rewards:
   - Plasma Reward: 100 (Reward besar untuk boss)
   ```

---

### Step 3: Configure OmegaVirus Component

Di Inspector Omega Virus prefab, set component `OmegaVirus`:

```yaml
Omega Boss Settings:
  Enable Spawning: ✓ TRUE

Spawn Configuration:
  Minions Per Spawn: 3
    # Jumlah minion yang spawn setiap interval
  
  Spawn Interval: 5
    # Spawn setiap 5 detik
  
  Spawn Radius: 3
    # Spawn dalam radius 3 meter dari Omega
  
  Health Thresholds: [0.75, 0.5, 0.25]
    # Spawn extra saat HP 75%, 50%, 25%

Minion Types:
  Size: 4
  Element 0: CommonVirusData
  Element 1: SwarmVirusData
  Element 2: ArmoredVirusData
  Element 3: MutantVirusData
    # List virus yang bisa di-spawn (random)

Spawn Visual Effect:
  Spawn Effect Prefab: [Optional - particle effect saat spawn]
  Spawn Effect Duration: 0.5

Debug:
  Show Debug Logs: ✓ TRUE (untuk testing)
```

---

### Step 4: Setup NavMeshPathFollower

```yaml
Queue / Spacing Configuration:
  Minimum Spacing: 0.7
  Spacing Check Radius: 1.0
  Enemy Layer: Enemy
  Stop Distance From Blocker: 0.5
```

---

### Step 5: Setup EnemyBase

```yaml
Enemy Configuration:
  Enemy Data: OmegaVirusData

Hit Effect:
  Hit Flash Duration: 0.15
  Hit Flash Color: White (255, 255, 255, 255)
  Search In Children: ✓ TRUE
```

---

## 🎮 Cara Kerja

### Spawn Behavior:

1. **Interval Spawn** (Periodik)
   - Setiap X detik (default 5s), spawn Y minion (default 3)
   - Jenis virus: **RANDOM** dari Minion Types list
   - Posisi: **RANDOM** dalam Spawn Radius

2. **Health Threshold Spawn** (Saat HP turun)
   - Saat HP ≤ 75%: Spawn 5 minion (bonus +2)
   - Saat HP ≤ 50%: Spawn 5 minion
   - Saat HP ≤ 25%: Spawn 5 minion
   - Hanya trigger **SEKALI** per threshold

3. **Death Spawn** (Saat mati)
   - Spawn 6 minion (double normal amount)
   - Final wave sebelum boss mati

### Minion Pathing:
- Minion yang di-spawn akan **follow path yang sama** dengan Omega
- Otomatis copy waypoints dan target dari boss
- Minion jalan ke target core base yang sama

---

## 🎨 Visual Recommendations

### Boss Appearance:
```
Size: 1.5x - 2x lebih besar dari virus normal
Color: Merah gelap / Ungu gelap / Hitam
Glow Effect: Tambahkan glow sprite untuk efek boss
Animation: Idle animation yang lebih dinamis
```

### Health Bar:
- Boss seharusnya punya health bar yang lebih prominent
- Bisa pakai `VerticalHealthBar` atau buat custom boss health bar

---

## 🧪 Testing

### Test 1: Basic Spawn
1. Place Omega di wave
2. Jalankan game
3. Lihat apakah spawn minion setiap 5 detik
4. Check Console logs: `[OmegaVirus] Spawning X minions`

### Test 2: Health Threshold
1. Damage Omega sampai HP 75%
2. Lihat apakah spawn extra minion
3. Check Console: `Health threshold 75% triggered!`
4. Ulangi untuk 50% dan 25%

### Test 3: Death Spawn
1. Kill Omega
2. Lihat apakah spawn banyak minion sebelum mati
3. Check Console: `Boss defeated! Spawning final wave`

### Test 4: NavMesh Position
1. Lihat posisi spawn minion
2. Pastikan semua spawn di NavMesh (tidak di dinding/obstacle)
3. Jika ada yang spawn di luar NavMesh, perbesar Spawn Radius

---

## ⚙️ Tuning Tips

### Untuk Boss Lebih Mudah:
```
Minions Per Spawn: 2
Spawn Interval: 7
Base Health: 300
Health Thresholds: [0.5, 0.25]  # Hanya 2 threshold
```

### Untuk Boss Lebih Sulit:
```
Minions Per Spawn: 5
Spawn Interval: 3
Base Health: 1000
Health Thresholds: [0.9, 0.75, 0.5, 0.25]  # 4 threshold
Spawn Radius: 5  # Area spawn lebih luas
```

### Untuk Boss Final Level:
```
Minions Per Spawn: 8
Spawn Interval: 2
Base Health: 1500
Flat Armor: 20
Health Thresholds: [0.8, 0.6, 0.4, 0.2]
Minion Types: Hanya Armored & Mutant (virus kuat)
```

---

## 🐛 Troubleshooting

### Minion tidak spawn:
- Check `Enable Spawning` = TRUE
- Check `Minion Types` list tidak kosong
- Check Console untuk error
- Check NavMesh ada di area spawn

### Minion spawn di tempat aneh:
- Perbesar `Spawn Radius` (misal dari 3 ke 5)
- Check NavMesh coverage di area Omega jalan

### Spawn terlalu sering/jarang:
- Adjust `Spawn Interval` (detik)
- Adjust `Minions Per Spawn` (jumlah)

### Boss terlalu kuat/lemah:
- Adjust `Base Health` di EnemyDataSO
- Adjust `Minions Per Spawn`
- Adjust `Health Thresholds` (lebih sedikit = lebih mudah)

---

## 📝 Script Reference

**File:** `Assets/Scripts/Enemies/OmegaVirus.cs`

**Key Methods:**
- `SpawnMinions(int count)` - Spawn X minion
- `CheckHealthThresholdSpawning()` - Check HP threshold
- `TriggerManualSpawn(int count)` - Manual spawn dari luar
- `GetRandomNavMeshPosition()` - Find random valid spawn position

**Extends:** `EnemyBase`

**Public Methods:**
```csharp
public void TriggerManualSpawn(int count = -1)
// Trigger spawn manual, bisa dipanggil dari event/script lain
```

---

## 🎯 Usage in WaveDataSO

```yaml
Wave 5 (Boss Wave):
  Path Spawn Groups:
    - Enemy Data: OmegaVirusData
      Path Index: 0
      Count: 1  # Hanya 1 boss!
      Initial Delay: 0
      Spawn Interval: 0
```

**Recommended:** Boss sebaiknya spawn **sendirian** di wave terpisah atau di akhir wave sebagai mini-boss.

---

## 🌟 Enhancement Ideas

1. **Boss Phases:**
   - Phase 1 (100%-75%): Spawn Common & Swarm
   - Phase 2 (75%-25%): Spawn Armored & Mutant
   - Phase 3 (<25%): Spawn semua jenis

2. **Special Abilities:**
   - AOE damage saat spawn minion
   - Speed boost untuk minions yang di-spawn
   - Shield barrier yang harus dihancurkan dulu

3. **Visual Effects:**
   - Particle effect saat spawn
   - Screen shake saat spawn banyak minion
   - Color change tiap phase (HP threshold)

---

**Selamat! Omega Virus Boss sudah siap!** 🎉

Spawn minions secara dinamis, adjustable, dan random untuk experience yang berbeda tiap game!
