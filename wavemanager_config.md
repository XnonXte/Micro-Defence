# ⚙️ Configure WaveManager - NavMesh Setup

Panduan untuk mengkonfigurasi WaveManager agar menggunakan NavMesh system untuk virus pathfinding.

---

## 📋 Prerequisites

Sebelum configure WaveManager, pastikan sudah complete:

- ✅ NavMeshSurface sudah di-bake (NavMesh biru visible)
- ✅ Semua virus prefab punya NavMeshAgent + NavMeshPathFollower
- ✅ SpawnPoint dan TargetCoreBase sudah dibuat di scene

---

## 🎯 Langkah Configure WaveManager

### **Step 1: Buka WaveManager**

1. Di **Hierarchy**, klik GameObject **`_Managers`**
2. Di **Inspector**, cari komponen **Wave Manager (Script)**

### **Step 2: Set Pathing System ke NavMesh**

Di section **Pathing System (NavMesh vs Waypoints)**:

| Field | Value | Keterangan |
|-------|-------|-----------|
| **Use Nav Mesh** | ✅ `true` | Enable NavMesh mode |
| **Target Core Base** | `TargetCoreBase` | Drag GameObject dari Hierarchy |
| **Spawn Point** | `SpawnPoint` | Drag GameObject dari Hierarchy |
| **Waypoints** | (kosong) | Tidak digunakan untuk NavMesh mode |

### **Step 3: Verifikasi Component**

Di Inspector, pastikan:

```
Wave Manager (Script)
├─ Wave Configuration
│  └─ Waves: [list of WaveDataSO]
├─ Pathing System (NavMesh vs Waypoints)
│  ├─ Use Nav Mesh: ✅ TRUE
│  ├─ Target Core Base: TargetCoreBase (Transform)
│  ├─ Waypoints: (empty list)
│  └─ Spawn Point: SpawnPoint (Transform)
└─ Runtime Status
   ├─ Current Wave Index: 0
   ├─ Active Enemy Count: 0
   └─ Is Wave Spawning: false
```

---

## 🔗 Script Logic Reference

**WaveManager.cs** (line 95-102):

```csharp
if (useNavMesh)
{
    NavMeshPathFollower navFollower = enemyObj.GetComponent<NavMeshPathFollower>();
    if (navFollower != null && targetCoreBase != null)
    {
        navFollower.InitializeNavMesh(targetCoreBase);
    }
}
```

Ketika `Use Nav Mesh = true`:
1. WaveManager akan memanggil `InitializeNavMesh(targetCoreBase)` pada setiap virus
2. NavMeshPathFollower akan set destination ke `targetCoreBase.position`
3. NavMeshAgent akan otomatis pathfind menuju target

---

## ✅ Testing Checklist

Setelah configure, test dengan:

```
1. ☐ Play scene
2. ☐ Console tidak ada error terkait NavMesh/NavMeshPathFollower
3. ☐ Klik tombol "Start Wave"
4. ☐ Virus spawn di SpawnPoint
5. ☐ Virus bergerak ke TargetCoreBase
6. ☐ Virus mencapai end → OnReachedEndPath() trigger
7. ☐ Leak counter naik (jika ada)
8. ☐ Wave complete notification muncul
```

---

## 🐛 Common Issues & Fixes

| Error | Penyebab | Solusi |
|-------|---------|--------|
| `Virus tidak spawn` | SpawnPoint = null di WaveManager | Assign SpawnPoint ke slot |
| `Virus stuck di spawn` | TargetCoreBase = null | Assign TargetCoreBase ke slot |
| `NavMeshAgent error` | Use Nav Mesh = false | Centang Use Nav Mesh = true |
| `Virus keluar path` | Virus di luar NavMesh area | Pastikan SpawnPoint & TargetCoreBase di area biru |
| `No path available` | NavMesh tidak connected | Cek apakah path dan tower area terkoneksi di NavMesh |

---

## 📝 Config Summary

```yaml
WaveManager Configuration:
  pathing_mode: "NavMesh"
  spawn_point: "SpawnPoint (GameObject)"
  target: "TargetCoreBase (GameObject)"
  virus_spawning:
    - initialize: "EnemyBase.Initialize(EnemyDataSO)"
    - pathfind: "NavMeshPathFollower.InitializeNavMesh(targetCoreBase)"
    - movement: "NavMeshAgent.SetDestination(target)"
  completion:
    - on_reach_target: "EnemyBase.OnReachedEndPath()"
    - on_all_dead: "WaveManager.CompleteWave()"
    - next_wave: "WaveManager.StartNextWave()"
```
