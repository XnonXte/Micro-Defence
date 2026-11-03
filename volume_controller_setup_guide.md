# Volume Controller Setup Guide

Panduan lengkap untuk membuat UI slider BGM dan SFX volume di Settings menu.

---

## 📦 Files Created

- **VolumeController.cs** - Script untuk control BGM & SFX volume via sliders

---

## 🎮 Features

✅ **BGM Volume Slider** - Control background music volume
✅ **SFX Volume Slider** - Control sound effects volume (termasuk button SFX!)
✅ **Auto Save** - Volume settings disimpan otomatis di PlayerPrefs
✅ **Auto Load** - Volume settings di-load saat game start
✅ **Preview Sound** - Play sound saat adjust SFX volume
✅ **Percentage Display** - Show volume as percentage (75%) atau decimal (0.75)
✅ **Mute/Unmute** - Optional mute buttons

---

## 🚀 Setup di Unity

### Step 1: Create UI Elements

Di Settings Panel, buat UI hierarchy:

```
Settings Panel
├── BGM Volume Section
│   ├── BGM Label (Text: "BGM Volume")
│   ├── BGM Slider (Slider component)
│   └── BGM Value Text (Text: "100%")
│
└── SFX Volume Section
    ├── SFX Label (Text: "SFX Volume")
    ├── SFX Slider (Slider component)
    └── SFX Value Text (Text: "100%")
```

### Step 2: Create Sliders

**BGM Slider:**
1. Right-click Settings Panel → UI → Slider
2. Rename: "BGM Slider"
3. Configure:
   ```
   Min Value: 0
   Max Value: 1
   Whole Numbers: FALSE
   Value: 1
   ```

**SFX Slider:**
1. Right-click Settings Panel → UI → Slider
2. Rename: "SFX Slider"
3. Configure sama seperti BGM Slider

### Step 3: Add VolumeController Script

1. **Create Empty GameObject:**
   - Right-click Settings Panel → Create Empty
   - Rename: "Volume Controller"

2. **Add Script:**
   - Select Volume Controller GameObject
   - Add Component → VolumeController

3. **Assign References:**
   ```
   Volume Sliders:
   - BGM Slider: [Drag BGM Slider]
   - SFX Slider: [Drag SFX Slider]
   
   Volume Text Labels (Optional):
   - BGM Value Text: [Drag BGM Value Text]
   - SFX Value Text: [Drag SFX Value Text]
   
   Settings:
   - Show Percentage: ✓ TRUE
   - Percent Format: "0" (untuk 75%) atau "0.0" (untuk 75.5%)
   ```

---

## 🎨 UI Layout Examples

### Option 1: Horizontal Layout (Simple)

```
[BGM Volume]  [━━━━━━━━━━] 75%
[SFX Volume]  [━━━━━━━━━━] 100%
```

### Option 2: Vertical with Labels

```
Background Music
[━━━━━━━━━━━━━━━━] 75%

Sound Effects
[━━━━━━━━━━━━━━━━] 100%
```

### Option 3: With Mute Buttons

```
BGM  [━━━━━━━━━━] 75%  [Mute]
SFX  [━━━━━━━━━━] 100% [Mute]
```

---

## 🎯 Complete Settings Panel Setup

```
Settings Panel (full screen or popup)
│
├── Title (Text: "SETTINGS")
│
├── Audio Section
│   ├── Section Header (Text: "AUDIO")
│   │
│   ├── BGM Volume
│   │   ├── Label (Text: "Background Music")
│   │   ├── Slider
│   │   └── Value (Text: "100%")
│   │
│   └── SFX Volume
│       ├── Label (Text: "Sound Effects")
│       ├── Slider
│       └── Value (Text: "100%")
│
├── Graphics Section (Optional)
│   ├── Section Header (Text: "GRAPHICS")
│   ├── Quality Dropdown
│   └── Fullscreen Toggle
│
└── Buttons
    ├── Reset to Default Button
    ├── Apply Button
    └── Back Button
```

---

## 📋 Slider Configuration

### Unity Slider Component Settings:

**Handle Slide Area:**
```
Left: 0
Right: Fill Area width
```

**Fill Area:**
```
Color: Primary color (misal biru/hijau)
Sprite: Default slider fill atau custom
```

**Handle:**
```
Sprite: Circle atau custom handle
Size: 20-30 pixels
```

**Background:**
```
Color: Dark gray (untuk contrast)
Sprite: Rounded rectangle
```

---

## 🎵 How It Works

### Volume Flow:

1. **User drags slider** (misal BGM ke 75%)
2. **VolumeController.OnBGMVolumeChanged()** dipanggil
3. **AudioManager.SetBGMVolume(0.75)** dieksekusi
4. **AudioManager** update volume + save ke PlayerPrefs
5. **Text label** update jadi "75%"

### Auto Load on Game Start:

1. Game start
2. VolumeController.Start() load dari PlayerPrefs
3. Slider di-set ke saved value
4. AudioManager apply volume

### SFX Preview:

Saat user adjust SFX slider:
- Auto play button click sound
- User bisa dengar hasilnya real-time
- Semua SFX (button hover/click, shooting, dll) akan terpengaruh

---

## 🔧 Additional Features

### Reset to Default Button:

1. **Create Button** di Settings Panel
2. **Add onClick Event:**
   - Select Button
   - Inspector → Button → On Click()
   - Drag Volume Controller GameObject
   - Function: VolumeController → ResetToDefault()

### Mute/Unmute Toggle:

1. **Create Toggle** atau Button
2. **Add Event:**
   ```csharp
   // Untuk BGM Mute Toggle:
   Toggle.onValueChanged → VolumeController.MuteBGM() / UnmuteBGM()
   
   // Untuk SFX Mute Toggle:
   Toggle.onValueChanged → VolumeController.MuteSFX() / UnmuteSFX()
   ```

---

## 🎨 Styling Tips

### Slider Colors:

**BGM Slider:**
```
Fill: Blue (#2196F3)
Handle: White
Background: Dark Gray (#333333)
```

**SFX Slider:**
```
Fill: Green (#4CAF50)
Handle: White
Background: Dark Gray (#333333)
```

### Text Labels:

```
Font: Bold
Size: 18-24
Color: White
Alignment: Center
```

### Value Display:

```
Font: Regular
Size: 16-20
Color: Light Gray or White
Format: "75%" atau "0.75"
```

---

## ✅ Testing Checklist

- [ ] BGM slider adjust music volume
- [ ] SFX slider adjust sound effects volume
- [ ] Button hover/click sounds terpengaruh SFX slider
- [ ] Volume persists setelah restart game
- [ ] Text label update real-time
- [ ] Preview sound play saat adjust SFX
- [ ] Reset button set ke 100%
- [ ] Slider visual smooth dan responsive

---

## 🐛 Troubleshooting

### Volume tidak berubah:

**Check:**
- AudioManager.Instance ada di scene?
- Slider assigned di VolumeController?
- Slider onValueChanged event working?

**Fix:**
- Ensure AudioManager GameObject exists
- Re-assign slider references
- Check Console for errors

### Volume tidak save:

**Check:**
- PlayerPrefs write permission?
- AudioManager.SetBGMVolume() dipanggil?

**Fix:**
- Check platform PlayerPrefs support
- Verify AudioManager save logic (line 143, 157)

### Preview sound tidak play:

**Check:**
- AudioManager ada audio clips?
- SFX volume > 0?

**Fix:**
- Assign buttonClick audio di AudioManager
- Set SFX slider value > 0

---

## 💡 Pro Tips

1. **Volume Curve:** Untuk feel yang lebih natural, bisa pakai exponential curve:
   ```csharp
   float linearVolume = slider.value;
   float curvedVolume = linearVolume * linearVolume; // Quadratic
   ```

2. **Volume Indicators:** Tambahkan icon (🔊 loud, 🔉 medium, 🔇 mute) based on volume

3. **Haptic Feedback:** Untuk mobile, add haptic saat adjust slider

4. **Visual Feedback:** Slider fill color berubah based on value (red → yellow → green)

---

## 📱 Platform Notes

### PC/Mac:
- Slider control with mouse drag
- Keyboard arrows untuk fine control (optional)

### Mobile:
- Touch drag slider
- Larger handle untuk easier touch
- Consider vertical sliders untuk thumb reach

### Console:
- D-pad/joystick untuk adjust
- Visual indicator saat selected

---

## 🎯 Integration Points

Volume settings affect:

✅ **BGM:**
- Main Menu music
- Level 1-5 music
- Level 6-10 music

✅ **SFX:**
- Button hover/click (UIButtonSFX)
- Tower shooting sounds
- Enemy hit sounds
- Plasma pickup
- Game over/win sounds
- Boss grunt
- ALL PlaySFX() calls

---

**Setup complete!** 🎉

Settings panel sekarang punya volume control yang lengkap, auto-save, dan user-friendly!
