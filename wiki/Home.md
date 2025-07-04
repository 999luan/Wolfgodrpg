# Wolfgod RPG Mod Wiki

Welcome to the Wolfgod RPG Mod Wiki for tModLoader!

This mod aims to implement a comprehensive RPG progression system within Terraria, focusing on three interconnected pillars of character development:

-   **Player Level (Overall):** Represents the fundamental growth of your character, providing attribute points for customization.
-   **Class Levels (Action-Based):** Rewards players for performing specific actions in the game world, such as combat, exploration, or crafting.
-   **Equipment Proficiency (Mastery):** Encourages continuous use of specific weapon and armor types, granting specialized bonuses.

## Core Systems:

### 1. Player Level System

Your overall Player Level increases as you achieve significant milestones in the game. Unlike class or proficiency levels, general Player XP is not gained through direct grinding but by reaching key progression points (e.g., defeating bosses, completing major quests).

**Rewards:**
-   **Attribute Points:** Upon leveling up, you gain points to distribute among your primary attributes, allowing for tailored character builds.
-   **Passive Stat Increases:** Automatic increases to core stats like maximum health and mana.

### 2. Primary Attributes

These are the foundational statistics that define your character's raw capabilities. You gain Attribute Points from increasing your Player Level, which can then be allocated to enhance these stats:

-   **Strength (💪):** Primarily affects melee damage and carrying capacity. Related classes: Warrior, Blacksmith.
-   **Dexterity (🎯):** Influences ranged damage, critical strike chance, and attack speed. Related classes: Archer, Acrobat.
-   **Intelligence (🧠):** Boosts magic damage, maximum mana, and spell casting speed. Related classes: Mage, Alchemist, Mystic.
-   **Constitution (🛡️):** Increases maximum health, defense, and health regeneration. Related classes: Warrior, Survivalist.
-   **Wisdom (🦉):** Affects summon damage, luck, and resistance to debuffs. Related classes: Mystic, Summoner.

### 3. Class Level System

Classes in Wolfgod RPG are defined by actions, allowing players to evolve multiple classes simultaneously by engaging in diverse activities.

**XP Gain:** XP is gained by performing actions relevant to each class (e.g., dealing melee damage for Warrior, exploring for Explorer).

**Rewards:**
-   **Passive Bonuses:** Each class level grants passive bonuses to relevant attributes or stats.
-   **Unlocked Abilities:** At key milestones (e.g., Level 25, 50, 75, 100), classes unlock unique active or passive abilities.
    -   **Example (Acrobat):** Level 1 (Basic Dash), Level 25 (Double Jump), Level 50 (Enhanced Dash), Level 75 (Evasive Dash), Level 100 (Second Aerial Dash).

### 4. Equipment Proficiency System

This system rewards continuous use of specific weapon and armor types, encouraging mastery over your gear.

#### Weapon Proficiency

-   **XP Gain:** You gain XP for a weapon type by dealing damage with that type of weapon (e.g., Swords for Melee, Bows for Ranged, Staves for Magic, Whips for Summon).
-   **Rewards:** Passive bonuses specific to that weapon type (e.g., increased damage, attack speed, critical chance).
-   **Weapon Types Implemented:**
    -   **Melee (⚔️):** Swords, Spears, Yoyos, etc.
    -   **Ranged (🏹):** Bows, Guns, Repeaters, etc.
    -   **Magic (✨):** Staves, Spellbooks, etc.
    -   **Summon (🐾):** Whips, Summoning Staves, etc.

#### Armor Proficiency

-   **XP Gain:** You gain XP for an armor type by taking damage while wearing armor of that type.
-   **Rewards:** Passive bonuses specific to that armor category (e.g., increased defense, damage reduction).
-   **Armor Types Implemented:**
    -   **Light (🏃):** Focuses on movement speed and evasion.
    -   **Heavy (🛡️):** Provides increased defense and damage reduction.
    -   **Magic Robes (🔮):** Enhances mana capacity and magical abilities.

### 5. Item Affixes

To make loot more exciting, weapons and armor can be generated with random bonuses (affixes) that interact with the mod's progression systems.

-   **Generation:** When an item is dropped or crafted, it has a chance to receive one or more affixes.
-   **Affix Examples:**
    -   `+5 Strength` (Primary Attribute bonus)
    -   `+10% Warrior Class XP` (Class XP bonus)
    -   `+2 Acrobat Class Levels (while equipped)` (Class Level bonus)
    -   `+15% Sword Damage` (Weapon Proficiency bonus)
    -   `+5% Mining Speed` (Utility bonus)
-   **Display:** Affixes are listed in the item's tooltip, along with their rarity (Common, Uncommon, Rare, Epic, Legendary).

## Current Project Status:

The core systems for Player Level, Primary Attributes, Class Levels, Weapon Proficiency, Armor Proficiency, and Item Affixes have been implemented. Data management, persistence (`SaveData`/`LoadData` in `RPGPlayer.cs` and `RPGGlobalItem.cs`), and multiplayer synchronization (`SyncPlayer`, `SendClientChanges` in `RPGPlayer.cs`) are in place.

**UI Implementation Status:**
-   **`SimpleRPGMenu.cs`:** Functions as the main UI container, managing tabs. Updated to correctly reflect armor proficiency changes.
-   **`RPGStatsPageUI.cs`:** Displays vital statistics and class summaries. Updated to display Player Level, Player Experience, Attribute Points, and Primary Attributes. The functionality to distribute attribute points via UI buttons has been implemented.
-   **`RPGClassesPageUI.cs`:** Displays class details, unlocked abilities, and attribute bonuses. `GetStatDisplayName` has been centralized to `RPGDisplayUtils.cs`, and hardcoded dash logic removed.
-   **`RPGItemsPageUI.cs`:** Displays items with RPG attributes and progressive items. `GetStatDisplayName` has been centralized, and duplicate code removed. Progressive item XP calculation corrected.
-   **`RPGProficienciesPageUI.cs`:** Displays armor and weapon proficiencies. Updated to correctly display both armor and weapon proficiencies, with `CreateProficiencyCard` renamed to `CreateArmorProficiencyCard` for clarity.
-   **`RPGProgressPageUI.cs`:** Displays player progress (bosses, events, general stats). No significant inconsistencies found.

**Debugging & Best Practices:**
-   Debugging logs have been added to `RPGPlayer.cs` (`OnHitNPC`, `OnPlayerDamaged`) to diagnose XP gain for weapon and armor proficiencies.
-   Review of tModLoader documentation (`ModPlayer`, `GlobalItem`) and `ExampleMod` confirms that current implementations align with general best practices for data management, persistence, synchronization, and item modification.

## Future Plans & Recommendations:

1.  **UI Refinement (Scrolling & Positioning):** A thorough review of UI scrolling and element positioning across all interfaces is recommended to ensure a fluid user experience.
2.  **Comprehensive Testing:** After the implementation of primary attributes and their interactions, extensive testing of all mod functionalities is crucial to ensure stability and correctness of calculations and UI.
3.  **Display Proficiency in Item Tooltips:** Implement logic in `RPGGlobalItem.cs` to display the player's current weapon/armor proficiency levels on relevant item tooltips. This is a missing feature, not a bug in existing display logic.
4.  **`ResetEffects()` for Stat Application:** Ensure all stat bonuses (attributes, classes, proficiencies, affixes) are correctly aggregated and applied in `RPGCalculations.cs` for a robust and balanced stat system.
5.  **Optimized Multiplayer Synchronization:** For large data dictionaries, consider network optimizations to send only changes (deltas) instead of the entire dictionary, although the current approach is functional for the present scope.
6.  **Robust `GetWeaponType` and `GetEquippedArmorType`:** For broader compatibility with modded and vanilla items, these functions can be expanded to verify more item properties and damage types.
7.  **Error Handling & Edge Cases:** Add more robust error handling, especially for data loading and network synchronization, to manage unexpected scenarios (e.g., corrupted data, missing enums).

For more in-depth technical details and the complete development plan, please refer to the `ANALISE_COMPLETA.md` file in the project repository.