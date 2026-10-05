# 🎨 SETUP UI CANVAS & BUTTONS - MANUAL GUIDE

Panduan step-by-step membuat Canvas + UI buttons agar game bisa dimainkan.

---

## 📋 APA YANG PERLU DIBUAT

```
Canvas (Screen Space - Overlay)
├─ HUD Panel (Top)
│  ├─ Plasma Text
│  ├─ Wave Text
│  └─ Leaks Text
└─ Shop Panel (Bottom)
   ├─ B-Cell Button
   ├─ T-Cell Button
   ├─ Macrophage Button
   └─ Start Wave Button
```

---

## 🛠️ LANGKAH-LANGKAH MEMBUAT CANVAS

### STEP 1: Create Canvas

1. Di Hierarchy, **Right-click** → **UI** → **Canvas**
   - Canvas akan muncul di Hierarchy
   - Otomatis create EventSystem juga

2. Select Canvas, di Inspector:
   - Canvas component → **Render Mode**: `Screen Space - Overlay`
   - RectTransform → Anchor: `Stretch-Stretch` (full screen)

### STEP 2: Create HUD Panel (Top Panel)

1. Select Canvas
2. **Right-click** → **UI** → **Panel - Image**
   - Beri nama: **`HUD Panel`**

3. Di Inspector HUD Panel, set RectTransform:
   ```
   Anchors: Top-Stretch (atas, full width)
   Pos Y: -30 (offset dari top)
   Height: 60
   ```

4. Set Image color ke semi-transparent black:
   ```
   Image → Color: (0, 0, 0, 0.8)
   ```

### STEP 3: Add HUD Texts (3 texts)

Di HUD Panel, **Right-click 3x** → **UI** → **Text (Legacy)**

**Text #1: Plasma**
- Name: `Plasma Text`
- Text: `Plasma: 300`
- Font Size: 24
- Color: White
- Alignment: Middle Left
- Position: (-400, 0)
- Width: 250

**Text #2: Wave**
- Name: `Wave Text`
- Text: `Wave: 1/1`
- Font Size: 24
- Color: White
- Alignment: Middle Center
- Position: (0, 0)
- Width: 250

**Text #3: Leaks**
- Name: `Leaks Text`
- Text: `Leaks: 0/5`
- Font Size: 24
- Color: White
- Alignment: Middle Right
- Position: (400, 0)
- Width: 250

### STEP 4: Create Shop Panel (Bottom Panel)

1. Select Canvas
2. **Right-click** → **UI** → **Panel - Image**
   - Beri nama: **`Shop Panel`**

3. Di Inspector Shop Panel, set RectTransform:
   ```
   Anchors: Bottom-Stretch (bawah, full width)
   Height: 70
   ```

4. Set Image color:
   ```
   Image → Color: (0.2, 0.2, 0.2, 0.9)
   ```

### STEP 5: Add Shop Buttons (4 buttons)

Di Shop Panel, **Right-click 4x** → **UI** → **Button - TextMeshPro** (atau Button - Legacy)

**Button #1: B-Cell**
- Name: `B-Cell Button`
- Text: `B-Cell`
- Font Size: 24
- Position: (-300, 0)
- Width: 120
- Height: 50
- Color: Light Blue (0.2, 0.6, 0.8, 1)

**Button #2: T-Cell**
- Name: `T-Cell Button`
- Text: `T-Cell`
- Font Size: 24
- Position: (-100, 0)
- Width: 120
- Height: 50
- Color: Light Blue (0.2, 0.6, 0.8, 1)

**Button #3: Macrophage**
- Name: `Macrophage Button`
- Text: `Macrophage`
- Font Size: 20
- Position: (100, 0)
- Width: 140
- Height: 50
- Color: Light Blue (0.2, 0.6, 0.8, 1)

**Button #4: Start Wave**
- Name: `Start Wave Button`
- Text: `START WAVE`
- Font Size: 24
- Position: (300, 0)
- Width: 140
- Height: 50
- Color: Green (0.2, 0.8, 0.2, 1)

---

## 🔗 WIRE UP BUTTONS KE SCRIPTS

### STEP 1: Add TowerShopUI Component

1. Select **Canvas** di Hierarchy
2. Di Inspector, **Add Component** → cari `TowerShopUI`
3. Script akan ditambahkan

### STEP 2: Assign Tower Data

Di Canvas → TowerShopUI component:

```
B Cell Data: [Drag B Cell.asset dari Assets/Data/TowerData/]
T Cell Data: [Drag T Cell.asset dari Assets/Data/TowerData/]
Macrophage Data: [Drag Macrophage cell.asset dari Assets/Data/TowerData/]
```

### STEP 3: Assign Button References

Di Canvas → TowerShopUI component:

```
B Cell Button: [Drag B-Cell Button dari Hierarchy]
T Cell Button: [Drag T-Cell Button dari Hierarchy]
Macrophage Button: [Drag Macrophage Button dari Hierarchy]
Start Wave Button: [Drag Start Wave Button dari Hierarchy]
```

---

## ✅ VERIFY SETUP

Setelah semuanya, hierarchy harus kayak gini:

```
Canvas
├─ HUD Panel
│  ├─ Plasma Text
│  ├─ Wave Text
│  └─ Leaks Text
├─ Shop Panel
│  ├─ B-Cell Button
│  │  └─ Text
│  ├─ T-Cell Button
│  │  └─ Text
│  ├─ Macrophage Button
│  │  └─ Text
│  └─ Start Wave Button
│     └─ Text
└─ EventSystem
```

---

## 🎮 TEST SETUP

1. **Play game**
2. **Console** - Sekarang harus ada log dari TowerShopUI:
   ```
   [TowerShopUI] Start() called
   [TowerShopUI] bCellButton=B-Cell Button, startWaveButton=Start Wave Button
   [TowerShopUI] bCellButton listener added
   [TowerShopUI] tCellButton listener added
   [TowerShopUI] macrophageButton listener added
   [TowerShopUI] startWaveButton listener added
   ```

3. **Klik Start Wave button**
4. **Console** - Harus ada log:
   ```
   [TowerShopUI.OnStartWaveClicked] Called
   [TowerShopUI.OnStartWaveClicked] WaveManager.Instance=_Managers
   [TowerShopUI.OnStartWaveClicked] Calling WaveManager.StartNextWave()
   [WaveManager] StartNextWave called...
   [WaveManager] SpawnWaveRoutine started...
   [WaveManager] Spawning enemy 1/5
   ...
   ```

5. **Hierarchy** - Harus ada virus Clone muncul:
   ```
   Common Virus(Clone)
   Common Virus(Clone)
   Common Virus(Clone)
   ...
   ```

---

## 🎨 VISUAL LAYOUT

```
┌─────────────────────────────────────────────────┐
│  Plasma: 300          Wave: 1/1      Leaks: 0/5 │  ← HUD Panel (Top)
├─────────────────────────────────────────────────┤
│                      (GAME AREA)                 │
│                                                   │
│                                                   │
├─────────────────────────────────────────────────┤
│  [B-Cell]  [T-Cell]  [Macrophage]  [START WAVE]  │  ← Shop Panel (Bottom)
└─────────────────────────────────────────────────┘
```

---

## 🐛 TROUBLESHOOTING

| Problem | Solution |
|---------|----------|
| Button tidak responsive | Cek apakah Canvas punya EventSystem child |
| Text tidak muncul | Canvas → RenderMode harus `Screen Space - Overlay` |
| Button text terlalu kecil | Increase font size di Text component |
| Layout berantakan | Adjust RectTransform anchors & sizing |
| Button OnClick tidak trigger | Cek apakah button wired ke TowerShopUI fields |

---

## 📝 SUMMARY

Setelah setup ini complete:

- ✅ Canvas created dengan HUD + Shop panels
- ✅ 3 HUD texts untuk Plasma, Wave, Leaks
- ✅ 4 buttons untuk tower selection + start wave
- ✅ TowerShopUI script attached ke Canvas
- ✅ All buttons wired ke script
- ✅ Tower data assigned
- ✅ Ready to play!

Lanjut test sesuai TEST SETUP section di atas.
