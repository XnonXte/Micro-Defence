# PANDUAN SETUP HEALTH BAR VERTIKAL

## 📋 RINGKASAN
Health Bar Vertikal dengan cairan yang berkurang dari **ATAS ke BAWAH** menggunakan 2 gambar PNG.

---

## 🎨 ASET YANG DIBUTUHKAN

### 1. wadahBening.png
- **Fungsi**: Container/bingkai indikator (lapisan DEPAN)
- **Rekomendasi**: Gambar transparan dengan border/outline yang jelas
- **Ukuran**: Contoh 128x512 px (vertikal)

### 2. cairanPlasma.png
- **Fungsi**: Isi cairan di dalam wadah (lapisan BELAKANG)
- **Rekomendasi**: Gambar solid atau gradient vertikal
- **Ukuran**: Sama dengan wadahBening.png
- **Warna**: Akan diubah dinamis oleh script (cyan → yellow → red)

---

## 🔧 CARA SETUP DI UNITY

### LANGKAH 1: Import Aset
1. Copy `wadahBening.png` dan `cairanPlasma.png` ke folder `Assets/Resources/UI/`
2. Set **Texture Type** kedua gambar ke **Sprite (2D and UI)**
3. Klik **Apply**

### LANGKAH 2: Buat UI Hierarchy
Di Canvas, buat struktur berikut:

```
Canvas
 └── VerticalHealthBar (Empty GameObject)
      ├── CairanPlasma (Image) ← LAPISAN BELAKANG
      └── WadahBening (Image)  ← LAPISAN DEPAN
```

**Cara membuat:**
1. Klik kanan di Canvas → Create Empty → Rename jadi "VerticalHealthBar"
2. Klik kanan VerticalHealthBar → UI → Image → Rename jadi "CairanPlasma"
3. Klik kanan VerticalHealthBar → UI → Image → Rename jadi "WadahBening"

### LANGKAH 3: Setup CairanPlasma (Image)
**Inspector Settings:**
- **Source Image**: Drag `cairanPlasma.png`
- **Image Type**: **Filled**
- **Fill Method**: **Vertical**
- **Fill Origin**: **Bottom** ← PENTING! Cairan berkurang dari atas
- **Fill Amount**: 1.0 (penuh)
- **Color**: Cyan (akan diubah otomatis oleh script)

**RectTransform:**
- **Anchors**: Stretch-Stretch (untuk memenuhi container)
- **Left**: 0, **Right**: 0, **Top**: 0, **Bottom**: 0
- **Width/Height**: Auto

### LANGKAH 4: Setup WadahBening (Image)
**Inspector Settings:**
- **Source Image**: Drag `wadahBening.png`
- **Image Type**: **Simple**
- **Color**: White (biarkan transparan natural)

**RectTransform:**
- **Anchors**: Stretch-Stretch
- **Left**: 0, **Right**: 0, **Top**: 0, **Bottom**: 0
- Atau sesuaikan ukuran manual (contoh: 128 x 512)

### LANGKAH 5: Attach Script
1. Pilih GameObject **VerticalHealthBar** (parent)
2. Add Component → Cari "**VerticalHealthBar**"
3. **Assign References** di Inspector:
   - **Wadah Bening**: Drag `WadahBening` Image
   - **Cairan Plasma**: Drag `CairanPlasma` Image

### LANGKAH 6: Konfigurasi Settings (Inspector)
```
Pengaturan Health:
- Max HP: 20 (sesuaikan dengan LeakManager)

Animasi:
- Drain Duration: 0.3 detik
- Use Shake Effect: ✓ (centang)
- Shake Intensity: 5
- Shake Duration: 0.2 detik

Visual Feedback:
- Full Health Color: Cyan (0, 255, 255)
- Mid Health Color: Yellow (255, 255, 0)
- Low Health Color: Red (255, 0, 0)
```

### LANGKAH 7: Posisi di Canvas
Letakkan health bar di pojok kanan atas atau tempat yang terlihat:
- **Anchor Preset**: Top-Right
- **Pos X**: -80
- **Pos Y**: -80

---

## 🎮 CARA KERJA

### Otomatis (Sudah Terintegrasi)
Script **otomatis subscribe** ke `LeakManager`:
```csharp
// Tidak perlu panggil manual, sudah auto-connect!
LeakManager.Instance.OnHealthChanged += OnHealthChanged;
```

Saat virus mencapai target:
1. `LeakManager.RegisterLeak(damage)` dipanggil
2. VerticalHealthBar otomatis update via event
3. Animasi cairan turun dari atas
4. Wadah bergetar (shake effect)
5. Warna berubah: Cyan → Yellow → Red

### Manual (Jika Perlu)
```csharp
// Kurangi HP
VerticalHealthBar.Instance.TakeDamage(1);

// Set HP langsung
VerticalHealthBar.Instance.SetHealth(10, animate: true);

// Reset ke penuh
VerticalHealthBar.Instance.ResetHealth();

// Cek HP saat ini
int currentHP = VerticalHealthBar.Instance.CurrentHP;
float ratio = VerticalHealthBar.Instance.HealthRatio; // 0.0 - 1.0
```

---

## 🎨 PERUBAHAN WARNA OTOMATIS

| HP Ratio | Warna Cairan | Kondisi |
|----------|--------------|---------|
| 100% - 51% | Cyan → Yellow | Sehat |
| 50% - 1% | Yellow → Red | Bahaya |
| 0% | Red | Game Over |

---

## ✨ FITUR

### 1. Animasi Smooth Drain (0.3 detik)
- Cairan turun secara halus, tidak langsung loncat
- Menggunakan `SmoothStep` easing untuk efek natural

### 2. Shake Effect saat Damage
- Wadah bergetar saat terkena damage
- Intensitas berkurang seiring waktu (decay)
- Otomatis kembali ke posisi asli

### 3. Perubahan Warna Dinamis
- Cairan berubah warna sesuai HP
- Gradient smooth antara 3 warna

### 4. Fill Amount Vertikal
- Cairan berkurang dari **ATAS ke BAWAH**
- Bagian bawah selalu menempel di dasar wadah
- Menggunakan Unity Image Filled system

---

## 🔍 TROUBLESHOOTING

### Cairan tidak terlihat berkurang?
✅ **Solusi**: Pastikan di Inspector CairanPlasma:
- Image Type = **Filled**
- Fill Method = **Vertical**
- Fill Origin = **Bottom**

### Cairan berkurang dari bawah, bukan dari atas?
✅ **Solusi**: Ubah Fill Origin dari **Top** ke **Bottom**

### Shake effect tidak bekerja?
✅ **Solusi**: 
- Centang "Use Shake Effect" di Inspector
- Pastikan WadahBening sudah di-assign

### Warna tidak berubah?
✅ **Solusi**: Pastikan di Inspector cairanPlasma warna awal di-set ke **White** (255,255,255), bukan warna custom

### Health tidak sinkron dengan LeakManager?
✅ **Solusi**: 
- Pastikan LeakManager.Instance ada di scene
- Cek Max HP di LeakManager sama dengan Max HP di VerticalHealthBar

---

## 📝 CONTOH CUSTOM CALL

Jika ingin memanggil dari script lain:

```csharp
using MicroDefense.UI;

public class MyScript : MonoBehaviour
{
    void Start()
    {
        // Damage 2 HP
        VerticalHealthBar.Instance.TakeDamage(2);
    }
    
    void OnButtonClick()
    {
        // Heal ke 50% HP
        int halfHP = VerticalHealthBar.Instance.MaxHP / 2;
        VerticalHealthBar.Instance.SetHealth(halfHP);
    }
}
```

---

## 🎯 CATATAN PENTING

1. **LeakManager Integration**: Health bar otomatis sinkron dengan `LeakManager.CurrentHealth`
2. **Singleton Pattern**: Akses via `VerticalHealthBar.Instance`
3. **Game Over**: Handled oleh LeakManager, tidak perlu double-handle
4. **Performance**: Animasi menggunakan Coroutine, efisien untuk mobile
5. **Customizable**: Semua parameter bisa diubah di Inspector tanpa edit code

---

## 📸 PREVIEW VISUAL

```
┌─────────────┐ ← WadahBening (Container)
│█████████████│ ← 100% HP (Cyan penuh)
│█████████████│
│█████████████│
│█████████████│
└─────────────┘

┌─────────────┐
│░░░░░░░░░░░░░│ ← Kosong (transparan)
│░░░░░░░░░░░░░│
│█████████████│ ← 50% HP (Yellow)
│█████████████│
└─────────────┘

┌─────────────┐
│░░░░░░░░░░░░░│
│░░░░░░░░░░░░░│
│░░░░░░░░░░░░░│
│█████████████│ ← 25% HP (Red)
└─────────────┘

┌─────────────┐
│░░░░░░░░░░░░░│
│░░░░░░░░░░░░░│ ← 0% HP (Kosong)
│░░░░░░░░░░░░░│
│░░░░░░░░░░░░░│
└─────────────┘
```

---

## ✅ CHECKLIST SETUP

- [ ] Import wadahBening.png dan cairanPlasma.png
- [ ] Set Texture Type ke Sprite (2D and UI)
- [ ] Buat GameObject VerticalHealthBar di Canvas
- [ ] Buat child Image: CairanPlasma (lapisan belakang)
- [ ] Buat child Image: WadahBening (lapisan depan)
- [ ] Set CairanPlasma: Type=Filled, Method=Vertical, Origin=Bottom
- [ ] Attach script VerticalHealthBar ke parent GameObject
- [ ] Assign WadahBening dan CairanPlasma di Inspector
- [ ] Test: Jalankan game, cek apakah bar muncul penuh
- [ ] Test: Panggil TakeDamage(), cek animasi turun dari atas

---

**SELESAI! Health Bar siap digunakan! 🎉**
