# Panduan Setup Enemy Hit Effect (Shake/Getaran untuk Semua Enemy)

## Overview

Script **EnemyHitEffect.cs** sudah dibuat untuk menambahkan efek visual shake/getaran pada **semua jenis enemy/virus** saat menerima damage dari tower.

## Fitur Efek Visual:

✅ **Position Shake** - Getaran posisi saat kena hit
✅ **Rotation Shake** - Efek berputar/wobble
✅ **Scale Pulse** - Efek menciut sebentar
✅ **Color Flash** - Flash putih saat kena damage

## Universal untuk Semua Enemy Types:

- Common Virus
- Swarm Virus
- Armored Virus
- Mutant Virus
- Omega Boss
- Semua enemy lainnya

## Cara Setup di Unity Editor

### Metode 1: Setup di Prefab (Recommended)

**Untuk setiap enemy prefab:**

1. **Buka prefab enemy** (Common Virus, Swarm Virus, dll)
2. **Select root GameObject**
3. **Inspector → Add Component → Enemy Hit Effect**
4. **Settings default sudah OK**, atau customize per enemy type
5. **Save Prefab** (Ctrl+S)

**Ulangi untuk semua enemy prefabs:**
- Common Virus prefab
- Swarm Virus prefab
- Armored Virus prefab
- Mutant Virus prefab
- Omega Virus prefab
- dll

### Metode 2: Bulk Add via Editor Script (Advanced)

Jika punya banyak enemy prefabs, bisa buat editor script untuk auto-add component ke semua prefabs sekaligus.

## Default Settings (Universal)

```
Enemy Hit Effect (Script)
├── Shake Settings
│   ├── Enable Shake: ✓
│   ├── Shake Duration: 0.15 seconds (lebih cepat dari Macrophage)
│   ├── Shake Intensity: 0.08 (lebih halus)
│   └── Shake Speed: 40
├── Rotation Shake
│   ├── Enable Rotation Shake: ✓
│   └── Rotation Shake Amount: 8 degrees
├── Scale Pulse
│   ├── Enable Scale Pulse: ✓
│   ├── Scale Pulse Amount: 0.08 (8% shrink)
│   └── Scale Pulse Duration: 0.12 seconds
└── Color Flash
    ├── Enable Color Flash: ✓
    ├── Flash Color: White (255, 255, 255, 255)
    └── Flash Duration: 0.1 seconds
```

## Customization per Enemy Type

### Common Virus (Weak):
```
Shake Intensity: 0.08
Shake Duration: 0.15
Scale Pulse Amount: 0.08
```

### Swarm Virus (Fast, Small):
```
Shake Intensity: 0.06 (lebih kecil)
Shake Duration: 0.1 (lebih cepat)
Shake Speed: 50 (lebih rapid)
```

### Armored Virus (Tanky):
```
Shake Intensity: 0.05 (lebih subtle)
Shake Duration: 0.2 (lebih lama)
Rotation Shake Amount: 5 (lebih stiff)
```

### Mutant Virus (Special):
```
Shake Intensity: 0.1
Flash Color: Yellow (255, 255, 0) - beda dari yang lain
Scale Pulse Amount: 0.12
```

### Omega Boss (Big):
```
Shake Intensity: 0.15 (lebih kuat karena besar)
Shake Duration: 0.25
Rotation Shake Amount: 12
Scale Pulse Amount: 0.1
```

## Cara Kerja

### Automatic Detection:

```
Tower Shoot Enemy
        ↓
Enemy.TakeDamage()
        ↓
Health Berkurang
        ↓
EnemyHitEffect Auto-Detect
        ↓
📳 Shake + 🔄 Rotate + 📉 Scale + ⚪ Flash
```

### Visual Effects Sequence:

1. **Tower menembak enemy**
2. **Enemy health berkurang**
3. **Script detect health drop**
4. **Play all enabled effects:**
   - Position shake (0.15s)
   - Rotation shake (0.15s)
   - Scale pulse (0.12s)
   - Color flash (0.1s)

## Testing

### Test Checklist:

- [ ] Add script ke semua enemy prefabs
- [ ] Spawn wave dengan berbagai jenis enemy
- [ ] Place tower (T-Cell atau B-Cell)
- [ ] Tower shoot enemy → enemy harus shake saat kena hit
- [ ] Test semua enemy types (Common, Swarm, Armored, Mutant, Boss)
- [ ] Efek tidak mengganggu gameplay atau movement

### Expected Result:

**Saat enemy kena tembakan tower:**
1. ⚪ Flash putih sebentar
2. 📳 Bergetar horizontal & vertical
3. 🔄 Berputar sedikit (wobble)
4. 📉 Scale menciut sebentar lalu normal
5. Enemy tetap jalan normal setelah efek selesai

## Troubleshooting

### Problem: Efek tidak muncul

**Penyebab 1:** Script tidak attached ke enemy prefab
**Solusi:** Add EnemyHitEffect component ke semua enemy prefabs

**Penyebab 2:** EnemyBase component tidak ditemukan
**Solusi:** Pastikan script di-attach ke GameObject yang sama dengan EnemyBase

**Penyebab 3:** Semua efek di-disable
**Solusi:** Enable minimal 1 efek di Inspector

### Problem: Enemy position stuck setelah shake

**Penyebab:** LateUpdate() sudah handle movement, tapi jika masih stuck
**Solusi:** 
- Cek apakah ada conflict dengan NavMeshAgent
- Script sudah auto-update original position setiap frame

### Problem: Shake terlihat aneh saat enemy bergerak

**Penyebab:** Normal, karena shake berlapis dengan movement
**Solusi:** Kurangi shake intensity atau duration
- Shake Intensity: 0.05
- Shake Duration: 0.1

### Problem: Color flash tidak terlihat di enemy tertentu

**Penyebab:** SpriteRenderer tidak ditemukan atau sprite sudah putih
**Solusi:**
- Check apakah enemy punya SpriteRenderer
- Ganti Flash Color ke warna lain (merah, kuning)
- Atau disable color flash untuk enemy tersebut

### Problem: Efek terlalu subtle/tidak keliatan

**Solusi:** Perbesar intensity khusus untuk enemy kecil
- Shake Intensity: 0.12
- Scale Pulse Amount: 0.15
- Rotation Shake Amount: 12

### Problem: Boss shake terlalu kecil

**Solusi:** Boss butuh intensity lebih besar
- Shake Intensity: 0.15-0.2
- Rotation Shake Amount: 15
- Scale Pulse Amount: 0.12

## Performance

✅ **Efisien** - Hanya berjalan saat damage
✅ **Auto-detect** - Monitor health every frame (minimal overhead)
✅ **Auto-cleanup** - Coroutines stopped on destroy
✅ **Movement-friendly** - LateUpdate() handle enemy movement

## Perbedaan dengan MacrophageHitEffect

| Feature | MacrophageHitEffect | EnemyHitEffect |
|---------|-------------------|----------------|
| Target | Macrophage Tower | All Enemy Types |
| Shake Intensity | 0.1 (stronger) | 0.08 (subtle) |
| Shake Duration | 0.2s | 0.15s (faster) |
| Flash Color | Red | White |
| Use Case | Tower kena hit | Enemy kena hit |
| Movement | Static tower | Moving enemies |

## Integration dengan Existing Systems

Efek visual ini berjalan bersamaan dengan:
- `EnemyBase.PlayHitEffect()` - Flash color (existing)
- Health bar update
- Damage SFX
- Tower attack animations

## Advanced: Custom Flash Colors per Enemy Type

Bisa customize flash color untuk visual variety:

```csharp
// Common Virus - White flash
Flash Color: (255, 255, 255)

// Swarm Virus - Yellow flash
Flash Color: (255, 255, 100)

// Armored Virus - Gray flash
Flash Color: (200, 200, 200)

// Mutant Virus - Green flash
Flash Color: (100, 255, 100)

// Boss - Red flash
Flash Color: (255, 100, 100)
```

## Script Location

- **Script:** `Assets/Scripts/Enemies/EnemyHitEffect.cs`
- **Namespace:** `MicroDefense.Enemies`

## Public Methods

```csharp
// Manual trigger (dapat dipanggil dari EnemyBase)
PlayHitEffect()

// Trigger otomatis via health monitoring
TriggerHitEffect()
```

## Optional: Integration dengan EnemyBase

Jika ingin lebih tightly integrated, bisa tambahkan di EnemyBase.cs:

```csharp
// Di EnemyBase.TakeDamage()
protected virtual void PlayHitEffect()
{
    // Existing code untuk color flash...
    
    // Trigger EnemyHitEffect jika ada
    EnemyHitEffect hitEffect = GetComponent<EnemyHitEffect>();
    if (hitEffect != null)
    {
        hitEffect.PlayHitEffect();
    }
}
```

Tapi script saat ini sudah auto-detect via health monitoring, jadi integration opsional.

## Quick Setup Checklist

- [ ] Add EnemyHitEffect ke Common Virus prefab
- [ ] Add EnemyHitEffect ke Swarm Virus prefab
- [ ] Add EnemyHitEffect ke Armored Virus prefab
- [ ] Add EnemyHitEffect ke Mutant Virus prefab
- [ ] Add EnemyHitEffect ke Omega Boss prefab
- [ ] Test semua enemy types di game
- [ ] Adjust intensity per enemy type jika perlu

---

**Tanggal:** 5 November 2026  
**Status:** Ready untuk implementasi  
**Universal:** Ya, untuk semua enemy types  
**Auto-detect:** Ya, monitor health changes  
**Performance Impact:** Minimal (per-enemy basis)
