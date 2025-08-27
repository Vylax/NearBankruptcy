# World Menu Setup Guide

## 🎯 Complete Unity 6 Game Jam Setup Guide

This guide will walk you through setting up a complete 2D platformer world menu with level progression, perfect sync animations, and HFSM state management.

---

## 📂 Scripts Overview

### 1. GameManager.cs ✅
- **Purpose**: Compact singleton managing global game state and level progression
- **Features**: Current level tracking, PlayerPrefs save/load, debug methods

### 2. WorldLevelManager.cs ✅  
- **Purpose**: Scene logic manager using UnityHFSM for state management
- **Features**: State transitions, animation orchestration, player interaction handling

### 3. SimpleAnimationSequencer.cs ✅
- **Purpose**: Plays animations in perfect sequence with no delays
- **Features**: Sequential animation playback, progress visualization, reset functionality

### 4. LevelBump.cs ✅
- **Purpose**: Level start point interaction with proximity detection
- **Features**: "Press SPACE" UI prompts, level validation, scene loading

---

## 🔧 Step-by-Step Setup

### **Step 1: Scene Hierarchy Setup**

Create this exact hierarchy in your World Menu scene:

```
📁 World Menu Scene
├── 🎮 GameManager (Empty GameObject)
├── 🌍 WorldLevelManager (Empty GameObject)
├── 📍 Level Path (Empty GameObject - for organization)
│   ├── 🟦 Square1 (Sprite Renderer)
│   ├── 🟦 Square2 (Sprite Renderer)  
│   ├── 🟦 Square3 (Sprite Renderer)
│   ├── 🔘 LevelBump1 (Sprite Renderer)
│   ├── 🟦 Square4 (Sprite Renderer)
│   ├── 🟦 Square5 (Sprite Renderer)
│   ├── 🔘 LevelBump2 (Sprite Renderer)
│   ├── 🟦 Square6 (Sprite Renderer)
│   ├── 🟦 Square7 (Sprite Renderer)
│   ├── 🔘 LevelBump3 (Sprite Renderer)
│   ├── 🟦 Square8 (Sprite Renderer)
│   ├── 🟦 Square9 (Sprite Renderer)
│   ├── 🔘 LevelBump4 (Sprite Renderer)
│   ├── 🟦 Square10 (Sprite Renderer)
│   ├── 🟦 Square11 (Sprite Renderer)
│   └── 🔘 LevelBump5 (Sprite Renderer)
└── 🎯 Player (Your player character)
```

---

### **Step 2: Animation Setup (Simple!)**

#### **For Each Sprite (Squares + Bumps):**
1. **Add Animator Component**
   - Select sprite → Add Component → Animator

2. **Create Animator Controller**
   - Right-click in Project → Create → Animator Controller
   - Name it "LineAnimator" (can reuse same controller for all)
   - Drag controller to Animator component

3. **Setup Animation States**
   - Open Animator window
   - Create state named **"Line_Idle"** (default/idle state - invisible)
   - Create state named **"Line"** (drawing animation state - animated)
   - Create state named **"Line_Drawn"** (final state - visible/drawn)
   - Add your drawing animation clip to the "Line" state
   - Leave "Line_Idle" empty (invisible) and "Line_Drawn" with final frame (visible)
   - **No triggers needed!** The SimpleAnimationSequencer handles everything

4. **Set Default State**
   - Right-click **"Line_Idle"** state → "Set as Layer Default State"

---

### **Step 3: Component Configuration**

#### **🎮 GameManager Setup:**
1. **Attach GameManager.cs** to GameManager GameObject
2. **Configure:**
   ```
   Max Levels: 5
   ```

#### **🌍 WorldLevelManager Setup:**
1. **Attach WorldLevelManager.cs** to WorldLevelManager GameObject
2. **Attach SimpleAnimationSequencer.cs** to WorldLevelManager GameObject
3. **Configure WorldLevelManager:**
   ```
   Progress Animation Sequencer: [Drag WorldLevelManager GameObject here]
   Level Bumps: [Drag all 5 LevelBump GameObjects here]
   Animation Delay: 0.5
   ```
4. **Configure SimpleAnimationSequencer:**
   ```
   Level Animation Groups: [Configure each level completion]
   Animation State Name: "Line"
   Idle State Name: "Line_Idle"
   Drawn State Name: "Line_Drawn"
   Delay Between Animations: 0
   ```
   
   **For each Level Group:**
   ```
   Level Number: 1, 2, 3, 4, 5
   Level Name: "Level 1", "Level 2", etc.
   Animators: [Drag sprites for THIS level completion]
   ```

#### **🔘 LevelBump Setup:**
1. **Attach LevelBump.cs** to each LevelBump GameObject
2. **Add Collider2D** (set as Trigger)
3. **Configure each bump:**
   ```
   LevelBump1: Level Number = 1, Level Scene Name = "Level1"
   LevelBump2: Level Number = 2, Level Scene Name = "Level2"  
   LevelBump3: Level Number = 3, Level Scene Name = "Level3"
   LevelBump4: Level Number = 4, Level Scene Name = "Level4"
   LevelBump5: Level Number = 5, Level Scene Name = "Level5"
   
   All: Interaction Range = 3
   ```

#### **🎯 Player Setup:**
1. **Add "Player" tag** to your player GameObject
2. **Add Collider2D** for proximity detection

---

### **Step 4: Final Connections**

#### **WorldLevelManager References:**
1. **Progress Animation Sequencer**: Drag WorldLevelManager GameObject
2. **Level Bumps Array**: 
   - Set Size to 5
   - Drag LevelBump1 → Element 0
   - Drag LevelBump2 → Element 1
   - Drag LevelBump3 → Element 2
   - Drag LevelBump4 → Element 3
   - Drag LevelBump5 → Element 4

#### **SimpleAnimationSequencer Level Groups:**
Configure each level completion separately:
```
Level Group 0:
├── Level Number: 1
├── Level Name: "Level 1"
└── Animators: [Square1, Square2, Square3, LevelBump1]

Level Group 1:  
├── Level Number: 2
├── Level Name: "Level 2"
└── Animators: [Square4, Square5, LevelBump2]

Level Group 2:
├── Level Number: 3
├── Level Name: "Level 3" 
└── Animators: [Square6, Square7, Square8, Square9, LevelBump3]

... (continue for each level)
```

---

## 🎮 How It All Works

### **Game Flow:**
1. **Scene Loads** → All sprites start in idle state (Line_Idle)
2. **GameManager** loads saved progress 
3. **Player approaches current level bump** → "Press SPACE to start" appears
4. **Player completes level** → returns to world menu
5. **Next line animates** (Line animation) → new level becomes available
6. **After animation** → sprites return to idle state

### **Animation Sequence:**
```
Player completes Level 1 → PlayLevelCompletionAnimation(1) →
Square1: Line_Idle → Line → Line_Drawn
Square2: Line_Idle → Line → Line_Drawn  
Square3: Line_Idle → Line → Line_Drawn
LevelBump1: Line_Idle → Line → Line_Drawn
(Perfect sync, permanent progress!)
```

---

## 🧪 Testing & Debug

### **Testing Level Progression:**
1. **Play the scene**
2. **GameManager** → Right-click → "Complete Current Level"
3. **Watch line draw** to next bump perfectly!
4. **Walk to bump** → see "Press SPACE" prompt

### **Debug Methods Available:**
- **GameManager**: "Complete Current Level", "Reset Progress"
- **WorldLevelManager**: "Force Progress Animation", "Show Current Progress", "Reset World"
- **SimpleAnimationSequencer**: "Test Play Level 1", "Test Play Level 2", "Debug Level Groups", "Test Reset"

---

## ✅ Troubleshooting

### **Animations Not Playing:**
- Check state names: "Line", "Line_Idle", "Line_Drawn" 
- Verify Level Numbers are set correctly (1, 2, 3, 4, 5)
- Use "Debug Level Groups" to check your setup
- Ensure animation clips are properly assigned
- Make sure "Line_Idle" is set as the default state in Animator

### **Level Bumps Not Responding:**
- Verify player has "Player" tag
- Check Collider2D is set as Trigger
- Confirm Level Numbers are set correctly (1-5)

### **Progress Not Saving:**
- GameManager persists between scenes automatically
- Progress saves to PlayerPrefs automatically

---

## 🎯 You're Done!

Your complete game jam world menu system is now ready with:
✅ Compact, readable code  
✅ Perfect sync animations  
✅ HFSM state management  
✅ Smart level progression  
✅ Proximity-based UI  

**Happy game jamming!** 🎮
