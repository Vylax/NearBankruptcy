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
│   ├── 🟦 Square1 (Sprite Renderer + Animator)
│   ├── 🟦 Square2 (Sprite Renderer + Animator)  
│   ├── 🟦 Square3 (Sprite Renderer + Animator)
│   ├── 🔘 LevelBump1_Parent (Sprite Renderer - for color)
│   │   └── 🔘 LevelBump1_Animator (Animator - for animation)
│   ├── 🟦 Square4 (Sprite Renderer + Animator)
│   ├── 🟦 Square5 (Sprite Renderer + Animator)
│   ├── 🔘 LevelBump2_Parent (Sprite Renderer - for color)
│   │   └── 🔘 LevelBump2_Animator (Animator - for animation)
│   ├── 🟦 Square6 (Sprite Renderer + Animator)
│   ├── 🟦 Square7 (Sprite Renderer + Animator)
│   ├── 🔘 LevelBump3_Parent (Sprite Renderer - for color)
│   │   └── 🔘 LevelBump3_Animator (Animator - for animation)
│   ├── 🟦 Square8 (Sprite Renderer + Animator)
│   ├── 🟦 Square9 (Sprite Renderer + Animator)
│   ├── 🔘 LevelBump4_Parent (Sprite Renderer - for color)
│   │   └── 🔘 LevelBump4_Animator (Animator - for animation)
│   ├── 🟦 Square10 (Sprite Renderer + Animator)
│   ├── 🟦 Square11 (Sprite Renderer + Animator)
│   └── 🔘 LevelBump5_Parent (Sprite Renderer - for color)
│       └── 🔘 LevelBump5_Animator (Animator - for animation)
└── 🎯 Player (Your player character)
```

---

### **Step 2: Animation Setup**

#### **For Line Sprites (Squares):**
1. **Add Animator Component**
   - Select line sprite → Add Component → Animator

2. **Create Line Animator Controller**
   - Right-click in Project → Create → Animator Controller
   - Name it "LineAnimator" (can reuse same controller for all line sprites)
   - Drag controller to Animator component

3. **Setup Line Animation States**
   - Open Animator window
   - Create state named **"Line_Idle"** (default/idle state - invisible)
   - Create state named **"Line"** (drawing animation state - animated)
   - Create state named **"Line_drawn"** (final state - visible/drawn permanently)
   - Add your line drawing animation clip to the "Line" state
   - Leave "Line_Idle" empty (invisible) and "Line_drawn" with final frame (visible)
   - **No triggers needed!** The SimpleAnimationSequencer handles everything

4. **Set Default State**
   - Right-click **"Line_Idle"** state → "Set as Layer Default State"

#### **For Bump Sprites (Level Unlocks):**
1. **Add Animator Component**
   - Select bump sprite → Add Component → Animator

2. **Create Bump Animator Controller**
   - Right-click in Project → Create → Animator Controller
   - Name it "BumpAnimator" (can reuse same controller for all bump sprites)
   - Drag controller to Animator component

3. **Setup Bump Animation States**
   - Open Animator window
   - Create state named **"Hidden"** (default/idle state - invisible)
   - Create state named **"Drawing"** (unlock animation state - animated)
   - Create state named **"Drawn"** (final state - visible/unlocked permanently)
   - Add your bump unlock animation clip to the "Drawing" state
   - Leave "Hidden" empty (invisible) and "Drawn" with final frame (visible)
   - **No triggers needed!** The SimpleAnimationSequencer handles everything

4. **Set Default State**
   - Right-click **"Hidden"** state → "Set as Layer Default State"

5. **Setup Parent-Child Structure (Important!)**
   - **Parent GameObject**: Has the **SpriteRenderer** component (for color changes)
   - **Child GameObject**: Has the **Animator** component (for animations)
   - The color changes happen on the parent, animations happen on the child

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
   
   Line Animation Settings:
   ├── Line Animation State Name: "Line"
   ├── Line Idle State Name: "Line_Idle"
   └── Line Drawn State Name: "Line_drawn"
   
   Bump Animation Settings:
   ├── Bump Animation State Name: "Drawing"
   ├── Bump Idle State Name: "Hidden"
   └── Bump Drawn State Name: "Drawn"
   
   Bump Color Settings:
   ├── Done Color: Green (completed levels)
   ├── Locked Color: Gray (future levels)
   ├── Unlocked Color: White (current level)
   └── Color Fade Duration: 1.0 (seconds)
   
   Final Completion Visuals:
   └── Final Completion Sprites: [Drag additional sprites for final completion]
   
   Timing Settings:
   └── Delay Between Animations: 0
   ```
   
   **For each Level Group:**
   ```
   Level Number: 1, 2, 3, 4, 5
   Level Name: "Level 1", "Level 2", etc.
   Line Animators: [Drag line sprites for THIS level completion]
   Bump Animator: [Drag bump CHILD (with Animator) for THIS level completion]
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
   
   All: Interaction Range = 3, Interaction Key = F
   Prompt Sprite: [Drag your custom interaction sprite here]
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
├── Line Animators: [Square1, Square2, Square3]
└── Bump Animator: [LevelBump1_Animator] (the CHILD with Animator)

Level Group 1:  
├── Level Number: 2
├── Level Name: "Level 2"
├── Line Animators: [Square4, Square5]
└── Bump Animator: [LevelBump2_Animator] (the CHILD with Animator)

Level Group 2:
├── Level Number: 3
├── Level Name: "Level 3"
├── Line Animators: [Square6, Square7, Square8, Square9] 
└── Bump Animator: [LevelBump3_Animator] (the CHILD with Animator)

... (continue for each level)
```

---

## 🎮 How It All Works

### **Game Flow:**
1. **Scene Loads** → All sprites start in idle/hidden state
2. **GameManager** loads saved progress:
   - **Completed levels**: Lines + bumps instantly appear drawn (green bumps)
   - **Current level**: Lines instantly appear drawn (no bump yet)
   - **Fresh Level 1 start**: Wait 3 seconds → draw level 1 lines (no bump)
   - **After Reset**: Immediately draw level 1 lines (no delay)
3. **Player approaches current level bump** → Custom interaction sprite appears
4. **Player completes level** → returns to world menu
5. **Level completion sequence**:
   - **Completed level bump**: Animates and turns green (done)
   - **Next level lines**: Draw sequentially (Line_Idle → Line → Line_drawn)
   - **Next level bump**: Color fades from gray to white (unlocked)
   - **Next level bump animation**: Stays hidden until that level is completed
6. **Progressive drawing** → Only draw up to current progress, bumps only after completion

### **Animation Sequences:**

#### **Scene Load (Level 1):**
```
Scene loads → Wait 3 seconds →
Square1: Line_Idle → Line → Line_drawn
Square2: Line_Idle → Line → Line_drawn  
Square3: Line_Idle → Line → Line_drawn
(Level 1 bump stays hidden until level is completed)
```

#### **Level 1 Completion:**
```
Player completes Level 1 → PlayLevelCompletionAnimation(1) →

Phase 1 - Complete Level 1:
LevelBump1: Hidden → Drawing → Drawn + Color: Gray → Green
(Level 1 lines already drawn - no redraw needed)

Phase 2 - Prepare Level 2 (PARALLEL):
├── Square4: Line_Idle → Line → Line_drawn
├── Square5: Line_Idle → Line → Line_drawn  
└── Level2Bump: Color fade Gray → White (SIMULTANEOUS)

(Smooth parallel flow - lines and colors animate together!)
```

#### **Level 5 Completion (Final):**
```
Player completes Level 5 → PlayLevelCompletionAnimation(5) →

Phase 1 - Complete Level 5:
LevelBump5: Hidden → Drawing → Drawn + Color: Gray → Green
(NO level 5 lines redraw - they're already drawn!)

Phase 2 - Final Completion Visuals:
FinalSprite1: Line_Idle → Line → Line_drawn
FinalSprite2: Line_Idle → Line → Line_drawn
FinalSprite3: Line_Idle → Line → Line_drawn
(Visual completion - no more levels, just celebration!)

✅ Fixed: Level 5 completion now correctly triggers final visuals instead of attempting to draw "level 6" content
```

### **🎨 Visual Result:**
- **Progressive line drawing**: Lines draw up to current progress (no further)
- **Bump revelation**: Bumps only appear after their level is completed
- **Color coding**: 
  - **Gray bumps**: Future locked levels (hidden until lines reach them)
  - **White bump**: Current level available to play (lines drawn, bump hidden)
  - **Green bumps**: Completed levels (lines + bump both visible)
  - **Smooth progression**: Lines lead the way, bumps confirm completion

---

## 🧪 Testing & Debug

### **Testing Level Progression:**
1. **Play the scene**
2. **GameManager** → Right-click → "Complete Current Level"
3. **Watch line draw** to next bump perfectly!
4. **Walk to bump** → see interaction sprite

### **Debug Methods Available:**
- **GameManager**: "Complete Current Level", "Reset Progress", "Complete Level 5" *(for testing final completion)*
- **SimpleAnimationSequencer**: "Debug Level Groups" *(for verifying setup)*

---

## ✅ Troubleshooting

### **Animations Not Playing:**
- **Line sprites**: Check state names "Line", "Line_Idle", "Line_drawn" 
- **Bump sprites**: Check state names "Drawing", "Hidden", "Drawn"
- Verify Level Numbers are set correctly (1, 2, 3, 4, 5)
- Use "Debug Level Groups" to check your setup
- Ensure animation clips are properly assigned
- Make sure default states are set: "Line_Idle" for lines, "Hidden" for bumps

### **Animation Jittering/Flickering:**
- This is fixed with improved timing in the animation system
- If still occurring, check animation clip settings and ensure smooth transitions
- Make sure "Line_drawn" state has proper setup with final frame

### **Final Completion Visuals Not Playing:**
- Check that **Final Completion Sprites array** is populated in SimpleAnimationSequencer
- Use **GameManager → "Complete Level 5"** to test the complete flow
- Verify **MaxLevels is set correctly** in GameManager (should be 5)
- Check debug logs for "Just completed the final level" message
- **Level 5 Logic**: Level 5 completion is handled separately from other levels to ensure proper final completion flow
- Level 5 bump should turn green (done color) when final completion visuals start

### **Colors Not Changing:**
- Verify **parent GameObjects** of bump animators have **SpriteRenderer** components
- Check parent-child structure: Parent (SpriteRenderer) → Child (Animator)
- Check "Test Update Bump Colors" to manually trigger color update
- Ensure GameManager.Instance is available when scene loads
- Verify Level Numbers in Level Groups match expected values

### **Level Bumps Not Responding:**
- Verify player has "Player" tag
- Check Collider2D is set as Trigger  
- Confirm Level Numbers are set correctly (1-5)

### **Interaction Sprite Not Showing:**
- **Critical**: Make sure your player GameObject has the **"Player" tag**
- **Critical**: Make sure you've assigned a **Prompt Sprite** (Texture2D) in the LevelBump Inspector
- Check that player is within Interaction Range (adjust in Inspector if needed)
- **Level Logic**: Only the CURRENT level bump shows the interaction sprite:
  - If current level = 1 → Only Level 1 bump shows interaction sprite
  - If current level = 2 → Only Level 2 bump shows interaction sprite
  - Other bumps show nothing (locked or completed levels)
- Check Console for helpful debug messages:
  - ✅ "Showing interaction sprite" = Working correctly
  - 🔒 "This level is locked" = Walk to current level bump instead  
  - ✅ "This level is already completed" = Walk to current level bump instead
- **Sprite Requirements**: Use any Texture2D asset (PNG, JPG, etc.) - the system will display it at native size
- **Changed**: Now uses **F key** instead of Space to avoid accidental level loading when jumping

### **Progress Not Saving:**
- GameManager persists between scenes automatically  
- Progress saves to PlayerPrefs automatically

### **Reset Not Working:**
- GameManager "Reset Progress" automatically resets both game data AND animation states
- If animations don't reset, check that WorldLevelManager is listening to OnProgressReset event
- **Fixed**: Reset now properly clears Level 5 completion tracking to prevent final completion sprites from playing during reset
- Only Level 1 lines should draw after reset (no delay)
- Final completion sprites and Level 5 content should NOT appear during reset

---

## 🎯 You're Done!

Your complete game jam world menu system is now ready with:
✅ Compact, readable code  
✅ Perfect sync animations  
✅ HFSM state management  
✅ Smart level progression  
✅ Proximity-based UI  

**Happy game jamming!** 🎮
