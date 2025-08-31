# Items Effects System

This folder contains the Items Effects system that manages item abilities in the game.

## System Overview

The Items Effects system automatically enables/disables item effect scripts based on what items are active in the player's item slots.

### Key Components:

1. **ItemsEffects.cs** - Main controller that manages all effect scripts
2. **Effect Scripts** - Individual scripts for each item type (e.g., SpeedBoostEffect.cs, DashEffect.cs, ShieldEffect.cs)

## Setup Instructions

### 1. Add ItemsEffects Component
Add the `ItemsEffects` script to the same GameObject that has your `GameManager` and `ItemsManager` components.

### 2. Create Effect GameObjects
For each item effect you want to implement:
- Create a child GameObject under your GameManager
- Add the appropriate effect script (e.g., SpeedBoostEffect, DashEffect, etc.)
- The effect script will be initially disabled

### 3. Assign References in Inspector
In the ItemsEffects component inspector:
- Assign each effect script to its corresponding field
- For example: Drag the GameObject with SpeedBoostEffect to the "Speed Boost Effect" field

### 4. Configure Effect Scripts
Each effect script has its own settings:
- **SpeedBoostEffect**: Speed multiplier
- **DashEffect**: Dash force, cooldown, input key
- **ShieldEffect**: Shield regeneration time

## How It Works

1. **Monitoring**: ItemsEffects continuously monitors the `itemsSlots` array in ItemsManager
2. **Activation**: When an item is placed in an active slot, its corresponding effect script is enabled
3. **Deactivation**: When an item is removed from active slots, its effect script is disabled
4. **Automatic Updates**: The system automatically updates when items are moved/changed

## Scene Restrictions

⚠️ **IMPORTANT**: All item effects only work in these scenes:
- **WorldMenu** - The main world/hub scene
- **Level scenes** - Any scene whose name starts with "Level" (e.g., "Level1", "Level2", "LevelBoss")

Effects are automatically disabled in other scenes like menus, cutscenes, etc.

## Supported Items

Currently implemented effect scripts:
- ✅ **SpeedBoost** - Increases movement speed
- ✅ **Dash** - Forward dash ability  
- ✅ **Shield** - Protection from one hit (one-time use per scene)
- ✅ **SlowMotion** - Slows down time (one-time use per scene)

### Items needing implementation:
- **DoubleJump** - Double jump ability
- **Glider** - Slow fall effect
- **SmallSize** - Reduces player size
- **ReverseGravity** - Flips gravity
- **WallJump** - Wall jumping ability
- **WalkThroughWalls** - Phase through walls
- **DepthStrider** - Swim faster

## Special Effects

### SlowMotion Effect
- **One-time use per scene**: Once activated, cannot be used again until scene changes
- **Activation**: Press Space key (configurable)
- **Duration**: 5 seconds of real-world time
- **Factor**: Slows game to half speed (0.5x)
- **Reset**: Usage resets automatically when entering a new scene

### Shield Effect
- **One-time use per scene**: Once triggered, cannot be used again until scene changes
- **Activation**: Automatically triggers when player takes damage
- **Invincibility Window**: 0.2 seconds of complete invincibility
- **Behavior**: Blocks one hit, provides brief invincibility, then wears out
- **Reset**: Usage resets automatically when entering a new scene

## Debug Features

### ItemsEffects Debug:
- Enable "Debug Mode" in inspector for detailed logging
- Use Context Menu "Force Update Effects" to manually refresh
- Use Context Menu "Log Current Active Effects" for debugging

### Individual Effect Debug:
- Each effect script has debug mode and test methods
- Use Context Menu options to manually test effects
- **SlowMotionEffect** has "Reset Usage" and "Test Slow Motion" context menus
- **ShieldEffect** has "Reset Usage" and "Test Shield" context menus

## Integration with Player

The effect scripts contain TODO comments showing where to integrate with your player controller:

```csharp
// Example from SpeedBoostEffect:
// PlayerController playerController = FindObjectOfType<PlayerController>();
// if (playerController != null)
// {
//     playerController.ApplySpeedMultiplier(speedMultiplier);
// }
```

Replace these TODOs with actual calls to your player system.

## Events Integration

The system automatically handles:
- **Game Reset**: All effects disabled when GameManager.ResetProgress() is called
- **Scene Changes**: Effects remain active across level transitions
- **Item Changes**: Real-time updates when items are moved in/out of active slots

## Performance Notes

- Effects are only updated when item slots actually change
- Disabled effect scripts consume minimal resources
- System uses efficient HashSet comparisons to detect changes
