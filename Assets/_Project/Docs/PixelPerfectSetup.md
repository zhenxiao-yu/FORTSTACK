# Last Kernel — Visual & Rendering Setup Guide

**Unity:** 6000.4.3f1 · **Pipeline:** URP 17.4.0  
**Last updated:** 2026-04

---

## Overview

Last Kernel is a **3D isometric card-stacking game** rendered with URP. The visual quality strategy differs from traditional 2D pixel-art games:

- The game world is **3D** — cards sit on a physical board in world space.
- There is **no 2D Pixel Perfect Camera** (the package is not installed and would not be appropriate).
- "Crispness" comes from correct **texture filtering**, **URP anti-aliasing**, and **UI canvas scaling**, not a pixel grid lock.
- The primary target is **1920 × 1080 (16:9)**.

---

## 1. Target Resolution & Aspect Ratio

| Setting | Value |
|---------|-------|
| Target resolution | 1920 × 1080 |
| Aspect ratio | 16:9 (primary) |
| Supported range | 1280 × 720 → 2560 × 1440 |
| Ultrawide (21:9) | Supported with letterbox/pillarbox — verify menu layout |

Configured in: **Project Settings > Player > Resolution and Presentation**  
Default resolution is `1920 × 1080 (Windowed)`.

---

## 2. URP Configuration

**Assets:** `Assets/_Project/Settings/URP/`

| Asset | Purpose |
|-------|---------|
| `URP_Asset.asset` | Main URP pipeline asset — quality, shadows, HDR |
| `URP_Renderer.asset` | Renderer features (post-process, custom passes) |
| `URP_GlobalSettings.asset` | Shader stripping, global render settings |

### Recommended URP Settings for Card Crispness
In `URP_Asset.asset`, check:
- **Anti Aliasing (MSAA):** 2× or 4× for clean card edges in 3D
- **HDR:** Enabled (needed for bloom post-processing)
- **Render Scale:** 1.0 (never reduce for card games — pixel shimmer)

In `URP_Renderer.asset`:
- Ensure the custom `CustomPostProcessFeature` is configured correctly
- Do not add per-object motion blur (breaks card hover feel)

---

## 3. Camera Setup

The camera is controlled by `CameraController.cs` (Scripts/Core/).

### How It Works
- The camera **rig** moves in world space via SmoothDamp.
- Pan is driven by mouse drag; zoom by scroll wheel along the camera's forward vector.
- Movement bounds are driven by `Board.OnBoundsUpdated` at runtime.
- In scenes without a Board (e.g. Title), the fallback bounds from `CameraController._clampMin/_clampMax` are used.

### Inspector Fields (on the CameraController prefab)
| Field | Default | Notes |
|-------|---------|-------|
| Pan Speed | 0.01 | Scales further with camera height |
| Smooth Time | 0.15 | SmoothDamp time — lower = snappier |
| Pan Padding | 0.5 | World-unit buffer past board edge |
| Zoom Speed | 1.0 | Scroll multiplier |
| Min Distance | 5 | Closest zoom (from ground) |
| Max Distance | 20 | Furthest zoom |
| Clamp Min | (-10, -5) | Fallback pan min (no Board present) |
| Clamp Max | (10, 5) | Fallback pan max (no Board present) |

### CameraSettings Asset (Optional)
A `CameraSettings` ScriptableObject acts as the documentation/design record for these values.

**To create it:**
1. Right-click in the Project window
2. Select **Last Kernel > Camera Settings**
3. Save as `Assets/_Project/Settings/Default_Camera_Settings.asset`
4. Fill in the reference values to match the CameraController prefab

The CameraSettings asset is standalone documentation. CameraController does not read from it by default — values are set directly on the prefab component.

---

## 4. Canvas Scaler Setup

Every root **Canvas** in every scene must have a `CanvasScaler` configured for responsive scaling.

### Correct Settings
| Property | Value |
|----------|-------|
| UI Scale Mode | **Scale With Screen Size** |
| Reference Resolution | **1920 × 1080** |
| Screen Match Mode | **Match Width Or Height** |
| Match | **0.5** (balanced — works for both landscape and near-portrait) |

### How to Verify (Manual Unity Step)
1. Open each scene: `Title.unity`, `Main.unity`, `Island.unity`
2. Select each root Canvas in the Hierarchy
3. In the Inspector, confirm a `CanvasScaler` component is present with the above settings
4. If missing: Add Component → UI → Canvas Scaler, then configure as above

### World-Space Canvases
For UI pinned in world space (e.g. card stat labels, `ProgressUI`):
- Use **World Space** canvas mode
- Set a fixed pixel-per-unit density that matches your card's physical size in world units
- Do NOT add a CanvasScaler to world-space canvases

---

## 5. Sprite & Texture Import Settings

Card art textures should be set to:

| Setting | Value | Why |
|---------|-------|-----|
| Filter Mode | **Point (no filter)** | Keeps pixel art crisp at any zoom |
| Compression | **None** or **Lossless** | Prevents compression artifacts on sharp edges |
| Max Size | Match source (e.g. 256 × 256) | Don't upscale — let URP handle it |
| Read/Write Enabled | Off (unless needed) | Save memory |
| Mip Maps | **Off** for card art | Mip maps cause blurring at far zoom |
| Generate Mip Maps | Off | Same reason |

### For Background Art (larger textures)
- Filter Mode: **Bilinear** or **Trilinear** is acceptable
- Mip maps: On (reduces shimmer at far zoom)

### Checking Existing Textures
In the Project window, filter by Texture type. Sort by Filter Mode to quickly find any that are accidentally set to Bilinear.

---

## 6. TextMesh Pro (TMP) Font Settings

The project uses two font families:

| Font | Asset | Use |
|------|-------|-----|
| NotoSans SC | `Fonts/NatoSans/TMP/TMP_NotoSansSC_Fallback.asset` | CJK fallback characters |
| SmileySans | `Fonts/SmileySans/TMP/TMP_SmileySans_Display.asset` | Display / title text |

### Recommendations for Crisp TMP
- **Atlas Resolution:** 2048 × 2048 minimum for display fonts; 4096 × 4096 for body text with many glyphs.
- **Sampling Point Size:** Higher = smoother curves. 48–90pt for display; 36pt for body.
- **Padding:** 5–9 pixels — needed for outline/shadow materials.
- **Material Preset:** Use the **Distance Field** shader for scalable crisp edges at any size.
- **Font Asset Creator:** Re-bake fonts if visual artifacts appear after resolution changes.

### Chinese / CJK Text
- Simplified Chinese uses `TMP_NotoSansSC_Fallback.asset` as a fallback in the font stack.
- If CJK glyphs appear as boxes: verify the fallback is assigned in the TMP font asset's **Fallback Font Assets** list.
- TMPChineseFontBootstrap.cs handles runtime font injection — do not remove it.

---

## 7. Title / Menu Visual Guidelines

### Title Logo / Game Name
- If displayed as **TMP text**: use SmileySans Display font, Distance Field shader, large point size (72+).
- If displayed as a **Sprite/Image**: set Texture Filter Mode to **Point**, Preserve Aspect Ratio on the Image component.
- Anchor to center of Canvas — never use absolute pixel offsets for the title.

### Menu Buttons
- Use `TextButton` component (Scripts/UI/TextButton.cs) — it handles localization and hover state automatically.
- Button text must never be hardcoded strings; always route through `GameLocalization.Get(key)`.
- Layout: use a `Vertical Layout Group` for the button stack with controlled spacing and padding.
- Button text should have a `Content Size Fitter` set to **Preferred Size** to handle long translations.

### Localization Layout Safety
Chinese and German strings are typically 20–50% longer than English equivalents. Layout must not overflow:
- Use `Layout Element` with a `Max Width` on buttons to prevent overflow.
- Test layout by switching language in Play Mode (Settings > Language).

---

## 8. Common Mistakes to Avoid

| Mistake | Problem | Fix |
|---------|---------|-----|
| Canvas with no CanvasScaler | UI breaks at non-1080p | Add CanvasScaler, Scale With Screen Size |
| Canvas Scaler reference is 0 × 0 | All UI invisible or wrong size | Set Reference Resolution to 1920 × 1080 |
| Card texture Filter Mode = Bilinear | Blurry card art | Set to Point (no filter) |
| TMP Mip Maps enabled | Font blurs at small sizes | Disable Generate Mip Maps on font atlas texture |
| Multi-line YAML string in .asset | Parser fails on Rebuild | Keep `m_Localized:` on a single line |
| Hardcoded UI string | Not localized, breaks Chinese | Use `GameLocalization.Get(key)` |
| `Physics.OverlapSphere` in `Update` | Performance at high card counts | Triggered only on drop / spawn — guard carefully |
| Subscribing LanguageChanged without unsubscribing | Memory leak / stale callbacks | Always unsubscribe in `OnDisable` or `OnDestroy` |
| `Resources.Load` in `Update` | Hitches | Use catalog/cache in `Awake` only |
| Setting Render Scale < 1.0 in URP | Blurry everything | Keep Render Scale at 1.0 |

---

## 9. Recommended Testing Checklist

Run this after any visual change:

- [ ] 1920 × 1080 windowed — layout correct, no clipping
- [ ] 1280 × 720 — buttons scale, title readable
- [ ] 2560 × 1440 — UI doesn't stretch or shrink oddly
- [ ] 21:9 ultrawide — check for excessive horizontal stretch
- [ ] Fullscreen toggle — no layout pop
- [ ] Language: English → Chinese — text fits buttons, no glyph boxes
- [ ] Language: English → German — longest strings fit without overflow
- [ ] Card art at max zoom-in — pixels sharp, no blur
- [ ] Card art at max zoom-out — acceptable quality, no shimmer
- [ ] Title logo crisp at 1080p
- [ ] No yellow warnings in Console about missing fonts or glyph fallback
