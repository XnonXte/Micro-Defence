# 🎮 GAME SETUP - SIAP MAIN!

## ✅ SEMUA SISTEM SUDAH OTOMATIS!

Script `GameSceneSetup.cs` akan **AUTO-CREATE** semua UI dan managers saat game start!

---

## 📋 CARA SETUP CEPAT (5 MENIT):

### **1. BUKA SCENE TILEMAP**
- Assets → Scenes → **Tilemap.unity**

### **2. SETUP WAVE DATA** (PENTING!)
- Di Hierarchy, cari/buat GameObject: **WaveManager**
- Inspector → WaveManager component
- **Waves (list)**: Assign WaveDataSO yang sudah dibuat
  - Contoh: Wave 1, Wave 2, Wave 3, Wave 4, Wave 5
- ✅ Centang **Auto Start Waves**
- **Spawn Point**: Assign Transform spawn point
- **Target Core Base**: Assign Transform target destination
- ✅ Centang **Use Nav Mesh**

### **3. SETUP ENEMY DAMAGE**
- Assets → Data → EnemyData → **Common Virus**
- Inspector → **Damage To Cell** = `1` (atau sesuai keinginan)
- Ulangi untuk enemy lain (Mutant Virus, Boss, dll)

### **4. SETUP PLAYER HEALTH**
- Di Hierarchy, cari GameObject: **PlayerHealth** (atau LeakManager)
- Inspector → LeakManager component
- **Max Health** = `20` (atau sesuai keinginan)

### **5. PLAY!**
- Press **Play** button
- Sistem akan auto-create:
  - ✅ Wave Announcement UI
  - ✅ Game Over UI (Victory/Defeat)
  - ✅ Game Managers (GameManager, PlasmaManager, WaveManager, PlayerHealth)

---

## 🎯 SISTEM YANG SUDAH JALAN OTOMATIS:

| **Sistem** | **Status** | **Fitur** |
|-----------|-----------|-----------|
| **Wave System** | ✅ Auto | Wave 1→2→3→4→5 otomatis mulai dengan announcement |
| **Wave Announcement** | ✅ Auto | Popup "WAVE 1", "WAVE 2" dengan fade in/out |
| **Player Health** | ✅ Auto | Health berkurang saat enemy sampai target |
| **Enemy Damage** | ✅ Fleksibel | Bisa set damage berbeda per enemy type |
| **Victory Condition** | ✅ Auto | Menang jika semua wave selesai |
| **Defeat Condition** | ✅ Auto | Kalah jika health = 0 |
| **Victory Screen** | ✅ Auto | Menampilkan stats + button Restart/Menu |
| **Defeat Screen** | ✅ Auto | Menampilkan stats + button Restart/Menu |

---

## 🔧 SETTINGS YANG BISA DI-ADJUST:

### **A. Wave Settings** (WaveManager)
- **Wave Announcement Duration**: Berapa lama text "WAVE X" muncul
- **Auto Start Waves**: Toggle auto/manual start
- **Waves List**: Tambah/kurang jumlah wave

### **B. Player Health** (LeakManager)
- **Max Health**: Total health player (default 20)

### **C. Enemy Damage** (EnemyDataSO)
- **Damage To Cell**: Damage yang diberikan enemy ke player
  - Common Virus: 1
  - Mutant Virus: 2
  - Boss: 5 (contoh)

### **D. Wave Data** (WaveDataSO)
- **Spawn Groups**: Count, interval, delay
- **Enemy Data**: Jenis enemy yang spawn
- **Completion Bonus**: Plasma reward per wave

---

## 📊 GAMEPLAY FLOW:

```
START GAME
    ↓
"WAVE 1" announcement (2.5s)
    ↓
Spawn enemies
    ↓
Player build & defend
    ↓
All enemies killed/reached?
    ↓
Wait 3s → "WAVE 2" announcement
    ↓
... repeat ...
    ↓
WAVE 5 complete → "VICTORY!" screen
   OR
Health = 0 → "DEFEAT" screen
```

---

## 🐛 TROUBLESHOOTING:

**Q: Wave tidak auto start?**
- Check WaveManager → Auto Start Waves = TRUE
- Check Waves List tidak kosong

**Q: Enemy tidak damage player?**
- Check EnemyDataSO → Damage To Cell > 0
- Check LeakManager exist di scene

**Q: UI tidak muncul?**
- Check GameSceneSetup.cs ada di project
- Check Console untuk error messages

**Q: Health tidak update di HUD?**
- Check HUD GameObject ada di scene
- Check HUDController component ter-assign Text references

---

## ✨ YANG SUDAH DIBUAT:

### **Scripts Baru:**
1. ✅ `GameSceneSetup.cs` - Auto-create semua UI & managers
2. ✅ `WaveAnnouncementUI.cs` - Wave popup text
3. ✅ `GameOverUI.cs` - Victory/Defeat screens

### **Scripts Updated:**
1. ✅ `LeakManager.cs` - Player health system dengan flexible damage
2. ✅ `WaveManager.cs` - Auto wave start + announcement
3. ✅ `EnemyBase.cs` - Flexible damage ke player
4. ✅ `HUDController.cs` - Display health/wave/plasma

---

## 🎮 READY TO PLAY!

1. **Assign Wave Data** di WaveManager
2. **Set Enemy Damage** di EnemyDataSO
3. **Press Play!**
4. Game akan auto-setup semua UI dan mulai wave 1!

**Tinggal polishing aja sekarang! 🚀**
