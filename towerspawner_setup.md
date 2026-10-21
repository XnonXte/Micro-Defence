# 🏗️ SETUP TOWERSPAWNER - AUTO SPAWN TOWER

Panduan setup TowerSpawner script untuk auto-spawn 3 tower (B-Cell, T-Cell, Macrophage) di scene saat game start.

---

## 📋 APA ITU TOWERSPAWNER?

TowerSpawner adalah helper script yang auto-spawn tower di posisi tertentu untuk testing. Daripada harus manual place tower pakai placement grid, script ini spawn mereka langsung di Start.

---

## 🎯 LANGKAH SETUP

### STEP 1: Add TowerSpawner Component ke _Managers

1. Di Hierarchy, select **`_Managers`** GameObject
2. Di Inspector, **Add Component** → cari `TowerSpawner`
3. Component akan ditambahkan

### STEP 2: Assign Tower Data

Di TowerSpawner component, Inspector akan show:

```
Tower Data:
├─ B Cell Data: [DRAG B Cell.asset]
├─ T Cell Data: [DRAG T Cell.asset]
└─ Macrophage Data: [DRAG Macrophage cell.asset]
```

Drag dari `Assets/Data/TowerData/`:
- `B Cell.asset` → B Cell Data field
- `T Cell.asset` → T Cell Data field
- `Macrophage cell.asset` → Macrophage Data field

### STEP 3: Configure Tower Positions (Optional)

Di TowerSpawner component, dapat adjust posisi spawn:

```
Tower Positions:
├─ B Cell Position: (-2, 0, 0)      ← B-Cell spawn di X=-2
├─ T Cell Position: (0, 0, 0)       ← T-Cell spawn di X=0
└─ Macrophage Position: (2, 0, 0)   ← Macrophage spawn di X=2
```

Ubah nilai X untuk adjust horizontal positioning. Y dan Z biasanya (0, 0).

### STEP 4: Enable Auto Spawn

Di TowerSpawner component:

```
Auto Spawn on Start: ✅ TRUE (checked)
```

Pastikan checkmark ada. Jika unchecked, tower tidak akan spawn.

---

## 🎮 TEST SETUP

1. **Play game** (▶ button)

2. **Console** - Harus ada logs:
   ```
   [TowerSpawner] Auto-spawning towers...
   [TowerSpawner] Spawning test towers
   [TowerSpawner] Spawning B-Cell Tower at (-2, 0, 0)
   [TowerSpawner] B-Cell Tower spawned successfully
   [TowerSpawner] Spawning T-Cell Tower at (0, 0, 0)
   [TowerSpawner] T-Cell Tower spawned successfully
   [TowerSpawner] Spawning Macrophage Tower at (2, 0, 0)
   [TowerSpawner] Macrophage Tower spawned successfully
   ```

3. **Hierarchy** - Harus ada 3 tower muncul:
   ```
   B-Cell Tower
   T-Cell Tower
   Macrophage Tower
   ```

4. **Scene View** - 3 colored squares seharusnya visible:
   - Cyan square: B-Cell (ranged, slow)
   - Yellow square: T-Cell (AOE, armor shred)
   - Green square: Macrophage (blocker)

5. **Gameplay** - Saat virus spawn dan muncul:
   - Tower harus detect virus di range
   - Tower harus attack (apply damage)
   - Virus health harus berkurang
   - Console harus show attack logs

---

## ✅ VERIFY TOWER ATTACKS

Jika tower sudah spawn, check apakah tower bisa attack:

### Visual Check:
- [ ] Tower visible di scene (colored square)
- [ ] Virus spawn di scene
- [ ] Virus memasuki tower range (distance)

### Debug Check (Console):
```
Cari logs yang menunjukkan attack:
[BCellTower] Attack target: virus_name
[TCellTower] AOE attack: 3 enemies hit
[MacrophageTower] Blocking enemy: virus_name
```

Jika tidak ada attack logs, berarti:
- Tower range terlalu kecil (virus tidak dalam range)
- Tower attack speed = 0 (stats problem)
- Tower data tidak valid

---

## 🐛 TROUBLESHOOTING

| Problem | Solution |
|---------|----------|
| Tower tidak spawn | Check console error, verify tower data assigned |
| Tower spawn tapi tidak attack | Check tower range (di TowerDataSO tier data), increase range value |
| Virus visible tapi tower tidak detect | Increase tower range atau adjust tower position ke spawn area |
| Console: "tierData missing or no tier data" | Tower asset tidak punya tier configuration di TowerDataSO |
| Tower spawn di posisi aneh | Adjust X/Y/Z values di Tower Positions inspector fields |

---

## 🔧 ADVANCED: CUSTOMIZE TOWER POSITIONS

Jika mau adjust di mana tower spawn, edit di Inspector:

```
Tower Positions:
├─ B Cell Position: X -2 → ubah jadi -3, -4, dst
├─ T Cell Position: X 0  → ubah jadi -1, 1, dst
└─ Macrophage Position: X 2 → ubah jadi 3, 4, dst
```

Y-value untuk vertical position (jangan ubah jika spawn di flat area).

---

## 📊 EXPECTED BEHAVIOR AFTER SETUP

### Start Game:
```
t=0s:   Game start
├─ Console: WaveManager auto-spawn first wave
├─ Console: TowerSpawner auto-spawn 3 towers
├─ Hierarchy: 3 towers + 5 virus muncul
└─ Scene: Colored squares (towers) + green circles (virus)

t=1-3s: Virus bergerak ke TargetCoreBase
├─ Tower detect virus dalam range
├─ Tower attack per 0.5-1 second (depends on attack speed)
├─ Virus health berkurang
└─ Console: Attack logs

t=5-10s: Virus mati atau reach target
├─ Jika mati: Plasma +20
├─ Jika reach: Leak +1
└─ Wave complete jika semua virus dead
```

---

## ✅ FULL GAMEPLAY FLOW SAAT INI

```
1. Play → WaveManager.Start() → Auto-spawn wave
2. WaveManager.Start() → TowerSpawner.Start() → Auto-spawn towers
3. Virus spawn di SpawnPoint
4. Tower detect virus dalam range
5. Tower attack setiap interval
6. Virus take damage, health berkurang
7. Virus mati atau reach TargetCoreBase
8. Wave complete → bonus plasma

HASIL: ✅ Virus spawn, attack, mati ✅
```

---

## 📝 NEXT STEPS (Setelah Tower Attack Berfungsi)

1. **Verify all tower types work:**
   - B-Cell: Normal damage + slow effect
   - T-Cell: AOE damage + armor shred
   - Macrophage: Blocking + tank damage

2. **Balance tower stats:**
   - Adjust damage, attack speed, range di TowerDataSO tiers
   - Test different tower configurations

3. **Add UI Canvas:**
   - HUD untuk display Plasma, Wave, Leaks
   - Shop buttons untuk manual tower placement

4. **Complete game loop:**
   - Multiple waves
   - Victory/Defeat conditions
   - Score tracking
