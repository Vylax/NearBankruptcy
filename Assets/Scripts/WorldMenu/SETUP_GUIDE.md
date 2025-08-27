# World Menu Setup Guide

## Scripts Overview

### 1. GameManager.cs
- **Purpose**: Singleton that manages global game state and level progression
- **Setup**: Attach to a GameObject in your scene (it will persist across scenes)
- **Key Features**:
  - Stores current level and max levels
  - Handles level completion and progression
  - Saves/loads progress using PlayerPrefs
  - Provides debug methods for testing

### 2. WorldLevelManager.cs
- **Purpose**: Manages the world menu scene logic using HFSM
- **Setup**: Attach to a GameObject in your world menu scene
- **Required References**:
  - `progressAnimationSequencer`: Reference to AnimationSequencer component
  - `levelBumps`: Array of 5 LevelBump components
- **Key Features**:
  - Handles state transitions (Initialize → Idle → PlayingProgressAnimation)
  - Orchestrates progress animations when levels are completed
  - Manages player interactions with level bumps

### 3. AnimationSequencer.cs
- **Purpose**: Plays animations in perfect sequence for visual progression
- **Setup**: Attach to a GameObject and configure the animation steps
- **Configuration**:
  - Add AnimationStep entries for each square/bump sprite
  - Set animator references and trigger names
  - Configure durations or wait for animation completion
- **Key Features**:
  - Plays animations in sequence without delays
  - Can play to specific steps (for showing current progress)
  - Supports both duration-based and state-based waiting

### 4. LevelBump.cs
- **Purpose**: Handles player interaction with level start points
- **Setup**: Attach to each level bump GameObject
- **Configuration**:
  - Set `levelNumber` (1-5)
  - Configure `interactionRange` for proximity detection
  - Set `levelSceneName` for scene loading
  - Customize UI prompt text and style
- **Key Features**:
  - Detects player proximity
  - Shows "Press SPACE to start" prompt
  - Only allows interaction with current available level
  - Visual feedback based on level status (current/completed/locked)

## Setup Steps

1. **Create GameManager**:
   - Create an empty GameObject named "GameManager"
   - Attach GameManager.cs script
   - Set max levels to 5

2. **Setup World Menu Scene**:
   - Create an empty GameObject named "WorldLevelManager"
   - Attach WorldLevelManager.cs script

3. **Setup Animation Sequence**:
   - Create GameObject named "ProgressAnimator"
   - Attach AnimationSequencer.cs script
   - Add your square/bump sprites as children
   - Create Animator controllers for each sprite
   - Configure AnimationStep entries in the sequencer

4. **Setup Level Bumps**:
   - For each level bump sprite:
     - Attach LevelBump.cs script
     - Set appropriate level number (1-5)
     - Configure interaction range
     - Set scene name for level loading
     - Add Collider2D if using physics-based detection

5. **Connect References**:
   - In WorldLevelManager, assign:
     - ProgressAnimationSequencer reference
     - All 5 LevelBump references

## Usage Tips

- Use GameManager debug methods to test level progression
- The system automatically saves progress using PlayerPrefs
- Animations play in sequence when levels are completed
- Only the current level bump shows interaction prompts
- Player needs "Player" tag for proximity detection

## Animation Setup

1. Create Animator Controllers for each sprite
2. Add animation clips for your "drawing" effect
3. Set up triggers (default: "Play")
4. Configure the AnimationSequencer with proper durations or state names

## Testing

- Use GameManager context menu "Complete Current Level" to test progression
- Use "Reset Progress" to start over
- WorldLevelManager "Force Progress Animation" to test animations
