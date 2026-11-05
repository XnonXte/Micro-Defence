# Panduan Setup Macrophage Hit Effect (Shake/Getaran)

## Overview

Script **MacrophageHitEffect.cs** sudah dibuat untuk menambahkan efek visual shake/getaran pada Macrophage Tower saat menerima damage dari virus.

## Fitur Efek Visual:

✅ **Position Shake** - Getaran posisi (horizontal & vertical)
✅ **Rotation Shake** - Getaran rotasi (spinning effect)
✅ **Scale Pulse** - Efek "menciut" saat kena hit
✅ **Color Flash** - Flash warna merah saat kena damage

## Cara Setup di Unity Editor

### 1. Buka Prefab Macrophage

1. **Project Window** → Cari prefab Macrophage Tower
2. **Double-click** untuk buka Prefab Mode
3. Atau **drag ke scene** untuk edit

### 2. Tambahkan Hit Effect Script

1. **Select root GameObject** dari Macrophage Tower
2. **Inspector → Add Component**
3. Ketik **"Macrophage Hit Effect"**
4. Pilih **Macrophage Hit Effect (Script)**

### 3. Configure Settings di Inspector

**Default Settings (Recommended):**

```
Macrophage Hit Effect (Script)
├── Shake Settings
│   ├── Enable Shake: ✓ (checked)
│   ├── Shake Duration: 0.2 seconds
│   ├── Shake Intensity: 0.1
│   └── Shake Speed: 30
├── Rotation Shake (Optional)
│   ├── Enable Rotation Shake: ✓
│   └── Rotation Shake Amount: 10 degrees
├── Scale Pulse (Optional)
│   ├── Enable Scale Pulse: ✓
│   ├── Scale Pulse Amount: 0.1 (10% shrink)
│   └── Scale Pulse Duration: 0.15 seconds
└── Color Flash (Optional)
    ├── Enable Color Flash: ✓
    ├── Flash Color: Light Red (255, 128, 128, 255)
    └── Flash Duration: 0.1 seconds
```

### 4. Customization (Optional)

#### Getaran Lebih Kuat:
- **Shake Intensity**: 0.2 (lebih kuat)
- **Shake Speed**: 50 (lebih cepat)

#### Getaran Lebih Halus:
- **Shake Intensity**: 0.05 (lebih halus)
- **Shake Duration**: 0.1 (lebih singkat)

#### Disable Efek Tertentu:
- **Enable Rotation Shake**: Uncheck (tanpa rotasi)
- **Enable Scale Pulse**: Uncheck (tanpa scale effect)
- **Enable Color Flash**: Uncheck (tanpa flash warna)

### 5. Save Prefab

1. **Ctrl+S** atau File → Save
2. **Exit Prefab Mode** (tombol < di Hierarchy)

## Cara Kerja

### Automatic Detection:

Script otomatis mendeteksi damage dengan cara:
1. ✅ Monitor `MacrophageTower.currentHealth` setiap frame
2. ✅ Jika health turun → Trigger hit effect
3. ✅ Play semua efek yang di-enable

### Visual Effects Sequence:

```
Enemy Hit Macrophage
        ↓
Health Berkurang
        ↓
┌───────┴───────┐
│ Trigger Effect │
└───────┬────────┘
        ├─→ Position Shake (0.2s)
        ├─→ Rotation Shake (0.2s)
        ├─→ Scale Pulse (0.15s)
        └─→ Color Flash (0.1s)
```

## Testing

### Test Checklist:

- [ ] Spawn Macrophage di level
- [ ] Biarkan virus mendekat dan attack Macrophage
- [ ] Macrophage harus bergetar/shake saat kena hit
- [ ] Efek shake tidak mengganggu gameplay
- [ ] Macrophage kembali normal setelah efek selesai

### Expected Result:

**Saat virus hit Macrophage:**
1. 🔴 Flash merah sebentar
2. 📳 Bergetar horizontal & vertical
3. 🔄 Berputar sedikit (wobble)
4. 📉 Scale menciut sebentar lalu normal

## Troubleshooting

### Problem: Efek tidak muncul

**Penyebab 1:** Script tidak attached ke prefab
**Solusi:** Add MacrophageHitEffect component ke prefab

**Penyebab 2:** Semua efek di-disable
**Solusi:** Enable minimal 1 efek (Enable Shake/Scale Pulse/Color Flash)

**Penyebab 3:** MacrophageTower component tidak ditemukan
**Solusi:** Pastikan script di-attach ke GameObject yang sama dengan MacrophageTower

### Problem: Shake terlalu kuat/mengganggu

**Solusi:** Kurangi intensity
- Shake Intensity: 0.05 (dari 0.1)
- Shake Duration: 0.1 (dari 0.2)

### Problem: Color flash tidak terlihat

**Penyebab:** SpriteRenderer tidak ditemukan
**Solusi:**
- Pastikan Macrophage punya SpriteRenderer component
- Atau SpriteRenderer ada di child GameObject

**Alternatif:** Disable color flash jika tidak diperlukan
- Enable Color Flash: Uncheck

### Problem: Efek terlalu subtle/tidak keliatan

**Solusi:** Perbesar intensity
- Shake Intensity: 0.2
- Scale Pulse Amount: 0.2 (20%)
- Rotation Shake Amount: 20 degrees

### Problem: Transform stuck setelah efek

**Penyebab:** Coroutine interrupted
**Solusi:** Script sudah handle cleanup otomatis, tapi jika masih stuck:
- Restart scene
- Check console untuk error

## Advanced: Manual Trigger

### Trigger Efek dari Script Lain:

```csharp
MacrophageHitEffect hitEffect = macrophage.GetComponent<MacrophageHitEffect>();
if (hitEffect != null)
{
    hitEffect.PlayHitEffect(); // Trigger manual
}
```

### Trigger Efek dengan Event:

Di MacrophageTower.cs, bisa tambahkan event:

```csharp
// Di class MacrophageTower
public event Action OnTakeDamage;

public void TakeDamage(float rawDamage)
{
    // Existing code...
    
    OnTakeDamage?.Invoke(); // Trigger event
    
    // Rest of code...
}
```

Lalu di MacrophageHitEffect.cs:

```csharp
private void Start()
{
    // Existing code...
    
    if (macrophageTower != null)
    {
        macrophageTower.OnTakeDamage += TriggerHitEffect;
    }
}

private void OnDestroy()
{
    if (macrophageTower != null)
    {
        macrophageTower.OnTakeDamage -= TriggerHitEffect;
    }
}
```

## Performance Notes

✅ **Efisien** - Hanya berjalan saat damage terjadi
✅ **No Update() overhead** - Hanya monitor health change
✅ **Auto cleanup** - Coroutines di-stop saat destroy
✅ **Smooth** - Menggunakan coroutine, tidak blocking

## Customization Examples

### Efek "Bounce" (Mantul):

```
Scale Pulse Amount: 0.15
Scale Pulse Duration: 0.2
Enable Rotation Shake: Uncheck
```

### Efek "Wobble" (Goyang):

```
Enable Rotation Shake: ✓
Rotation Shake Amount: 20
Shake Intensity: 0.15
Enable Scale Pulse: Uncheck
```

### Efek "Minimal":

```
Shake Duration: 0.1
Shake Intensity: 0.05
Enable Rotation Shake: Uncheck
Enable Scale Pulse: Uncheck
Enable Color Flash: ✓
```

### Efek "Intense":

```
Shake Duration: 0.3
Shake Intensity: 0.2
Shake Speed: 50
Rotation Shake Amount: 15
Scale Pulse Amount: 0.2
```

## Integration dengan Audio

Efek visual ini berjalan bersamaan dengan:
- `AudioManager.PlayMacrophageHurt()` SFX
- Health bar update
- Damage number popup (jika ada)

## Script Location

- **Script:** `Assets/Scripts/Towers/MacrophageHitEffect.cs`
- **Namespace:** `MicroDefense.Towers`

## Public Methods

```csharp
// Manual trigger
PlayHitEffect()

// Update transform reference (jika Macrophage bergerak)
UpdateOriginalTransform()
```

---

**Tanggal:** 5 November 2026  
**Status:** Ready untuk implementasi  
**Auto-detect:** Ya, otomatis detect damage  
**Performance Impact:** Minimal (hanya saat damage)
