# 🧪 Testing & Troubleshooting - NavMesh System

Panduan lengkap untuk test NavMesh 2D system dan handle common issues.

---

## 🎮 Test Play Flow

### **Pre-Test Checklist**

Sebelum test, verify semua sudah complete:

- ✅ NavMeshSurface di-bake (area biru visible di path)
- ✅ Semua 4 virus prefab punya NavMeshAgent + NavMeshPathFollower
- ✅ SpawnPoint & TargetCoreBase dibuat di scene
- ✅ WaveManager: Use Nav Mesh = true
- ✅ WaveManager: SpawnPoint & TargetCoreBase assigned
- ✅ Minimal 1 WaveDataSO sudah dibuat dengan enemy data

### **Test Steps**

1. **Play Scene**
   ```
   Klik Play button (▶) di Unity Editor
   ```

2. **Start Wave**
   ```
   Klik tombol "Start Wave" di UI
   Pastikan tidak ada error di Console
   ```

3. **Observe Virus Behavior**
   - Virus spawn di SpawnPoint position
   - Virus bergerak menuju TargetCoreBase
   - Movement smooth mengikuti NavMesh path (biru)
   - Tidak ada jittering atau stuck behavior

4. **Verify End State**
   - Virus mencapai TargetCoreBase
   - `OnReachedEndPath()` trigger → leak counter +1
   - Virus disappear (destroy)
   - Wave status update

5. **Check Console**
   - Tidak ada error/exception
   - NavMeshAgent debug log (jika enabled)

---

## 🐛 Troubleshooting Guide

### **Issue #1: Virus Tidak Spawn**

**Symptoms:**
- Tidak ada virus di scene saat Start Wave
- Wave status tapi virus count = 0

**Root Causes & Fixes:**

| Penyebab | Solusi |
|---------|--------|
| SpawnPoint null di WaveManager | Di Inspector WaveManager, drag SpawnPoint ke slot |
| Enemy prefab reference error | Cek WaveDataSO → spawnGroups → enemyData → prefab |
| WaveDataSO kosong | Buat WaveDataSO dengan minimal 1 spawn group |
| enemyPrefab null | Assign virus prefab ke EnemyDataSO.enemyPrefab |

**Debug Command:**
```csharp
// Tambah ke WaveManager.SpawnEnemy() untuk debug
Debug.Log($"Spawning: {enemyData.name} at {spawnPos}");
```

---

### **Issue #2: Virus Spawn tapi Tidak Bergerak**

**Symptoms:**
- Virus visible di SpawnPoint
- Tapi tidak bergerak/stuck

**Root Causes & Fixes:**

| Penyebab | Solusi |
|---------|--------|
| NavMesh tidak include path area | Cek NavMeshSurface: Collect Objects = All Objects |
| Re-bake NavMesh (Window → AI → NavMesh Baking) | |
| SpawnPoint/TargetCoreBase di luar NavMesh | Pastikan posisi di area biru (walkable) |
| TargetCoreBase null | Di WaveManager, assign TargetCoreBase |
| NavMeshPathFollower.InitializeNavMesh() error | Cek script error di Console |

**Visual Debug:**
```
Di Scene View:
- Tekan Alt untuk toggle NavMesh visibility
- Area biru = walkable
- Area abu-abu/kosong = non-walkable
```

---

### **Issue #3: Virus Keluar dari Path**

**Symptoms:**
- Virus bergerak tapi tidak mengikuti path
- Virus cut corners atau jalan lurus

**Root Causes:**

| Penyebab | Solusi |
|---------|--------|
| NavMeshAgent radius terlalu besar | Reduce radius di NavMeshAgent component |
| Path terlalu sempit | Perlebar area path atau adjust NavMesh settings |
| Collision layer issue | Cek NavMeshAgent layer & collision settings |
| Update Rotation/Up Axis = true | Set both ke false (untuk 2D) |

---

### **Issue #4: NavMeshAgent Error**

**Error Message:**
```
"Given a position that is not on the NavMesh..."
"SetDestination only works if the agent is on the NavMesh..."
```

**Solutions:**

```csharp
// Verify agent & destination di NavMesh
if (!agent.isOnNavMesh)
{
    Debug.LogError("Agent tidak di NavMesh!");
    // Cek spawn position
}

if (!NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 1f, NavMesh.AllAreas))
{
    Debug.LogError("Target position tidak di NavMesh!");
    // Pindahkan TargetCoreBase
}
```

---

### **Issue #5: Memory/Performance Issues**

**Symptoms:**
- Game lag saat spawn banyak virus
- Inconsistent framerate

**Optimization:**

```csharp
// Check active enemy count
Debug.Log($"Active enemies: {WaveManager.Instance.activeEnemyCount}");

// Consider:
// - Object pooling untuk virus
// - Batch NavMesh queries
// - Reduce NavMesh quality untuk large scenes
```

---

## 📊 Performance Baseline

| Metric | Target | Acceptable Range |
|--------|--------|-----------------|
| Virus Spawn Time | < 1ms | < 2ms |
| NavMesh Query | < 0.5ms | < 1ms |
| Movement Update | < 0.1ms per agent | < 0.2ms |
| Memory per Virus | ~50KB | < 100KB |

---

## ✅ Full Test Scenario

```
Game Flow:
1. Scene load → NavMesh visible (area biru)
2. UI ready → "Start Wave" button active
3. Click "Start Wave" → Preparation → WaveInProgress
4. Virus spawn (count: 5 common viruses)
5. Virus #1 → path-find → movement → reach target → leak +1
6. ... (repeat for all 5)
7. All virus dead → Wave complete bonus plasma
8. Game state → Preparation (ready next wave)
9. UI update → Wave counter +1
```

Expected outcome: **SUCCESS** ✅

---

## 📝 Post-Test Checklist

- [ ] Virus spawn di correct position
- [ ] Virus bergerak smooth
- [ ] Path-following natural (tidak jittery)
- [ ] Reach target → trigger callback
- [ ] No console errors
- [ ] Wave cycle complete
- [ ] Ready untuk next wave

---

## 🚀 Next Steps (After Successful Test)

1. **Tower Implementation**
   - Macrophage blocking via `agent.isStopped`
   - B-Cell/T-Cell targeting

2. **Combat Integration**
   - Damage application
   - Status effects (slow, armor shred)

3. **Wave Balancing**
   - Difficulty progression
   - Enemy mix tuning

4. **UI Polish**
   - Visual feedback
   - Audio integration
