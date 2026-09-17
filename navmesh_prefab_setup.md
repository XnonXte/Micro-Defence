# 🔧 Setup NavMesh untuk Virus Prefab - Panduan Step-by-Step

Panduan ini menjelaskan cara menambahkan NavMeshAgent dan NavMeshPathFollower ke setiap virus prefab di Unity Editor.

---

## 📋 Prefab yang Perlu Diupdate

- ✅ Common Virus
- ✅ Mutant Virus
- ✅ Armored Virus
- ✅ Swarm Virus

---

## 🎯 Langkah Setup untuk Setiap Prefab

### **Step 1: Buka Prefab di Editor**

1. Di **Project** panel, navigasi ke `Assets/Prefabs/`
2. Double-click prefab virus (misalnya **Common Virus.prefab**)
3. Prefab akan terbuka dalam Prefab Edit Mode

### **Step 2: Tambah NavMeshAgent Component**

1. Di **Hierarchy**, klik GameObject virus (misal "Common Virus")
2. Di **Inspector**, klik **Add Component**
3. Cari dan pilih **NavMeshAgent**
4. Di Inspector, set:
   - **Update Rotation**: `OFF` (unchecked)
   - **Update Up Axis**: `OFF` (unchecked)
   - **Radius**: `0.5` (atau sesuai ukuran virus)
   - **Speed**: Biarkan default (akan di-sync dari `EnemyBase.currentSpeed`)

### **Step 3: Ganti PathFollower dengan NavMeshPathFollower**

1. Di **Inspector**, cari komponen **PathFollower** (MonoBehaviour)
2. Klik tanda **⋮** (three dots) di top-right component → **Remove Component**
3. Klik **Add Component** → Cari **NavMeshPathFollower**
4. Pilih `NavMeshPathFollower` dari list

### **Step 4: Verifikasi Component Order**

Urutan komponen di Inspector seharusnya:
1. Transform
2. SpriteRenderer
3. CircleCollider2D
4. **NavMeshAgent** ← (BARU)
5. EnemyBase
6. **NavMeshPathFollower** ← (GANTI dari PathFollower)

### **Step 5: Save Prefab**

1. Klik **Save** di top-left Prefab Edit Mode
2. Atau tekan `Ctrl+S`
3. Close Prefab Edit Mode (klik back arrow atau double-click scene di Hierarchy)

---

## ✅ Verifikasi Setup

Setelah setup semua 4 prefab, cek di scene:

1. Buat temporary empty scene untuk test
2. Drag salah satu virus prefab ke scene
3. Di Inspector, verifikasi:
   - ✅ NavMeshAgent ada
   - ✅ NavMeshPathFollower ada
   - ✅ PathFollower **hilang**
4. Hapus dari scene setelah verifikasi

---

## 🚀 Langkah Selanjutnya

Setelah semua prefab diupdate:

1. **Setup SpawnPoint & TargetCoreBase** di scene
   - SpawnPoint: GameObject kosong di titik spawn virus
   - TargetCoreBase: GameObject kosong di titik target (core cell)

2. **Configure WaveManager**
   - Set `Use Nav Mesh = true`
   - Assign SpawnPoint ke slot `Spawn Point`
   - Assign TargetCoreBase ke slot `Target Core Base`

3. **Test Wave Spawning**
   - Play scene
   - Klik Start Wave button
   - Virus seharusnya bergerak dari SpawnPoint menuju TargetCoreBase mengikuti NavMesh path

---

## 🐛 Troubleshooting

| Masalah | Solusi |
|---------|--------|
| Virus tidak bergerak | NavMeshSurface belum di-bake, atau virus diluar NavMesh area |
| Virus stuck di satu tempat | Cek apakah NavMesh terbentuk di path area (warna biru) |
| NavMeshAgent error di console | Pastikan Update Rotation & Update Up Axis = OFF |
| Collision dengan tower | Tower harus ada NavMeshObstacle dengan Carve = true |

---

## 📝 Catatan

- Setup ini hanya butuh dilakukan 1x per prefab
- Pastikan AI Navigation package sudah terinstall (`com.unity.ai.navigation`)
- NavMesh hanya untuk 2D path-following, tidak untuk combat detection
