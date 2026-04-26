# Art Direction — LAST KERNEL

**Source of truth for all UI and visual design decisions.**
When implementing UI, consult this document first. Do not override these rules without explicit instruction.

---

## Visual Identity

**Game:** LAST KERNEL
**Style:** Dark cyberpunk pixel-art bunker terminal UI.
Aesthetic references: underground command terminal, server core, survival bunker HUD. Functional over decorative. Everything on screen should feel like it belongs to a machine that is barely holding together.

---

## Resolution and Scaling

| Setting | Value |
|---|---|
| Base pixel composition | 320 × 180 |
| Display reference resolution | 1920 × 1080 |
| Primary aspect ratio | 16:9 |
| Scaling mode | Integer scaling preferred; fallback to closest integer + letterbox |
| Platform priority | Desktop first, mobile-safe responsive |

- Pixel art must remain **crisp at all sizes** — never bilinear/trilinear filtered.
- Use **Unity Pixel Perfect Camera** wherever applicable.
- All sprites use a **consistent PPU** across the project (confirm in `ProjectSettings`).
- UI layout increments: **16 px / 24 px / 32 px** grid.
- Avoid soft gradients and anti-aliased UI edges — dither instead.

---

## Color Palette

| Role | Description | Usage |
|---|---|---|
| Background | Dark navy, charcoal, black-blue | Scene backgrounds, panel fills, card backs |
| Primary accent | Cyan terminal glow | Borders, active state, key highlights, cursor |
| Secondary accent | Muted magenta | Secondary highlights, categories, warnings |
| Text | Light gray / off-white | All body text, labels, card descriptions |
| Warning / danger | Amber or red-orange | HP low, threat indicators, night phase |
| Disabled / inactive | Desaturated mid-gray | Grayed-out buttons, locked items |

**Avoid:** bright rainbow colors, glossy gradients, high-saturation fills, mobile-game shine effects.
The palette should read as utilitarian and worn-in, not cheerful.

---

## Typography

- Use **pixel-style fonts** where readability holds.
- Both **English and Simplified Chinese** must be clearly legible at all text sizes.
- Do not use decorative or script fonts for body text.
- All UI text must be **localization-ready** — no hardcoded strings.
- Design every layout assuming **text can expand up to 30–40%** for translated languages.

### Text Hierarchy

| Level | Size | Style | Use case |
|---|---|---|---|
| Title | Large | Bold, high contrast | Scene titles, main menu header |
| Button text | Medium | Readable, uppercase or sentence case | All interactive buttons |
| Card title | Compact | Clear, single line preferred | Top of each card |
| Card description | Small | Readable at card size, word-wrap allowed | Effect/flavor text on cards |
| Resource numbers | High contrast | Paired with icon | HUD resource bars, cost indicators |
| Tooltip / label | Small | Muted, secondary | Supplementary info panels |

---

## Icons

Use icons alongside text for gameplay values. Icons must be pixel-art, consistent size, and theme-consistent.

| Value | Icon |
|---|---|
| Cost / currency | Coin / scrap chip |
| Output | Arrow or gear |
| Time / duration | Clock or hourglass |
| Danger / threat | Skull or hazard triangle |
| Morale | Heart or signal bar |
| Power / energy | Lightning bolt |
| Food / nutrition | Flask or ration |
| Scrap / material | Wrench or fragment |

---

## Card Visual Style

Cards are the primary game object. They must be instantly readable and feel like physical terminal readouts.

### Layout (top to bottom)

1. **Top bar** — Card name (localized), category badge
2. **Center** — Pixel-art icon or illustration, fills the visual weight
3. **Bottom bar** — Localized description / effect text; stat icons with values

### Frame and borders

- **Dark terminal card frame** — thick enough to read at small sizes.
- Border color or style indicates **category / rarity** — each category has a distinct consistent visual.
- Corner or edge cuts use subtle **cyberpunk panel geometry** (chamfered corners, diagonal cuts) — kept minimal.
- Cards must look **functional, not ornamental**.

### Card Categories — Visual Language

Each category uses consistent border treatment and icon set. Do not mix category styling.

| Category | Visual cue |
|---|---|
| Resource | Muted border, material icon |
| Worker / Villager | Warmer border tint, person silhouette |
| Machine | Angular frame elements, gear motif |
| Defense | Reinforced frame style, shield motif |
| Enemy / Threat | Red-orange or magenta accent, warning marks |
| Quest / Objective | Cyan accent, objective marker |
| Recipe / Output | Gear + arrow motif, output highlight |

---

## UI Panels and Modals

- Panels use **dark backgrounds with thin cyan borders**.
- Subtle **scanline or dither texture** is acceptable if it reads as texture, not noise.
- **Clean internal spacing** — do not pack elements edge to edge.
- Every modal has clear **close / confirm / cancel** affordance.
- Modals should **not obscure the game board** unless absolutely required — prefer side panels, drawers, or compact overlays.

---

## Main Menu

- Title / logo: **aligned left**, high on screen.
- Buttons: **stacked vertically on the left side**.
- Background art: weighted to the **right side** of the screen.
- Atmosphere: dark bunker / server core — dim lighting, no sparkle.
- Animation: **minimal** — no idle loops, no particle showers. A subtle scanline or slow ambient pulse is acceptable.
- No visual clutter on the start screen.

---

## Gameplay UI (HUD and Board)

- **The board is always the focus.** HUD elements are secondary.
- HUD must be **compact** — hug screen edges, use minimal vertical height.
- **Resource bar:** icon-driven, high contrast numbers, horizontal strip.
- **Quest / objective panel:** readable but not dominant — collapsible on mobile.
- **Battle / Night phase UI:** urgent feel (amber/red accents, tighter borders) but still clean and readable.
- Card management UI and battle UI should feel like **one unified system** — do not use visually disconnected styles for them.

---

## Animations and Motion

| Principle | Rule |
|---|---|
| Character | Subtle, terminal-like — nothing bouncy or cartoon |
| Hover | Small scale increase only (no glow bursts, no oversized feedback) |
| Button press | Gentle depression feedback, short |
| Transitions | Short, snappy — prefer cuts or fast slides over slow fades |
| Motion | Readable over flashy — player must always understand what happened |
| Pixel crispness | Tweens must not produce sub-pixel blur — use integer positions where possible |

**Avoid:** spring physics, overshooting bounces, heavy particle systems on common interactions, slow ease-in/out on frequent UI elements.

---

## Mobile Layout

- **Touch targets:** minimum 44 × 44 px display pixels.
- **Avoid small text** — no text below readable size on a phone screen.
- **Primary actions** (drag, select, confirm) reachable without thumb stretch.
- Implement **safe-area insets** for notch/home-bar devices.
- **Card drag/drop** must work with both touch and mouse — do not split the code paths.
- Desktop: can display multiple side panels simultaneously.
- Mobile: **collapse secondary panels** behind toggles or swipe drawers; show one focus area at a time.

---

## Localization — UI Design Rules

- Every visible string must be fed through the localization system — **no hardcoded UI text**.
- **English and Simplified Chinese** are the two required languages; all layouts must support both.
- Test card titles, button labels, and description boxes with Chinese text before finalizing layout sizes.
- Chinese characters render wider relative to line height — **allocate vertical space** for line wrap.
- Buttons and pills must not clip translated text — use flexible width where possible.

---

## Architecture and Code Rules

- All project UI assets and scripts live under `Assets/_Project/`.
- Do **not** modify third-party package files directly.
- Reuse existing assets and prefabs before creating new ones.
- **Separate UI presentation from gameplay logic** — UI components should not own game state.
- Prefer **reusable UI prefab components** over bespoke per-screen widgets.
- Avoid god classes — one component, one responsibility.
- Follow the `Markyu.LastKernel` namespace for all project scripts.

---

## Quick Reference Checklist

Before shipping any UI element, verify:

- [ ] Pixel art is **point-sampled** (no blur at any scale)
- [ ] Text is **localized** — no hardcoded strings
- [ ] Layout holds with **Chinese text** (test wrap and overflow)
- [ ] Color usage follows the **palette** — no off-palette fills
- [ ] **Touch target size** is adequate on mobile
- [ ] **Animation duration** is short and snappy, not bouncy
- [ ] Panel does not **obscure the board** unnecessarily
- [ ] Card border/style matches the correct **category**
- [ ] All icons are **pixel-art, consistent PPU**
- [ ] Tested at **1920×1080** and at a simulated **mobile aspect ratio**
