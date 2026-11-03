# Button SFX Setup Guide

Panduan lengkap untuk menambahkan sound effect (hover & click) ke semua button UI di game.

---

## 📦 Files Created

1. **UIButtonSFX.cs** - Component yang auto-play SFX saat hover dan click
2. **AddButtonSFXUtility.cs** - Editor tool untuk batch add SFX ke semua button

---

## 🎮 Buttons Yang Akan Diberi SFX

### Main Menu:
- Play button
- Settings button
- Credits button
- Exit button

### Pause Panel:
- Resume button
- Retry button
- Exit button

### Game Over Panel:
- Retry button
- Menu button

### Game Win Panel:
- Retry button
- Menu button
- Next Level button

### Gameplay UI:
- Start Wave button
- Tower cards (B-Cell, Macrophage, T-Cell)
- Upgrade buttons

---

## 🚀 Quick Setup (Recommended)

### Method 1: Automatic (All Buttons at Once)

1. **Open Scene** yang ingin diberi SFX (MainMenu, Level 1, dll)

2. **Di Unity Menu Bar** → `Tools` → `Micro Defense` → `Add Button SFX to All Buttons`

3. **Dialog akan muncul** menunjukkan berapa button yang sudah diberi SFX

4. **Done!** Semua button di scene sekarang punya SFX

5. **Repeat untuk scene lain:**
   - Open MainMenu scene → run tool
   - Open Level 1 scene → run tool
   - Open Level 2 scene → run tool
   - dst.

---

## 🛠️ Manual Setup (Per Button)

Jika Anda ingin kontrol lebih detail atau test satu button dulu:

1. **Select Button** di Hierarchy
   - Contoh: Pause Panel → Retry Button

2. **Add Component:**
   - Inspector → Add Component
   - Search: `UIButtonSFX`
   - Add!

3. **Configure (Optional):**
   ```
   Play Hover Sound: ✓ TRUE
   Play Click Sound: ✓ TRUE
   Hover Volume: 1.0
   Click Volume: 1.0
   ```

4. **Test:**
   - Play scene
   - Hover mouse ke button → hear hover sound
   - Click button → hear click sound

---

## 🎵 Audio Setup

Pastikan AudioManager sudah punya audio clips:

1. **Open AudioManager GameObject** (biasanya di scene atau DontDestroyOnLoad)

2. **Di Inspector → AudioManager Component:**
   ```
   SFX Clips:
   - Button Hover: [Assign audio clip]
   - Button Click: [Assign audio clip]
   ```

3. **Audio Clips:**
   - Import button hover sound → `Assets/Audio/SFX/`
   - Import button click sound → `Assets/Audio/SFX/`
   - Drag ke AudioManager fields

---

## 🔧 Editor Tools

Di Unity Menu: `Tools` → `Micro Defense`

### 1. **Add Button SFX to All Buttons**
   - Menambahkan UIButtonSFX ke semua button di scene
   - Skip button yang sudah punya component
   - Show dialog hasil (berapa button processed)

### 2. **Remove Button SFX from All Buttons**
   - Hapus UIButtonSFX dari semua button
   - Berguna jika mau reset atau ada masalah

### 3. **List All Buttons in Scene**
   - List semua button di scene
   - Show status: [HAS SFX] atau [NO SFX]
   - Berguna untuk debug

---

## 📋 Step-by-Step for All Scenes

### Scene 1: MainMenu
```
1. Open MainMenu scene
2. Tools → Micro Defense → Add Button SFX to All Buttons
3. Save scene
```

### Scene 2: Level 1
```
1. Open Level 1 scene
2. Tools → Micro Defense → Add Button SFX to All Buttons
3. Save scene
```

### Scene 3-5: Other Levels
```
Repeat untuk setiap level scene
```

### Prefabs (GameOverUI, PauseMenuUI)
```
1. Open prefab di Prefab Mode
2. Select button → Add Component → UIButtonSFX
3. Apply changes
4. Atau: Instantiate prefab di scene, run tool, Apply to prefab
```

---

## ✅ Verification

Setelah setup, verify dengan:

1. **List All Buttons:**
   - Tools → Micro Defense → List All Buttons
   - Check semua button [HAS SFX]

2. **Test In-Game:**
   - Play scene
   - Hover setiap button → should hear hover sound
   - Click setiap button → should hear click sound

---

## 🎨 Customization

### Per-Button Volume:

Di Inspector UIButtonSFX component:
```
Hover Volume: 0.5  (quieter hover)
Click Volume: 1.0  (normal click)
```

### Disable SFX for Specific Button:

Di Inspector UIButtonSFX component:
```
Play Hover Sound: FALSE  (no hover sound)
Play Click Sound: TRUE   (still has click sound)
```

### Different Sounds Per Button:

Jika ingin button tertentu pakai sound berbeda:
1. Duplicate UIButtonSFX script
2. Rename jadi `UIButtonSFXCustom`
3. Hardcode different AudioManager method
4. Add ke button yang mau pakai sound berbeda

---

## 🐛 Troubleshooting

### No Sound Playing:

**Check:**
1. AudioManager.Instance ada di scene?
2. Audio clips assigned di AudioManager?
3. Button interactable? (disabled button = no sound)
4. SFX volume tidak mute?

**Fix:**
- Check AudioManager GameObject exists
- Assign buttonHover & buttonClick clips
- Enable button if disabled
- Check volume settings

### Sound Playing Twice:

**Cause:** Button punya 2 event yang play sound

**Fix:**
- Check onClick event di button
- Remove duplicate AudioManager.PlayButtonClick() call
- Let UIButtonSFX handle it automatically

### Tool Menu Not Showing:

**Cause:** Script di folder salah atau compile error

**Fix:**
- Ensure `AddButtonSFXUtility.cs` di folder `Assets/Scripts/UI/Editor/`
- Check Console for compile errors
- Reimport script (right-click → Reimport)

---

## 📊 Expected Results

After setup, all buttons should:

✅ Play hover sound saat mouse enter
✅ Play click sound saat di-click
✅ No sound saat button disabled
✅ Consistent audio experience across all UI

---

## 🎯 Scene Coverage

Apply to these scenes:

- [x] MainMenu
- [x] Level 1
- [x] Level 2
- [x] Level 3
- [x] Level 4
- [x] Level 5
- [x] GameOver Prefab
- [x] Pause Prefab
- [x] Any other level scenes

---

## 💡 Pro Tips

1. **Batch Setup:** Open all scenes in Hierarchy (multi-scene editing) → run tool once
2. **Prefab Power:** Setup di prefab → auto apply ke semua instances
3. **Volume Balance:** Hover sound lebih quiet (0.5-0.7), click sound normal (1.0)
4. **Audio Polish:** Use subtle UI sounds, tidak terlalu loud/annoying

---

## 🎵 Recommended Audio

### Button Hover:
- Short, soft "whoosh" atau "tick"
- Duration: 0.1-0.2 seconds
- Volume: Subtle, tidak dominant

### Button Click:
- Crisp "click" atau "tap"
- Duration: 0.1-0.3 seconds
- Volume: Clear but not jarring

---

**Setup complete!** 🎉

All buttons sekarang punya SFX hover & click yang konsisten dan professional!
