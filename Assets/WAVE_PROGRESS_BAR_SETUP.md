# Wave Progress Bar - Setup & Integration Guide

## QUICK SUMMARY

**What Changed:**
- Old: Text-based wave indicator (e.g., "WAVE 2 / 4")
- New: Visual progress bar with virus character as position marker
- Location: Same place as old wave text in TowerShop UI
- Data source: Still WaveManager.currentWaveIndex and TotalWaves (no breaking changes)

**Files Modified:**
- `Assets/Scripts/UI/TowerShopUI.cs` - Added waveProgressBar reference, updated UpdateWave() method
- `Assets/Scripts/UI/WaveProgressBar.cs` - NEW script (complete progress bar implementation)

**Files Added:**
- `Assets/Scripts/UI/WaveProgressBarTest.cs` - Test script for iteration through waves every 2 seconds

**Files Deleted:**
- None (old wave text field remains optional via showWaveNumber toggle)

**Sprite Assets Used:**
- `Assets/Assets/Assets/progressBar-20261030T043021Z-1-001/progressBar/backBar.png` (400x100)
- `Assets/Assets/Assets/progressBar-20261030T043021Z-1-001/progressBar/frontBar.png` (400x100)
- `Assets/Assets/Assets/progressBar-20261030T043021Z-1-001/progressBar/virus.png` (80x64)

---

## UNITY EDITOR SETUP STEPS

### Step 1: Create the UI Hierarchy

In your TowerShop scene Canvas, create this hierarchy at the location where the old wave text was:

```
Canvas
└── WaveProgressBar (empty GameObject)
    ├── BackBar (Image)
    ├── FrontBar (Image)
    └── Virus (Image)
```

**Create BackBar:**
1. Right-click on WaveProgressBar → UI → Image
2. Name: `BackBar`
3. Component: Image
   - Source Image: backBar (from progressbar folder)
   - Preserve Aspect: ✓
4. RectTransform:
   - Anchors: Middle Center
   - Pivot: (0.5, 0.5)
   - Position: (0, 0)
   - Size: 400, 100 (or use "Apply Sizes From Sprites" button later)

**Create FrontBar:**
1. Right-click on WaveProgressBar → UI → Image
2. Name: `FrontBar`
3. Component: Image
   - Source Image: frontBar
   - Image Type: Filled
   - Fill Method: Horizontal
   - Fill Origin: Left
   - Preserve Aspect: ✓
4. RectTransform:
   - Same as BackBar (400, 100)
   - Anchor: Middle Center
   - Pivot: (0.5, 0.5)

**Create Virus:**
1. Right-click on WaveProgressBar → UI → Image
2. Name: `Virus`
3. Component: Image
   - Source Image: virus
   - Preserve Aspect: ✓
4. RectTransform:
   - Anchor: Middle Center
   - Pivot: (0.5, 0.5)
   - Size: 80, 64 (or auto-calculated by script)
5. Make sure Virus is LAST in hierarchy (rendered on top)

### Step 2: Sprite Import Settings

For each sprite in the progressbar folder (backBar.png, frontBar.png, virus.png):
1. Select the sprite in Assets
2. Inspector → Texture Type: Sprite (2D and UI)
3. Sprite Mode: Single
4. Pixels Per Unit: 100
5. Filter Mode: Point (no filter) or Bilinear
6. Apply

### Step 3: Add WaveProgressBar Script

1. Select WaveProgressBar GameObject
2. Add Component → WaveProgressBar (search by name)
3. Assign in Inspector:
   - Front Bar: Drag FrontBar (Image component)
   - Back Bar: Drag BackBar (RectTransform)
   - Virus: Drag Virus (Image component)
4. Configure Optional Settings:
   - Bar Scale: 1 (default, 0.5 = half size, 2 = double size)
   - Virus Scale: 1
   - Keep Virus Inside Bar: ✓ (prevents virus from going outside bar edges)
   - Show Wave Number: ☐ (unchecked = no text, ✓ = show "WAVE X / Y")
   - Smooth Duration: 0.5s (animation time)
   - Use Idle Bob: ✓ (virus bobs up and down)
   - Bob Amount: 0.1
   - Bob Speed: 2
   - Use Pop Effect: ✓ (virus scales up when wave changes)
   - Pop Duration: 0.3s
   - Pop Scale: 1.2x

5. Click "Apply Sizes From Sprites" button (ContextMenu)
   - This auto-calculates all RectTransform sizes based on sprite dimensions and scales

### Step 4: Update TowerShopUI Script Assignment

1. Select the TowerShop UI Canvas (or the object with TowerShopUI component)
2. In Inspector, find TowerShopUI script
3. Scroll to "Wave Progress Bar" section
4. Drag WaveProgressBar GameObject into `waveProgressBar` field
5. Optional: Drag the old wave text (waveText) into `waveNumberText` field if you want to show numbers
6. Save scene

### Step 5: Canvas Scaler (Optional but Recommended)

Ensure your Canvas has consistent scaling:
1. Select Canvas
2. Canvas Scaler component:
   - UI Scale Mode: Scale With Screen Size
   - Reference Resolution: 1920 x 1080 (or your target)
3. This ensures the progress bar scales consistently across all screen sizes

---

## PROGRESS FORMULA

```
currentWave = 1, totalWave = 4
progress = (currentWave - 1) / (totalWave - 1)

Wave 1 → progress = 0/3 = 0    (bar empty, virus at left)
Wave 2 → progress = 1/3 = 0.33 (bar 1/3 full)
Wave 3 → progress = 2/3 = 0.67 (bar 2/3 full)
Wave 4 → progress = 3/3 = 1    (bar full, virus at right)

Edge case: totalWave <= 1 → progress = 1 (always full)
```

---

## INTEGRATION WITH TOWERSHOPUI

### Old Code Flow:
```
WaveManager.OnWaveStatusChanged event
  → TowerShopUI.UpdateWave(current, total)
    → waveText.text = "WAVE 2 / 4"
```

### New Code Flow:
```
WaveManager.OnWaveStatusChanged event
  → TowerShopUI.UpdateWave(current, total)
    → waveProgressBar.SetWave(current, total)
      → Update FrontBar.fillAmount
      → Position Virus at correct X
      → Optional: update waveNumberText if assigned
```

**No changes needed to WaveManager or other systems.** TowerShopUI.UpdateWave() is the only integration point, and it's already updated.

---

## TESTING WITH WAVEPROGESSBARTEST

To test the progress bar independently:

1. Create an empty GameObject in your scene
2. Add component: WaveProgressBarTest
3. In Inspector, assign:
   - waveProgressBar: Your WaveProgressBar GameObject
4. Set updateInterval: 2 (seconds between wave changes)
5. Play the game
6. Watch the bar fill and virus move every 2 seconds, cycling through waves 1→2→3→4→1...

Output in Console:
```
[WaveProgressBarTest] Started at Wave 1/4
[WaveProgressBarTest] Updated to Wave 2/4
[WaveProgressBarTest] Updated to Wave 3/4
[WaveProgressBarTest] Updated to Wave 4/4
[WaveProgressBarTest] Reset to Wave 1
```

---

## OPTIONAL FEATURES

### Disable Animation (Instant Updates)

In code or Inspector:
```csharp
waveProgressBar.SetWaveInstant(currentWave, totalWaves); // No animation
```

### Disable Idle Bob

Inspector: Use Idle Bob = ☐ (unchecked)

### Disable Pop Effect

Inspector: Use Pop Effect = ☐ (unchecked)

### Listen to "Reached Final Wave" Event

```csharp
waveProgressBar.OnReachedFinalWave += () => {
    Debug.Log("Player reached the final wave!");
};
```

### Show Wave Number Alongside Bar

Inspector: Show Wave Number = ✓, then assign a TMP_Text to waveNumberText field in TowerShopUI

---

## TROUBLESHOOTING

| Issue | Solution |
|-------|----------|
| Virus not visible | Check Z position; ensure Virus is last in hierarchy. Check Image component has virus sprite assigned. |
| Progress bar not filling | Check FrontBar Image Type = Filled, Fill Method = Horizontal, Fill Origin = Left. |
| Virus stuck on left | Check BackBar and FrontBar are aligned (same position and size). |
| Sprites look pixelated | Check Texture Type = Sprite (2D and UI), Filter Mode = Point or Bilinear. |
| Virus position wrong after resizing | Click "Apply Sizes From Sprites" button on WaveProgressBar component. |
| Wave updates not reflected | Check WaveManager is calling UpdateWave(), check waveProgressBar field is assigned in TowerShopUI. |

---

## ASSET MEASUREMENTS (REFERENCE)

| Asset | Dimensions | Aspect | PPU | Notes |
|-------|-----------|--------|-----|-------|
| backBar.png | 400 x 100 px | 4:1 | 100 | Dark gray track |
| frontBar.png | 400 x 100 px | 4:1 | 100 | Red fill bar |
| virus.png | 80 x 64 px | 1.25:1 | 100 | Blue character, ~5px transparent margin |

Virus-to-bar ratio: 64/100 = 0.64x (virus is 64% of bar height)

---

## SCRIPT REFERENCE

### WaveProgressBar.cs Public API

```csharp
// Set wave with smooth animation
public void SetWave(int currentWaveIndex, int totalWaveCount)

// Set wave instantly (no animation)
public void SetWaveInstant(int currentWaveIndex, int totalWaveCount)

// Event: fired when reaching final wave
public event System.Action OnReachedFinalWave

// Apply sprite sizes (manual call if needed)
public void ApplySizesFromSprites()
```

### WaveProgressBar.cs Inspector Fields

- frontBar (Image): The fill bar
- backBar (RectTransform): The background track
- virus (Image): The position marker
- barScale (float): Scale multiplier for bar size
- virusScale (float): Scale multiplier for virus size
- keepVirusInsideBar (bool): Clamp virus within bar bounds
- showWaveNumber (bool): Reserved for future use
- smoothDuration (float): Animation time in seconds
- useIdleBob (bool): Enable bobbing animation
- bobAmount (float): How far virus bobs up/down
- bobSpeed (float): How fast virus bobs
- usePopEffect (bool): Scale pop when wave changes
- popDuration (float): Pop animation time
- popScale (float): Pop scale multiplier

---

## COMPLETE INTEGRATION CHECKLIST

- [ ] Sprite Import Settings applied (Type = Sprite (2D and UI), PPU = 100)
- [ ] UI Hierarchy created (BackBar, FrontBar, Virus under WaveProgressBar)
- [ ] BackBar and FrontBar aligned and same size
- [ ] Virus pivot set to (0.5, 0.5)
- [ ] WaveProgressBar script added to WaveProgressBar GameObject
- [ ] All three Image fields assigned (frontBar, backBar, virus)
- [ ] TowerShopUI.waveProgressBar field assigned
- [ ] "Apply Sizes From Sprites" clicked
- [ ] Canvas Scaler configured (optional)
- [ ] Test: Wave changes smoothly animate the bar and virus
- [ ] Test: Virus bobs idly when no wave change
- [ ] Test: Pop effect plays when wave increases
