# Panduan Setup Boss Label untuk Omega Virus

## Overview

Script **BossLabel.cs** sudah dibuat untuk menampilkan teks "BOSS" berwarna merah di atas Omega Virus saat spawn.

## Cara Setup di Unity Editor

### 1. Buka Prefab Omega Virus

1. **Project Window** → Cari `Omega Virus.prefab`
   - Location: `Assets/Omega Virus.prefab`
2. **Double-click** prefab untuk membuka Prefab Mode
3. Atau **Select prefab** → Inspector → Open Prefab

### 2. Tambahkan Boss Label Script

1. **Select root GameObject** dari prefab (Omega Virus)
2. **Inspector → Add Component**
3. Ketik **"Boss Label"**
4. Pilih **Boss Label (Script)**

### 3. Configure Settings di Inspector

**Boss Label Component:**

```
Boss Label (Script)
├── Label Settings
│   ├── Label Text: "BOSS"
│   ├── Label Color: Red (255, 0, 0, 255)
│   ├── Font Size: 24
│   └── Offset: (0, 2, 0)
├── Animation (Optional)
│   ├── Enable Pulse: ✓ (checked)
│   ├── Pulse Speed: 2
│   └── Pulse Scale: 0.1
└── Billboard (Always Face Camera)
    └── Enable Billboard: ✓ (checked)
```

### 4. Sesuaikan Posisi Label (Optional)

**Offset Y** menentukan seberapa tinggi teks di atas boss:
- **Y = 1**: Dekat dengan boss
- **Y = 2**: Default (recommended)
- **Y = 3**: Lebih tinggi

**Untuk adjust:**
1. Di Inspector → Boss Label → **Offset**
2. Ubah nilai Y sesuai kebutuhan
3. Play test untuk lihat hasilnya

### 5. Sesuaikan Visual (Optional)

#### Ukuran Font:
- **Font Size: 18** - Kecil
- **Font Size: 24** - Default (recommended)
- **Font Size: 36** - Besar

#### Warna:
- **Red** (255, 0, 0) - Default merah terang
- **Dark Red** (180, 0, 0) - Merah gelap
- **Orange Red** (255, 69, 0) - Orange kemerahan

#### Animation:
- **Enable Pulse**: Membuat teks bergerak naik-turun (breathing effect)
- **Pulse Speed**: Kecepatan animasi (2 = default)
- **Pulse Scale**: Intensitas gerakan (0.1 = 10% scaling)

### 6. Save Prefab

1. **Ctrl+S** atau File → Save
2. **Exit Prefab Mode** (tombol < di Hierarchy)

## Cara Kerja

### Saat Boss Spawn:

1. ✅ **OmegaVirus prefab di-spawn** oleh WaveManager
2. ✅ **BossLabel script Start()** dipanggil
3. ✅ **Label "BOSS" dibuat** sebagai child GameObject dengan TextMeshPro
4. ✅ **Label muncul** di atas boss dengan warna merah
5. ✅ **Billboard effect** membuat label selalu menghadap camera
6. ✅ **Pulse animation** membuat label bergerak naik-turun

### Features:

✅ **Billboard** - Label selalu menghadap player (tidak ikut rotasi boss)
✅ **Pulse Animation** - Breathing effect untuk menarik perhatian
✅ **Follow Boss** - Label mengikuti posisi boss
✅ **Auto-cleanup** - Label hilang saat boss mati

## Testing

### Test Checklist:

- [ ] Spawn boss di game
- [ ] Teks "BOSS" merah muncul di atas boss
- [ ] Teks menghadap camera (tidak miring)
- [ ] Teks bergerak naik-turun (pulse animation)
- [ ] Teks mengikuti boss saat boss bergerak
- [ ] Teks hilang saat boss mati

### Expected Result:

```
        ╔══════╗
        ║ BOSS ║  ← Teks merah, pulse animation
        ╚══════╝
           🦠      ← Omega Virus sprite
```

## Customization

### Ganti Text "BOSS" ke Text Lain:

**Opsi 1: Di Inspector**
- Boss Label → Label Text: "OMEGA BOSS" (atau text lain)

**Opsi 2: Via Script (Runtime)**
```csharp
BossLabel label = GetComponent<BossLabel>();
label.SetLabelText("OMEGA VIRUS");
```

### Ganti Warna:

**Opsi 1: Di Inspector**
- Boss Label → Label Color: Pilih warna dari color picker

**Opsi 2: Via Script (Runtime)**
```csharp
BossLabel label = GetComponent<BossLabel>();
label.SetLabelColor(Color.yellow); // Kuning
label.SetLabelColor(new Color(1f, 0.5f, 0f)); // Orange
```

### Disable Pulse Animation:

- Boss Label → Animation → **Enable Pulse**: Uncheck

### Disable Billboard (Label Ikut Rotasi Boss):

- Boss Label → Billboard → **Enable Billboard**: Uncheck

## Troubleshooting

### Problem: Teks tidak muncul

**Penyebab 1:** TextMeshPro tidak ter-import
**Solusi:** 
- Window → TextMeshPro → Import TMP Essential Resources

**Penyebab 2:** Camera tidak ditemukan
**Solusi:**
- Pastikan ada camera dengan tag "MainCamera" di scene

### Problem: Teks terlalu kecil/besar

**Solusi:** Adjust Font Size di Inspector
- Boss Label → Font Size: 18-36

### Problem: Teks tertutup oleh sprite boss

**Solusi:** Adjust offset Y lebih tinggi
- Boss Label → Offset Y: 2.5 atau 3

### Problem: Teks tidak menghadap camera

**Penyebab:** Billboard disabled atau camera null
**Solusi:**
- Enable Billboard: Boss Label → Enable Billboard: ✓
- Pastikan Main Camera ada di scene

### Problem: Pulse animation terlalu cepat/lambat

**Solusi:** Adjust speed
- Boss Label → Pulse Speed: 1-3 (1 = slow, 3 = fast)

## Advanced: Multiple Boss Types

Jika ada boss lain selain Omega Virus:

1. **Buat prefab boss baru** (misal: Mega Virus)
2. **Add BossLabel script** ke prefab tersebut
3. **Customize label text** untuk setiap boss:
   - Omega Virus: "OMEGA BOSS"
   - Mega Virus: "MEGA BOSS"
   - Final Boss: "FINAL BOSS"

## Script Location

- **Script:** `Assets/Scripts/UI/BossLabel.cs`
- **Namespace:** `MicroDefense.UI`

## Public Methods

```csharp
// Ganti text
SetLabelText(string newText)

// Ganti warna
SetLabelColor(Color newColor)

// Hide/Show label
HideLabel()
ShowLabel()
```

## Integration dengan Boss Phase (Optional)

Jika ingin label berubah saat boss masuk phase tertentu:

```csharp
// Di OmegaVirus.cs, tambahkan:
private BossLabel bossLabel;

protected override void Awake()
{
    base.Awake();
    bossLabel = GetComponent<BossLabel>();
}

private void CheckHealthThresholdSpawning()
{
    // Existing code...
    
    if (healthPercent <= 0.5f && bossLabel != null)
    {
        bossLabel.SetLabelText("ENRAGED");
        bossLabel.SetLabelColor(Color.red * 1.5f); // Merah lebih terang
    }
}
```

---

**Tanggal:** 5 November 2026  
**Status:** Ready untuk implementasi  
**Dependencies:** TextMeshPro (built-in dengan Unity)
