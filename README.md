# WolfgodRPG - Advanced RPG System for Terraria

[![Terraria](https://img.shields.io/badge/Terraria-1.4+-blue.svg)](https://terraria.org/)
[![tModLoader](https://img.shields.io/badge/tModLoader-1.4+-green.svg)](https://github.com/tModLoader/tModLoader)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

Transform your Terraria world into an epic RPG adventure with advanced systems, vitals management, and Souls-like combat mechanics.

## 🎮 Features

### ⚔️ Combat Mode System
- **Souls-like Dash**: Double-tap directional controls (A, D, W, S)
- **Smooth Animation**: 360° rotation during dash with easing functions
- **Multiple Dashes**: Up to 3 dashes with stamina management
- **Stun System**: 2-second stun when stamina depletes
- **Combat Toggle**: Hotkey to activate/deactivate combat mode

### 🍎 Vitals System
- **Hunger**: Natural decrease, affects damage and movement speed
- **Sanity**: Decreases in combat/darkness, regenerates in houses
- **Stamina**: Powers all movement abilities and dash

### 🏃 Movement Skills
- **Dash**: Souls-like directional movement
- **Double Jump**: Unlocked at level 3
- **Wall Jump**: Unlocked at level 4
- **Stamina Cost**: All skills consume stamina

### 📊 RPG Systems
- **Player Leveling**: Gain attribute points per level
- **Subclass System**: Multiple classes with unique abilities
- **Proficiency**: Armor and weapon mastery
- **Experience**: Real-time tracking and notifications

## 🎯 Key Systems

### ⚡ Stamina Management
```csharp
// Outside Combat Mode: Natural regeneration
CurrentStamina = Math.Min(100f, CurrentStamina + 1f);

// In Combat Mode: Automatic regeneration with hunger cost
if (CurrentStamina <= 0f && !isStunned) {
    stunTimer = 120; // 2-second stun
    isStunned = true;
}
```

### 🍖 Hunger System
```csharp
// Natural decrease: 1% per minute
if (Main.GameUpdateCount % 3600 == 0) {
    CurrentHunger = Math.Max(0f, CurrentHunger - 1f);
}

// Effects based on hunger level
if (CurrentHunger < 50f) {
    // -50% damage and speed
} else if (CurrentHunger >= 70f) {
    // +50% damage
}
```

### 🏠 Sanity System
```csharp
// Decreases in combat or darkness
if (isInCombat || isInDarkness) {
    CurrentSanity = Math.Max(0f, CurrentSanity - 0.00138f);
}

// Regenerates in houses
if (isInHouse) {
    CurrentSanity = Math.Min(100f, CurrentSanity + 0.0055f);
}
```

## 🎮 Controls

### Combat Mode
- **Toggle**: Hotkey (configurable)
- **Dash**: Double-tap A (left), D (right), W (up), S (down)
- **Skills**: Consume stamina automatically

### Vitals Management
- **Food**: Eat any food item to restore hunger
- **Houses**: Stay in houses to regenerate sanity
- **Combat**: Avoid combat/darkness to preserve sanity

## 📊 Systems Overview

### Stamina System
| Mode | Regeneration | Cost |
|------|-------------|------|
| Outside Combat | Fast (1%/frame) | None |
| In Combat | Automatic | 1% hunger |
| Stunned | Blocked | 2 seconds |

### Hunger Effects
| Level | Damage | Speed | Effects |
|-------|--------|-------|---------|
| 0-50% | -50% | -50% | Starvation |
| 50-70% | Normal | Normal | None |
| 70-100% | +50% | Normal | Bonus damage |

### Sanity Effects
| Level | Effects | Regeneration |
|-------|---------|--------------|
| 0% | Confusion | None |
| 0-30% | -5 defense | Slow |
| 30-100% | None | Normal |

## 🏗️ Architecture

### Modular Design
- **VitalsSystem**: Manages hunger, sanity, stamina
- **SubClassSystem**: Handles classes and skills
- **AttributesSystem**: Manages player attributes
- **RPGPlayer**: Main player class with all systems

### Event-Driven Updates
```csharp
public override void PostUpdate() {
    UpdateVitals();
    UpdateDash();
    UpdateStunEffects();
    ProcessMilestoneEffects();
    UpdateMovementSkills();
}
```

## 🚀 Installation

1. **Install tModLoader** for Terraria
2. **Download WolfgodRPG** from Steam Workshop or GitHub
3. **Enable the mod** in tModLoader
4. **Start a new world** or load existing world
5. **Activate Combat Mode** to access all features

## 🎯 Getting Started

### First Steps
1. **Activate Combat Mode** using the hotkey
2. **Test the dash** by double-tapping movement keys
3. **Monitor your vitals** (hunger, sanity, stamina)
4. **Eat food** to restore hunger
5. **Stay in houses** to regenerate sanity

### Advanced Features
1. **Level up** to unlock new movement skills
2. **Choose subclasses** for unique abilities
3. **Master armor/weapon proficiency**
4. **Manage stamina** to avoid stun penalties

## 🔧 Configuration

### Stamina Settings
- **Dash Cost**: 10% stamina per dash
- **Stun Duration**: 2 seconds
- **Regeneration**: 1% per frame (outside combat)

### Hunger Settings
- **Decrease Rate**: 1% per minute
- **Food Restoration**: 15-60% based on food type
- **Starvation**: HP loss when reaching 0%

### Sanity Settings
- **Decrease Rate**: 5% per minute in combat/darkness
- **Regeneration**: 100% in 5 minutes in houses
- **Confusion**: Effect when reaching 0%

## 🐛 Troubleshooting

### Common Issues
- **Dash not working**: Check if Combat Mode is active
- **Stamina not regenerating**: Ensure you're outside Combat Mode
- **Hunger not decreasing**: Wait for natural decrease (1% per minute)
- **Sanity not regenerating**: Stay in houses with furniture

### Performance
- **Modular systems** for optimal performance
- **Event-driven updates** to reduce overhead
- **Efficient vitals management** with frame-based calculations

## 🤝 Contributing

We welcome contributions! Please see our [Contributing Guidelines](CONTRIBUTING.md) for details.

### Development Setup
1. **Clone the repository**
2. **Install tModLoader development tools**
3. **Build the mod** using `dotnet build`
4. **Test in-game** with tModLoader

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- **tModLoader Team** for the amazing modding framework
- **Terraria Community** for inspiration and feedback
- **Souls-like Games** for combat mechanics inspiration

---

**Made with ❤️ for the Terraria community**

*Transform your Terraria experience into an epic RPG adventure!* 