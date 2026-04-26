# LAST KERNEL

> Cyberpunk survival strategy game  
> Build the system. Watch it run. Survive the night.

---

## Overview

**LAST KERNEL** is a hybrid system-driven game combining:

- Card-based colony simulation (day)
- Auto battler defense (night)

You don’t control units directly.  
You design a system that either holds — or collapses.

---

## Core Gameplay Loop

```
Start Run
  ↓
Day Phase (pauseable)
  ↓
Gather → Combine → Build → Assign
  ↓
Prepare defenses
  ↓
Night Phase (auto combat)
  ↓
Resolve outcome
  ↓
Repeat (increasing difficulty)
```

---

## Game Design

### Day Phase — System Building

Everything is a **card**:
- Resources (scrap, energy, food)
- Units (workers, defenders)
- Structures (generators, labs, defenses)

Cards interact through **stacking**:
- Worker + Resource → production  
- Resource + Structure → crafting  
- Unit + Equipment → upgrades  

You are building a **self-sustaining system**, not just placing objects.

---

### Night Phase — Auto Battler

At night, the system runs on its own:
- Units auto-fight
- Defenses trigger
- Buffs and modifiers apply

Combat is:
- real-time
- deterministic
- hands-off

Your preparation determines the outcome.

---

## Project Structure

```
Assets/
 ├── _Project/
 │   ├── Scripts/
 │   │   ├── Core/
 │   │   ├── Card/
 │   │   ├── Combat/
 │   │   ├── Crafting/
 │   │   ├── Encounter/
 │   │   ├── Night/
 │   │   ├── Pack/
 │   │   ├── Quest/
 │   │   ├── Trading/
 │   │   └── UI/
 │   ├── Art/
 │   ├── Audio/
 │   └── Data/
```

---

## Systems Overview

### Card System
Defines all gameplay entities as data.

```
Card
 ├── ID
 ├── Type
 ├── Tags
 ├── Stats
 └── Localization Key
```

---

### Recipe System

```
Input Cards → Match Recipe → Output
```

---

### Colony System

Tracks:
- resources
- population
- morale
- progression

---

### Combat System

- tick-based simulation
- deterministic
- independent from UI

---

### Localization

- key-based system
- runtime switching
- English / Chinese support

---

## Tech Stack

- Unity 6 (URP 2D)
- C#
- Unity Localization
- Addressables
- New Input System

---

## Installation

### Requirements

- Unity 6000.x
- Git

### Setup

```
git clone https://github.com/zhenxiao-yu/LAST_KERNEL.git
```

Open in Unity Hub → Add Project → Select folder

---

## Running

Open:
```
Assets/_Project/Scenes/Main.unity
```

Press Play.

---

## Roadmap

- More cards and synergies
- Enemy scaling
- Events system
- Meta progression
- Mobile support
- UI polish

---

## Author

Zhenxiao (Mark) Yu
