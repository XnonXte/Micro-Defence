# Panduan Setup Settings Panel dengan Volume Sliders

## Overview

Anda memiliki 2 script untuk volume control:
- **VolumeController.cs** (Recommended) - Khusus volume, lebih lengkap dengan percentage display dan preview sound
- **SettingsController.cs** - Settings umum (volume + quality + fullscreen)

Pilih salah satu sesuai kebutuhan:
- **Opsi A**: Gunakan **VolumeController** jika hanya butuh volume controls
- **Opsi B**: Gunakan **SettingsController** jika butuh settings lengkap (volume + quality + fullscreen)

---

## Opsi A: Setup dengan VolumeController (Recommended untuk Volume Only)

### 1. Buat Settings Panel UI

**Di Unity Editor:**

1. **Hierarchy → UI → Panel** (atau gunakan panel yang sudah ada)
2. **Rename** panel jadi `SettingsPanel`
3. **Setup Panel Background**:
   - Add Image component jika belum ada
   - Set warna semi-transparent (misal: RGBA 0, 0, 0, 200)

### 2. Buat Volume Controls untuk BGM

**Di dalam SettingsPanel:**

1. **Create Slider untuk BGM:**
   - Klik kanan SettingsPanel → **UI → Slider**
   - Rename jadi `BGMSlider`
   - Position: atur sesuai layout (misal X: 0, Y: 50)
   - Width: 300-400

2. **Create Label untuk BGM:**
   - Klik kanan SettingsPanel → **UI → Text - TextMeshPro**
   - Rename jadi `BGMLabel`
   - Text: "BGM Volume"
   - Position: di sebelah kiri slider

3. **Create Value Text untuk BGM (Optional):**
   - Klik kanan SettingsPanel → **UI → Text - TextMeshPro**
   - Rename jadi `BGMValueText`
   - Text: "100%"
   - Position: di sebelah kanan slider
   - Font size: sedikit lebih kecil

### 3. Buat Volume Controls untuk SFX

**Ulangi langkah yang sama untuk SFX:**

1. **Create Slider untuk SFX:**
   - Klik kanan SettingsPanel → **UI → Slider**
   - Rename jadi `SFXSlider`
   - Position: di bawah BGM slider (misal Y: 0)

2. **Create Label untuk SFX:**
   - Text: "SFX Volume"

3. **Create Value Text untuk SFX (Optional):**
   - Rename jadi `SFXValueText`
   - Text: "100%"

### 4. Setup Slider Settings

**Untuk kedua slider (BGM & SFX):**

1. **Select Slider** di Hierarchy
2. **Di Inspector → Slider Component:**
   - Min Value: **0**
   - Max Value: **1**
   - Whole Numbers: **unchecked** (false)
   - Value: **1**

3. **Visual Settings (Optional):**
   - Background: warna gelap
   - Fill: warna terang (hijau/biru untuk BGM, kuning/orange untuk SFX)
   - Handle: warna putih atau accent

### 5. Attach VolumeController Script

**Setup Script:**

1. **Create Empty GameObject** di dalam SettingsPanel
   - Rename jadi `VolumeController`

2. **Add Component → Volume Controller**

3. **Assign References di Inspector:**
   ```
   Volume Controller (Script)
   ├── Volume Sliders
   │   ├── BGM Slider: [Drag BGMSlider here]
   │   └── SFX Slider: [Drag SFXSlider here]
   ├── Volume Text Labels (Optional)
   │   ├── BGM Value Text: [Drag BGMValueText here]
   │   └── SFX Value Text: [Drag SFXValueText here]
   └── Settings
       ├── Show Percentage: ✓ (checked)
       └── Percent Format: "0" (untuk 75%, atau "0.0" untuk 75.5%)
   ```

### 6. Tambah Tombol Close/Back

1. **Create Button:**
   - Klik kanan SettingsPanel → **UI → Button - TextMeshPro**
   - Rename jadi `CloseButton`
   - Text: "✕" atau "Back" atau "Close"

2. **Add OnClick Event:**
   - Drag SettingsPanel ke onClick event
   - Function: `GameObject.SetActive(bool)` → unchecked (false)

### 7. Contoh Layout

```
SettingsPanel
├── Background (Image)
├── Title (Text: "Settings")
├── BGMLabel (Text: "BGM Volume")
├── BGMSlider (Slider: 0-1)
├── BGMValueText (Text: "100%")
├── SFXLabel (Text: "SFX Volume")
├── SFXSlider (Slider: 0-1)
├── SFXValueText (Text: "100%")
├── CloseButton (Button: "Close")
└── VolumeController (GameObject with VolumeController script)
```

---

## Opsi B: Setup dengan SettingsController (Settings Lengkap)

### Jika Anda Butuh Settings Lengkap:

Gunakan **SettingsController.cs** yang sudah include:
- BGM Volume Slider
- SFX Volume Slider
- Quality Dropdown
- Fullscreen Toggle

### Setup Steps:

1. **Buat Panel seperti Opsi A**
2. **Tambahkan elemen tambahan:**
   - Quality Dropdown (UI → Dropdown - TextMeshPro)
   - Fullscreen Toggle (UI → Toggle)
3. **Attach SettingsController** langsung ke SettingsPanel GameObject
4. **Assign semua references:**
   - BGM Volume Slider
   - SFX Volume Slider
   - Quality Dropdown
   - Fullscreen Toggle
   - Back Button

---

## Cara Membuka Settings Panel dari Main Menu

### Opsi 1: Dari Button

1. **Create Settings Button** di Main Menu
2. **Add OnClick Event:**
   - Drag SettingsPanel ke event
   - Function: `GameObject.SetActive(bool)` → checked (true)

### Opsi 2: Dari Script (MainMenuController)

```csharp
public void ShowSettings()
{
    mainMenuPanel.SetActive(false);
    settingsPanel.SetActive(true);
}

public void HideSettings()
{
    settingsPanel.SetActive(false);
    mainMenuPanel.SetActive(true);
}
```

---

## Testing

### Test Checklist:

- [ ] **Buka Settings Panel** - Panel muncul dengan benar
- [ ] **Geser BGM Slider** - BGM volume berubah real-time
- [ ] **Geser SFX Slider** - Terdengar button click sound sebagai preview
- [ ] **Value Text Update** - Angka percentage berubah saat slider digeser
- [ ] **Close Panel** - Panel tertutup dengan baik
- [ ] **Reopen Panel** - Slider position tersimpan (tidak reset ke default)
- [ ] **Restart Game** - Volume settings tetap tersimpan (PlayerPrefs)

### Expected Behavior:

✅ **Saat geser BGM Slider:**
- BGM volume langsung berubah
- Text menampilkan "75%" jika slider di 0.75

✅ **Saat geser SFX Slider:**
- Terdengar button click sound setiap kali slider berubah (preview)
- Text menampilkan "50%" jika slider di 0.5

✅ **Saat restart game:**
- Volume settings tetap sama seperti terakhir kali di-set
- Slider position sudah di posisi yang benar

---

## Fitur VolumeController

**VolumeController** punya fitur tambahan:

### Public Methods yang Bisa Dipanggil:

```csharp
// Set volume
volumeController.SetBGMVolume(0.5f); // 50%
volumeController.SetSFXVolume(1f);   // 100%

// Reset ke default (100%)
volumeController.ResetToDefault();

// Mute/Unmute
volumeController.MuteBGM();
volumeController.UnmuteBGM();
volumeController.MuteSFX();
volumeController.UnmuteSFX();
```

### Tambah Tombol Mute (Optional):

1. **Create Toggle Button** untuk mute BGM
2. **Add OnClick:**
   - Drag VolumeController GameObject
   - Function: `VolumeController.MuteBGM()` atau `UnmuteBGM()`

---

## Troubleshooting

### Problem: Slider tidak berpengaruh pada volume

**Penyebab:** AudioManager tidak ada atau belum diinisialisasi
**Solusi:** 
- Pastikan AudioManager ada di Main Menu scene
- Play dari Main Menu, bukan langsung dari scene settings

### Problem: Value text tidak update

**Penyebab:** BGMValueText/SFXValueText tidak di-assign
**Solusi:** 
- Assign reference di Inspector VolumeController
- Atau hapus reference jika tidak butuh text display

### Problem: Volume tidak tersimpan setelah restart

**Penyebab:** PlayerPrefs tidak di-save
**Solusi:** 
- Check AudioManager.cs line 186-187 dan 206-207
- Pastikan ada `PlayerPrefs.Save()` setelah `PlayerPrefs.SetFloat()`
- (Sudah ada di AudioManager yang sekarang)

### Problem: SFX preview terlalu sering/mengganggu

**Penyebab:** PlaySFXPreview() dipanggil setiap slider berubah
**Solusi:**
- Comment line 125 di VolumeController.cs: `// PlaySFXPreview();`
- Atau tambah cooldown timer untuk preview

---

## Layout Recommendations

### Vertical Layout (Recommended):

```
┌─────────────────────────┐
│       SETTINGS          │
│                         │
│  BGM Volume             │
│  [═══════○──]  75%      │
│                         │
│  SFX Volume             │
│  [════════○─]  85%      │
│                         │
│  [Quality: Medium  ▼]   │
│  [✓] Fullscreen         │
│                         │
│      [  Close  ]        │
└─────────────────────────┘
```

### Tips:
- Gunakan **Vertical Layout Group** untuk spacing otomatis
- Spacing antar elemen: 20-30 pixels
- Padding: 30-40 pixels dari tepi panel
- Min slider width: 300 pixels

---

## Quick Setup Summary

**5 Menit Setup:**

1. ✅ Buat Panel di Canvas
2. ✅ Tambah 2 Sliders (BGM & SFX) dengan range 0-1
3. ✅ Tambah 2 Text untuk label dan value (optional)
4. ✅ Create Empty GameObject → Add VolumeController script
5. ✅ Drag & drop sliders dan texts ke Inspector
6. ✅ Tambah Close button
7. ✅ Test!

**Files yang Digunakan:**
- `Assets/Scripts/UI/VolumeController.cs` (sudah ada)
- `Assets/Scripts/Core/AudioManager.cs` (sudah ada)

**Tidak Perlu Edit Code!** Semua setup dilakukan di Unity Inspector.

---

**Tanggal:** 5 November 2026  
**Status:** Ready untuk implementasi
