# 🎯 Setup SpawnPoint & TargetCoreBase - Panduan

Setup 2 GameObject sederhana untuk menentukan titik spawn virus dan target tujuan.

---

## 📍 Langkah-Langkah

### **Step 1: Buat SpawnPoint**

1. Di **Hierarchy**, right-click → **Create Empty**
2. Rename ke **`SpawnPoint`**
3. Set position sesuai titik spawn virus (misal: `X: -5, Y: 0`)
4. Pastikan posisi ini ada di atas/inside area **NavMesh biru** (walkable area)

### **Step 2: Buat TargetCoreBase**

1. Di **Hierarchy**, right-click → **Create Empty**
2. Rename ke **`TargetCoreBase`**
3. Set position sesuai titik target inti sel (misal: `X: 5, Y: 0`)
4. Pastikan posisi ini juga ada di atas area **NavMesh biru**

### **Step 3: Assign ke WaveManager**

1. Di **Hierarchy**, klik GameObject **`_Managers`**
2. Di **Inspector**, cari komponen **Wave Manager (Script)**
3. Di bagian **Pathing System (NavMesh vs Waypoints)**:
   - Centang **`Use Nav Mesh`** = `true`
   - Drag **SpawnPoint** → slot **`Spawn Point`**
   - Drag **TargetCoreBase** → slot **`Target Core Base`**

---

## ✅ Verifikasi

1. Play scene
2. Klik tombol **Start Wave**
3. Virus seharusnya:
   - Spawn di **SpawnPoint**
   - Bergerak mengikuti **NavMesh path** (biru)
   - Menuju **TargetCoreBase**
   - Trigger `OnReachedEndPath()` saat tiba

---

## 🗺️ Visual Guide

```
      SpawnPoint (virus start)
         ↓
    [========== Path ==========]  ← NavMesh walkable area (biru)
         ↓
      TargetCoreBase (virus end)
```

Pastikan kedua point berada di area biru NavMesh, bukan di area non-walkable.
