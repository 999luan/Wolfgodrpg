# WolfgodRPG - Wiki

Welcome to the WolfgodRPG wiki! This mod transforms Terraria into an epic RPG experience with advanced systems, vitals management, and Souls-like combat mechanics.

## 🎮 Core Systems

### ⚔️ Combat Mode System

The Combat Mode is the heart of WolfgodRPG, providing Souls-like combat mechanics:

#### **Souls-like Dash**
- **Controls**: Double-tap A (left), D (right), W (up), S (down)
- **Animation**: Smooth 360° rotation during dash
- **Stamina Cost**: 10% per dash
- **Multiple Dashes**: Up to 3 dashes before cooldown

#### **Stun System**
- **Trigger**: When stamina reaches 0%
- **Duration**: 2 seconds
- **Effects**: 
  - Dash blocked
  - Movement slowed
  - Visual feedback (particles, messages)
- **Recovery**: Automatic stamina regeneration after stun

#### **Combat Mode Toggle**
- **Activation**: Hotkey (configurable)
- **Effects**: Changes stamina regeneration behavior
- **Visual**: UI indicators for active state

### 🍎 Vitals System

Three interconnected vital systems that affect gameplay:

#### **Hunger System**
- **Natural Decrease**: 1% per minute
- **Effects**:
  - 0-50%: -50% damage and speed, starvation
  - 50-70%: Normal performance
  - 70-100%: +50% damage bonus
- **Restoration**: Eat any food item
- **Food Values**:
  - Basic foods: 15% hunger
  - Intermediate foods: 25% hunger
  - Main foods: 40% hunger
  - Special foods: 60% hunger

#### **Sanity System**
- **Decrease**: 5% per minute in combat or darkness
- **Regeneration**: 100% in 5 minutes when in houses
- **Effects**:
  - 0%: Confusion effect
  - 0-30%: -5 defense
  - 30-100%: Normal
- **House Detection**: Based on walls and furniture

#### **Stamina System**
- **Outside Combat**: Fast regeneration (1% per frame)
- **In Combat**: Automatic regeneration with hunger cost
- **Stun**: 2-second penalty when depleted
- **Usage**: Powers dash and movement skills

### 🏃 Movement Skills

Progressive skill system unlocked by leveling:

#### **Dash (Level 1)**
- **Type**: Souls-like directional movement
- **Cost**: 10% stamina
- **Animation**: 360° rotation with easing
- **Controls**: Double-tap directional keys

#### **Double Jump (Level 3)**
- **Type**: Additional jump in mid-air
- **Cost**: 10% stamina
- **Unlock**: Automatic at level 3

#### **Wall Jump (Level 4)**
- **Type**: Jump off walls
- **Cost**: 10% stamina
- **Unlock**: Automatic at level 4

### 📊 RPG Systems

#### **Player Leveling**
- **Experience**: Gained from various activities
- **Level Up**: Gain 5 attribute points
- **Skills**: Unlock movement abilities progressively

#### **Subclass System**
- **Multiple Classes**: Warrior, Archer, Mage, Summoner, Acrobat, Explorer
- **Unique Abilities**: Each class has special skills
- **Experience**: Separate XP tracking per class

#### **Proficiency System**
- **Armor Proficiency**: Gain XP when taking damage
- **Weapon Proficiency**: Gain XP when dealing damage
- **Level Effects**: Improved performance with higher proficiency

## 🎯 Gameplay Mechanics

### Stamina Management

#### **Outside Combat Mode**
```csharp
// Fast natural regeneration
CurrentStamina = Math.Min(100f, CurrentStamina + 1f);
```

#### **In Combat Mode**
```csharp
// Automatic regeneration with stun penalty
if (CurrentStamina <= 0f && !isStunned) {
    stunTimer = 120; // 2-second stun
    isStunned = true;
}
```

### Hunger Effects

#### **Damage and Speed Modifiers**
```csharp
if (CurrentHunger < 50f) {
    Player.GetDamage(DamageClass.Generic) *= 0.5f;
    Player.moveSpeed *= 0.5f;
} else if (CurrentHunger >= 70f) {
    Player.GetDamage(DamageClass.Generic) *= 1.5f;
}
```

### Sanity Management

#### **Decrease Conditions**
```csharp
bool isInCombat = CombatModeActive;
bool isInDarkness = !Main.dayTime || Player.ZoneRockLayerHeight;
```

#### **Regeneration Conditions**
```csharp
bool isInHouse = IsPlayerInHouse();
if (isInHouse) {
    CurrentSanity = Math.Min(100f, CurrentSanity + 0.0055f);
}
```

## 🎮 Controls Guide

### Combat Mode Controls
| Action | Control | Description |
|--------|---------|-------------|
| Toggle Combat Mode | Hotkey | Activate/deactivate combat mode |
| Dash Left | Double-tap A | Dash to the left |
| Dash Right | Double-tap D | Dash to the right |
| Dash Up | Double-tap W | Dash upward |
| Dash Down | Double-tap S | Dash downward |
| Double Jump | Jump (in air) | Additional jump |
| Wall Jump | Jump (near wall) | Jump off walls |

### Vitals Management
| Action | Method | Effect |
|--------|--------|--------|
| Restore Hunger | Eat food | Restore 15-60% hunger |
| Regenerate Sanity | Stay in houses | 100% in 5 minutes |
| Manage Stamina | Avoid overuse | Prevent stun penalty |

## 📊 Systems Overview

### Stamina System Comparison
| Mode | Regeneration | Cost | Dash Available |
|------|-------------|------|----------------|
| Outside Combat | Fast (1%/frame) | None | Yes |
| In Combat | Automatic | 1% hunger | Yes |
| Stunned | Blocked | 2 seconds | No |

### Hunger Effects Table
| Hunger Level | Damage | Speed | Special Effects |
|--------------|--------|-------|----------------|
| 0-50% | -50% | -50% | Starvation (HP loss) |
| 50-70% | Normal | Normal | None |
| 70-100% | +50% | Normal | Bonus damage |

### Sanity Effects Table
| Sanity Level | Effects | Regeneration Rate |
|--------------|---------|-------------------|
| 0% | Confusion | None |
| 0-30% | -5 defense | Slow |
| 30-100% | None | Normal |

## 🏗️ Technical Architecture

### Modular System Design
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

### Performance Optimizations
- **Frame-based calculations** for vitals
- **Event-driven updates** to reduce overhead
- **Modular systems** for optimal performance
- **Efficient UI updates** with change detection

## 🚀 Getting Started

### First Steps
1. **Install the mod** via tModLoader
2. **Activate Combat Mode** using the hotkey
3. **Test the dash** by double-tapping movement keys
4. **Monitor your vitals** (hunger, sanity, stamina)
5. **Eat food** to restore hunger
6. **Stay in houses** to regenerate sanity

### Advanced Features
1. **Level up** to unlock new movement skills
2. **Choose subclasses** for unique abilities
3. **Master armor/weapon proficiency**
4. **Manage stamina** to avoid stun penalties
5. **Optimize vitals** for maximum performance

## 🔧 Configuration

### Stamina Settings
- **Dash Cost**: 10% stamina per dash
- **Stun Duration**: 2 seconds
- **Regeneration**: 1% per frame (outside combat)
- **Combat Regeneration**: Automatic with hunger cost

### Hunger Settings
- **Decrease Rate**: 1% per minute
- **Food Restoration**: 15-60% based on food type
- **Starvation**: HP loss when reaching 0%
- **Bonus Damage**: +50% at 70-100% hunger

### Sanity Settings
- **Decrease Rate**: 5% per minute in combat/darkness
- **Regeneration**: 100% in 5 minutes in houses
- **Confusion**: Effect when reaching 0%
- **Defense Penalty**: -5 defense below 30%

## 🐛 Troubleshooting

### Common Issues
- **Dash not working**: Check if Combat Mode is active
- **Stamina not regenerating**: Ensure you're outside Combat Mode
- **Hunger not decreasing**: Wait for natural decrease (1% per minute)
- **Sanity not regenerating**: Stay in houses with furniture
- **Stun not ending**: Wait for 2-second duration

### Performance Issues
- **Modular systems** for optimal performance
- **Event-driven updates** to reduce overhead
- **Efficient vitals management** with frame-based calculations
- **UI optimization** with change detection

## 🤝 Contributing

We welcome contributions! The mod is built with:
- **Modular architecture** for easy extension
- **Event-driven systems** for performance
- **Comprehensive documentation** for developers

### Development Setup
1. **Clone the repository**
2. **Install tModLoader development tools**
3. **Build the mod** using `dotnet build`
4. **Test in-game** with tModLoader

---

**Transform your Terraria experience into an epic RPG adventure!**

*For more information, check the [README](../README.md) and [Changelog](../CHANGELOG.md).*