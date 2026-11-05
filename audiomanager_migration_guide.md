# Panduan Migrasi AudioManager ke Singleton Pattern

## Ringkasan Perubahan

AudioManager sekarang menggunakan **Singleton Pattern dengan DontDestroyOnLoad**, yang berarti:
- AudioManager hanya perlu ada di **Main Menu scene**
- AudioManager akan **tetap hidup** (persist) di semua scene berikutnya (Level 1-5)
- **Tidak perlu** lagi menaruh AudioManager di setiap level scene

## Langkah-Langkah Migrasi

### 1. Pastikan AudioManager Ada di Main Menu Scene

1. Buka scene **Main Menu** di Unity
2. Pastikan ada GameObject `AudioManager` (atau `_Managers` yang berisi AudioManager)
3. Pastikan semua **Audio Clips** sudah di-assign di Inspector:
   - BGM Clips: `mainMenuBGM`, `level1To5BGM`, `level6To10BGM`
   - SFX Clips: semua sound effects yang diperlukan

### 2. Hapus AudioManager dari Level Scenes (Level 1-5)

Untuk **setiap level scene** (Level 1, Level 2, Level 3, Level 4, Level 5):

1. Buka scene level tersebut di Unity
2. Cari GameObject yang memiliki komponen `AudioManager`
   - Biasanya bernama `AudioManager` atau `_Managers`
3. **HAPUS** GameObject AudioManager dari scene tersebut
   - Select GameObject → Klik kanan → Delete
   - Atau select lalu tekan tombol Delete
4. **PASTIKAN** ada GameObject `BGMManager` di setiap level
   - BGMManager akan otomatis menggunakan AudioManager yang sudah ada dari Main Menu
5. Save scene (Ctrl+S)

### 3. Verifikasi BGMManager Ada di Setiap Level

Untuk **setiap level scene**:

1. Buka scene level
2. Pastikan ada GameObject dengan komponen **`BGMManager`**
   - Jika belum ada, buat GameObject baru:
     - Hierarchy → Klik kanan → Create Empty
     - Rename menjadi "BGMManager"
     - Add Component → BGMManager
3. BGMManager akan otomatis:
   - Mendeteksi nama scene saat ini
   - Memutar BGM yang sesuai dari AudioManager
4. Save scene

### 4. Testing

#### Test Flow Normal:
1. **Play dari Main Menu**
   - Main Menu BGM harus terdengar
2. **Mulai Level 1**
   - BGM berubah ke `level1To5BGM`
3. **Pindah ke Level 2, 3, 4, 5**
   - BGM tetap `level1To5BGM` (atau berubah ke `level6To10BGM` untuk level 4-5 tergantung mapping)
4. **Semua SFX harus berfungsi**
   - Tower placement, shooting, UI clicks, dll

#### Test Play dari Level Langsung:
1. **Buka scene Level 1 langsung** (tanpa lewat Main Menu)
2. **Tekan Play**
3. **HASIL YANG DIHARAPKAN:**
   - Console akan menampilkan warning: `[BGMManager] AudioManager.Instance is NULL!`
   - Ini **NORMAL** karena AudioManager hanya ada di Main Menu
   - Game masih bisa dimainkan, hanya tidak ada audio
4. **Solusi untuk Development Testing:**
   - Lihat bagian "Tips Development" di bawah

### 5. Struktur Scene yang Benar

#### Main Menu Scene:
```
Main Menu
├── AudioManager (dengan DontDestroyOnLoad)
│   ├── BGMSource
│   ├── SFXSource
│   └── (semua audio clips di-assign)
├── BGMManager
└── (UI dan komponen lainnya)
```

#### Level Scenes (1-5):
```
Level 1 (atau 2, 3, 4, 5)
├── BGMManager (menggunakan AudioManager yang sudah ada)
├── GameManager
├── WaveManager
└── (komponen level lainnya)

TIDAK ADA AudioManager disini!
```

## Cara Kerja Sistem

### Singleton Pattern
```csharp
// Di AudioManager.cs
private void Awake()
{
    if (Instance != null && Instance != this)
    {
        // Jika sudah ada instance lain, hapus yang duplikat
        Destroy(gameObject);
        return;
    }
    
    Instance = this;
    DontDestroyOnLoad(gameObject); // Tetap hidup di semua scene
}
```

### Scene Transitions
1. **Main Menu loaded** → AudioManager dibuat, main menu BGM dimainkan
2. **Pindah ke Level 1** → AudioManager tetap ada (tidak dihapus)
3. **BGMManager di Level 1** → Memanggil `AudioManager.Instance.PlayBGM(level1To5BGM)`
4. **Semua script lain** → Menggunakan `AudioManager.Instance?.PlaySFX(...)`

### Keamanan Null-Safe
Semua script sudah menggunakan pola yang aman:
```csharp
// Null-conditional operator
AudioManager.Instance?.PlayButtonClick();

// Atau null check
if (AudioManager.Instance != null)
{
    AudioManager.Instance.PlaySFX(clip);
}
```

## Tips Development

### Testing Level Langsung (Without Main Menu)

Jika ingin test level langsung tanpa lewat Main Menu, ada 2 opsi:

#### Opsi A: Additive Scene Loading (Recommended)
1. Buka Window → General → Scene Loader (atau buat script helper)
2. Load Main Menu scene secara additive saat development:
```csharp
#if UNITY_EDITOR
void Start()
{
    if (AudioManager.Instance == null)
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
    }
}
#endif
```

#### Opsi B: Temporary AudioManager Prefab
1. Buat prefab AudioManager yang bisa di-drag ke scene saat testing
2. **JANGAN LUPA HAPUS** sebelum build final

## Troubleshooting

### Problem: Tidak ada audio di level
**Penyebab:** AudioManager tidak ada (play langsung dari level scene)
**Solusi:** Selalu play dari Main Menu scene, atau gunakan opsi development di atas

### Problem: Audio terdengar ganda atau beberapa instance
**Penyebab:** Ada AudioManager di beberapa scene
**Solusi:** Hapus semua AudioManager dari level scenes, hanya tinggalkan di Main Menu

### Problem: BGM tidak berubah saat pindah level
**Penyebab:** BGMManager tidak ada di level scene
**Solusi:** Pastikan setiap level memiliki GameObject dengan komponen BGMManager

### Problem: Console menampilkan "AudioManager destroyed" berulang-ulang
**Penyebab:** Masih ada AudioManager di level scenes
**Solusi:** Hapus semua AudioManager dari semua level scenes

## Checklist Migrasi

- [ ] AudioManager ada dan sudah configured di Main Menu scene
- [ ] Semua audio clips di-assign di AudioManager (BGM + SFX)
- [ ] AudioManager dihapus dari Level 1 scene
- [ ] AudioManager dihapus dari Level 2 scene
- [ ] AudioManager dihapus dari Level 3 scene
- [ ] AudioManager dihapus dari Level 4 scene
- [ ] AudioManager dihapus dari Level 5 scene
- [ ] BGMManager ada di setiap level scene
- [ ] Test play dari Main Menu → Level 1 → Level 2 (audio berfungsi)
- [ ] Test SFX (tower placement, shooting, UI clicks)
- [ ] Test volume controls di settings menu

## File yang Dimodifikasi

- `Assets/Scripts/Core/AudioManager.cs` - Updated Awake() dengan comment yang lebih jelas
- Script lain **TIDAK PERLU** diubah (sudah menggunakan singleton pattern dengan benar)

## Manfaat Sistem Baru

✅ **Lebih Efficient** - Hanya satu AudioManager di memory
✅ **Easier Maintenance** - Update audio clips di satu tempat (Main Menu)
✅ **No Duplication** - Tidak ada multiple instances yang bertabrakan
✅ **Seamless Transitions** - Audio tetap berjalan saat scene transitions
✅ **Better Performance** - Tidak perlu load/unload audio clips setiap scene

---

**Tanggal Migrasi:** 5 November 2026
**Status:** Ready untuk implementasi
