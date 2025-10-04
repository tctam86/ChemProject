# Jump System Architecture for MiniPlatformerController

## Overview

This document outlines the comprehensive design for implementing jump functionality in [`MiniPlatformerController.cs`](../Assets/Scripts/MiniPlatformerController.cs) with coyote time and jump buffer mechanics for enhanced player experience.

## Current State Analysis

### Existing Implementation

- **Input System**: Unity's new Input System with [`Chemistry.inputactions`](../Assets/Chemistry.inputactions)
- **Jump Action**: Already configured (Space key binding, line 290-297)
- **Controller**: Basic horizontal movement implemented
- **Animator**: [`Player_mini.controller`](../Assets/Animations/Player_mini.controller) has `isJump` parameter (line 95-100)
- **Physics**: Uses `Rigidbody2D` with `linearVelocity` property

### Requirements

1. **Coyote Time**: Allow jumping shortly after leaving a platform
2. **Jump Buffer**: Queue jump inputs for execution upon landing
3. **Ground Detection**: Reliable system to track grounded state
4. **Physics Integration**: Smooth jump arc with Rigidbody2D
5. **Animation Integration**: Trigger jump animations via Animator

---

## 1. Ground Detection System

### Design Approach

Use **LayerMask-based ground check** with configurable detection parameters.

### Architecture

```
┌─────────────────────────────────────┐
│     Ground Detection System         │
├─────────────────────────────────────┤
│ Components:                         │
│ • Ground Check Transform            │
│ • Ground Check Radius               │
│ • Ground Layer Mask                 │
│                                     │
│ Method: Physics2D.OverlapCircle()   │
│                                     │
│ Returns: bool isGrounded            │
└─────────────────────────────────────┘
```

### Implementation Specifications

**Variables Required:**

```csharp
[Header("Ground Detection")]
[SerializeField] private Transform groundCheck;
[SerializeField] private float groundCheckRadius = 0.2f;
[SerializeField] private LayerMask groundLayer;

private bool isGrounded;
```

**Ground Check Logic:**

- **Method**: [`Physics2D.OverlapCircle()`](https://docs.unity3d.com/ScriptReference/Physics2D.OverlapCircle.html)
- **Position**: At player's feet (child Transform object)
- **Radius**: 0.2f units (configurable)
- **Frequency**: Check every `FixedUpdate()` for physics consistency
- **Layer**: Filter to only detect ground layer

**Setup Requirements:**

1. Create empty GameObject as child of player
2. Position at feet (offset Y: -0.5 to -1.0 depending on sprite)
3. Assign to `groundCheck` field
4. Create "Ground" layer in Unity
5. Assign ground tiles to "Ground" layer

---

## 2. Coyote Time Implementation

### Concept

Coyote time gives players a grace period (typically 0.1-0.2 seconds) after leaving a platform where they can still initiate a jump. This makes platforming feel more forgiving and responsive.

### Architecture

```
┌──────────────────────────────────────────────────────┐
│           Coyote Time State Machine                  │
├──────────────────────────────────────────────────────┤
│                                                      │
│  ┌─────────────┐  Leave Ground   ┌──────────────┐  │
│  │  GROUNDED   │ ──────────────> │ COYOTE TIME  │  │
│  │             │                 │   ACTIVE     │  │
│  └─────────────┘                 └──────────────┘  │
│         ▲                               │           │
│         │                               │           │
│         │  Touch Ground                 │ Timer     │
│         │                               │ Expires   │
│         │                               ▼           │
│         │                        ┌──────────────┐  │
│         └────────────────────────│   AIRBORNE   │  │
│                                  │ (No Coyote)  │  │
│                                  └──────────────┘  │
└──────────────────────────────────────────────────────┘
```

### Implementation Specifications

**Variables Required:**

```csharp
[Header("Coyote Time")]
[SerializeField] private float coyoteTime = 0.15f;

private float coyoteTimeCounter;
private bool wasGrounded;
```

**Core Logic Flow:**

1. **State Tracking**:
   - Track previous frame's grounded state
   - Detect transition from grounded to airborne
2. **Timer Management**:
   - Initialize counter when leaving ground: `coyoteTimeCounter = coyoteTime`
   - Decrement in `Update()`: `coyoteTimeCounter -= Time.deltaTime`
   - Reset when grounded: `coyoteTimeCounter = coyoteTime`
3. **Jump Permission Check**:
   ```csharp
   bool CanJump() {
       return isGrounded || coyoteTimeCounter > 0f;
   }
   ```

**Timing Recommendations:**

- **Standard**: 0.15 seconds (good for most platformers)
- **Forgiving**: 0.2 seconds (easier, more casual)
- **Tight**: 0.1 seconds (harder, more precise)

---

## 3. Jump Buffer Implementation

### Concept

Jump buffer captures jump input before landing and automatically executes it upon touchdown. This prevents "lost" inputs when players press jump slightly before landing.

### Architecture

```
┌────────────────────────────────────────────────┐
│         Jump Buffer Input Queue                │
├────────────────────────────────────────────────┤
│                                                │
│   Player Presses Jump                          │
│         │                                      │
│         ▼                                      │
│   ┌─────────────┐                             │
│   │ Is Grounded?│                             │
│   └─────────────┘                             │
│    │           │                               │
│   Yes          No                              │
│    │           │                               │
│    │           ▼                               │
│    │    ┌──────────────┐                      │
│    │    │ Start Buffer │                      │
│    │    │    Timer     │                      │
│    │    └──────────────┘                      │
│    │           │                               │
│    │           ▼                               │
│    │    ┌──────────────┐    Timer    ┌─────┐ │
│    │    │ Wait for     │───Expires──>│Clear│ │
│    │    │  Landing     │             └─────┘ │
│    │    └──────────────┘                      │
│    │           │                               │
│    │      Land Detected                        │
│    │           │                               │
│    ▼           ▼                               │
│   ┌─────────────────┐                         │
│   │  Execute Jump   │                         │
│   └─────────────────┘                         │
│                                                │
└────────────────────────────────────────────────┘
```

### Implementation Specifications

**Variables Required:**

```csharp
[Header("Jump Buffer")]
[SerializeField] private float jumpBufferTime = 0.2f;

private float jumpBufferCounter;
```

**Core Logic Flow:**

1. **Input Capture**:
   - Detect jump button press via `OnJump(InputValue)` callback
   - Set buffer timer: `jumpBufferCounter = jumpBufferTime`
2. **Buffer Timer Management**:
   - Decrement in `Update()`: `jumpBufferCounter -= Time.deltaTime`
   - Persist across frames until expired or consumed
3. **Buffer Consumption**:
   - Check on landing: `if (isGrounded && jumpBufferCounter > 0f)`
   - Execute jump immediately
   - Reset buffer: `jumpBufferCounter = 0f`
4. **Integration with Jump Execution**:
   ```csharp
   bool ShouldJump() {
       return jumpBufferCounter > 0f && CanJump();
   }
   ```

**Timing Recommendations:**

- **Standard**: 0.2 seconds (balanced responsiveness)
- **Generous**: 0.3 seconds (very forgiving)
- **Strict**: 0.15 seconds (requires precise timing)

**Edge Cases:**

- Clear buffer after successful jump to prevent double jumps
- Clear buffer if player lands and doesn't jump within window
- Buffer should work with coyote time for maximum responsiveness

---

## 4. Jump Physics Integration

### Design Approach

Use **impulse-based jump** with configurable height and gravity modifiers.

### Architecture

```
┌──────────────────────────────────────────────┐
│         Jump Physics System                  │
├──────────────────────────────────────────────┤
│                                              │
│  Input: Jump Force (m/s)                     │
│         ↓                                    │
│  ┌──────────────────┐                       │
│  │ Apply Y Velocity │                       │
│  │ via Rigidbody2D  │                       │
│  └──────────────────┘                       │
│         ↓                                    │
│  ┌──────────────────┐                       │
│  │ Preserve X       │                       │
│  │ Momentum         │                       │
│  └──────────────────┘                       │
│         ↓                                    │
│  ┌──────────────────┐                       │
│  │ Optional: Better │                       │
│  │ Jump Modifiers   │                       │
│  └──────────────────┘                       │
│         ↓                                    │
│  Result: Parabolic Arc                       │
│                                              │
└──────────────────────────────────────────────┘
```

### Implementation Specifications

**Variables Required:**

```csharp
[Header("Jump Physics")]
[SerializeField] private float jumpForce = 12f;
[SerializeField] private float fallMultiplier = 2.5f;
[SerializeField] private float lowJumpMultiplier = 2f;
```

**Jump Execution:**

```csharp
void PerformJump() {
    // Apply jump force while preserving horizontal velocity
    Vector2 velocity = myRigidbody2D.linearVelocity;
    velocity.y = jumpForce;
    myRigidbody2D.linearVelocity = velocity;

    // Reset state counters
    coyoteTimeCounter = 0f;
    jumpBufferCounter = 0f;

    // Trigger animation
    UpdateJumpAnimation(true);
}
```

**Better Jump Feel (Optional Enhancement):**
Modify gravity based on jump button hold for variable jump height:

```csharp
void ApplyBetterJump() {
    if (myRigidbody2D.linearVelocity.y < 0) {
        // Falling - increase gravity
        myRigidbody2D.linearVelocity += Vector2.up * Physics2D.gravity.y *
                                        (fallMultiplier - 1) * Time.deltaTime;
    } else if (myRigidbody2D.linearVelocity.y > 0 && !jumpHeld) {
        // Rising but button released - reduce jump height
        myRigidbody2D.linearVelocity += Vector2.up * Physics2D.gravity.y *
                                        (lowJumpMultiplier - 1) * Time.deltaTime;
    }
}
```

**Physics Settings:**

- **Jump Force**: 12f (adjust based on gravity scale)
- **Gravity Scale**: 3-4 recommended for responsive platforming
- **Fall Multiplier**: 2.5f (faster falling)
- **Low Jump Multiplier**: 2f (shorter taps = shorter jumps)

---

## 5. Animator Integration

### Current Animator State

- Parameter: `isJump` (Bool, line 95-100 in Player_mini.controller)
- No jump animation state currently configured

### Architecture

```
┌────────────────────────────────────────────────┐
│         Animator State Machine                 │
├────────────────────────────────────────────────┤
│                                                │
│  ┌──────────┐  isJump=true   ┌─────────────┐ │
│  │   Idle/  │ ─────────────> │    Jump     │ │
│  │  Walking │                │  Animation  │ │
│  └──────────┘                └─────────────┘ │
│       ▲                            │          │
│       │                            │          │
│       │      isGrounded=true       │          │
│       └────────────────────────────┘          │
│                                                │
│  Additional Parameters:                        │
│  • isGrounded (Bool) - for landing detection  │
│  • verticalVelocity (Float) - for fall/rise   │
│                                                │
└────────────────────────────────────────────────┘
```

### Implementation Specifications

**Animator Parameters to Add:**

```csharp
// In Animator Controller
- isJump (Bool) - Already exists
- isGrounded (Bool) - NEW
- verticalVelocity (Float) - OPTIONAL for blend trees
```

**Animation States to Create:**

1. **Jump Start** - Initial jump frame(s)
2. **Jump Rising** - While moving upward
3. **Jump Peak** - At apex of jump
4. **Jump Falling** - While falling down
5. **Jump Landing** - Landing impact frame(s)

**Code Integration:**

```csharp
private Animator animator;

void Start() {
    animator = GetComponent<Animator>();
}

void UpdateJumpAnimation(bool jumping) {
    animator.SetBool("isJump", jumping);
    animator.SetBool("isGrounded", isGrounded);
}

void Update() {
    // Update grounded state
    animator.SetBool("isGrounded", isGrounded);

    // Optional: Update velocity for blend trees
    animator.SetFloat("verticalVelocity", myRigidbody2D.linearVelocity.y);
}
```

**Transition Conditions:**

- Idle/Walking → Jump: `isJump == true`
- Jump → Idle/Walking: `isGrounded == true && verticalVelocity ≈ 0`
- Auto-transition within jump phases based on verticalVelocity

---

## 6. Input Handling Integration

### Current Input System

- Uses Unity's Input System with Player Input Component
- Jump action bound to Space key (line 290-297 in Chemistry.inputactions)
- Callback method pattern: `OnJump(InputValue)`

### Implementation Specifications

**Input Callback Method:**

```csharp
private bool jumpPressed;

void OnJump(InputValue inputValue) {
    // Capture jump input
    jumpPressed = inputValue.isPressed;

    // Set jump buffer if not grounded
    if (jumpPressed) {
        jumpBufferCounter = jumpBufferTime;
    }
}
```

**Input Processing in Update:**

```csharp
void Update() {
    Run();
    HandleJump();
}

void HandleJump() {
    // Check if jump should be executed
    if (ShouldJump()) {
        PerformJump();
    }

    // Update timers
    UpdateCoyoteTime();
    UpdateJumpBuffer();

    // Optional: Better jump feel
    ApplyBetterJump();
}

private bool ShouldJump() {
    return jumpBufferCounter > 0f && CanJump();
}

private bool CanJump() {
    return isGrounded || coyoteTimeCounter > 0f;
}
```

---

## 7. Complete System Integration Flow

### Update Loop Architecture

```
FixedUpdate()
├─ CheckGroundStatus()
│  └─ Physics2D.OverlapCircle()
│
Update()
├─ UpdateCoyoteTime()
│  ├─ If wasGrounded && !isGrounded
│  │  └─ Start coyote timer
│  └─ Decrement timer
│
├─ UpdateJumpBuffer()
│  └─ Decrement buffer timer
│
├─ HandleJump()
│  ├─ If ShouldJump()
│  │  └─ PerformJump()
│  └─ ApplyBetterJump()
│
├─ Run()
│  └─ Handle horizontal movement
│
└─ UpdateAnimations()
   └─ Sync animator parameters
```

### State Diagram

```
                    ┌─────────────┐
                    │   GROUNDED  │
                    │ coyoteTime  │
                    │   = max     │
                    └──────┬──────┘
                           │
                    Jump Input or
                    Leave Ground
                           │
                           ▼
                    ┌─────────────┐
              ┌────>│  AIRBORNE   │
              │     │ coyoteTime  │
              │     │  counting   │
              │     └──────┬──────┘
              │            │
              │      Landing Detected
              │            │
              │            ▼
    Jump      │     ┌─────────────┐
    Buffer    │     │   LANDING   │
    Active    │     │ Check buffer│
              │     └──────┬──────┘
              │            │
              │      Buffer Active?
              │            │
              │         Yes│  No
              │            ▼   │
              └────────────┐   │
                           │   │
                           │   ▼
                    Jump Again or Return to Ground
```

---

## 8. Configuration Recommendations

### Serialized Field Defaults

```csharp
[Header("Movement")]
[SerializeField] private float moveSpeed = 5f;

[Header("Jump Physics")]
[SerializeField] private float jumpForce = 12f;
[SerializeField] private float fallMultiplier = 2.5f;
[SerializeField] private float lowJumpMultiplier = 2f;

[Header("Ground Detection")]
[SerializeField] private Transform groundCheck;
[SerializeField] private float groundCheckRadius = 0.2f;
[SerializeField] private LayerMask groundLayer;

[Header("Jump Timing")]
[SerializeField] private float coyoteTime = 0.15f;
[SerializeField] private float jumpBufferTime = 0.2f;
```

### Tuning Guidelines

**For Responsive Platformer:**

- Jump Force: 12-15f
- Coyote Time: 0.15-0.2s
- Buffer Time: 0.2-0.3s
- Fall Multiplier: 2.5-3f

**For Precise Platformer:**

- Jump Force: 10-12f
- Coyote Time: 0.1s
- Buffer Time: 0.15s
- Fall Multiplier: 2f

**For Casual/Forgiving:**

- Jump Force: 13-16f
- Coyote Time: 0.2-0.25s
- Buffer Time: 0.3-0.4s
- Fall Multiplier: 2.5f

---

## 9. Testing Checklist

### Functionality Tests

- [ ] Player can jump when grounded
- [ ] Player can jump during coyote time window
- [ ] Jump buffer captures early inputs
- [ ] Buffered jump executes on landing
- [ ] Cannot jump when airborne (outside coyote time)
- [ ] Variable jump height works (hold vs tap)
- [ ] Horizontal movement preserved during jump
- [ ] Animation transitions are smooth

### Edge Cases

- [ ] Rapid jump button spam doesn't cause issues
- [ ] Walking off platform still allows coyote jump
- [ ] Jumping from moving platforms works
- [ ] Landing on slopes behaves correctly
- [ ] Multiple rapid landings don't cause animation glitches

### Performance

- [ ] No frame drops during jump execution
- [ ] Ground check doesn't cause stuttering
- [ ] Timer updates are smooth and predictable

---

## 10. Implementation Order

### Recommended Step-by-Step Implementation

1. **Ground Detection** (Foundation)

   - Add ground check Transform
   - Implement ground detection logic
   - Test grounded state tracking

2. **Basic Jump** (Core Functionality)

   - Add jump input handler
   - Implement basic jump force application
   - Test jump execution when grounded

3. **Coyote Time** (Polish Layer 1)

   - Add coyote time tracking
   - Integrate with jump permission check
   - Test jumping after leaving platform

4. **Jump Buffer** (Polish Layer 2)

   - Add buffer timer system
   - Integrate with jump execution
   - Test pre-landing jump inputs

5. **Better Jump Physics** (Enhancement)

   - Add gravity modifiers
   - Implement variable jump height
   - Tune feel and responsiveness

6. **Animation Integration** (Visual Polish)
   - Add animator parameters
   - Create animation states
   - Wire up transitions
   - Test animation synchronization

---

## 11. Code Structure Overview

### Complete Variable Declaration Block

```csharp
// Movement
private Vector2 moveInput;
private Rigidbody2D myRigidbody2D;
[SerializeField] private float moveSpeed = 5f;

// Jump Physics
[SerializeField] private float jumpForce = 12f;
[SerializeField] private float fallMultiplier = 2.5f;
[SerializeField] private float lowJumpMultiplier = 2f;

// Ground Detection
[SerializeField] private Transform groundCheck;
[SerializeField] private float groundCheckRadius = 0.2f;
[SerializeField] private LayerMask groundLayer;
private bool isGrounded;
private bool wasGrounded;

// Timing Systems
[SerializeField] private float coyoteTime = 0.15f;
[SerializeField] private float jumpBufferTime = 0.2f;
private float coyoteTimeCounter;
private float jumpBufferCounter;

// Input
private bool jumpPressed;
private bool jumpHeld;

// Components
private Animator animator;
```

### Method Organization

```
MiniPlatformerController
├─ Unity Lifecycle
│  ├─ Start()
│  ├─ Update()
│  └─ FixedUpdate()
│
├─ Input Handlers
│  ├─ OnMove(InputValue)
│  └─ OnJump(InputValue)
│
├─ Movement
│  └─ Run()
│
├─ Ground Detection
│  └─ CheckGroundStatus()
│
├─ Jump System
│  ├─ HandleJump()
│  ├─ PerformJump()
│  ├─ CanJump()
│  ├─ ShouldJump()
│  └─ ApplyBetterJump()
│
├─ Timer Management
│  ├─ UpdateCoyoteTime()
│  └─ UpdateJumpBuffer()
│
└─ Animation
   └─ UpdateJumpAnimation(bool)
```

---

## 12. Potential Issues & Solutions

### Issue 1: Ground Check False Positives

**Problem**: Player detects ground when touching walls
**Solution**:

- Narrow ground check radius
- Position ground check lower
- Use specific ground layer only

### Issue 2: Jump Feels Floaty

**Problem**: Character hangs in air too long
**Solution**:

- Increase gravity scale on Rigidbody2D
- Increase fall multiplier
- Reduce jump force

### Issue 3: Can't Jump Off Edges

**Problem**: Coyote time too short or not implemented
**Solution**:

- Increase coyote time value
- Verify coyote time logic is in Update(), not FixedUpdate()

### Issue 4: Missed Jump Inputs

**Problem**: Jump buffer too short or not working
**Solution**:

- Increase buffer time
- Ensure buffer persists across frames
- Check buffer is consumed after jump execution

### Issue 5: Double Jumps Occurring

**Problem**: Jump buffer or coyote time not cleared after jump
**Solution**:

- Reset both counters to 0 after PerformJump()
- Ensure single-frame jump execution

---

## Summary

This architecture provides a comprehensive, production-ready jump system with:

✅ **Coyote Time** - Forgives edge jumps  
✅ **Jump Buffer** - Captures early inputs  
✅ **Robust Ground Detection** - LayerMask-based reliability  
✅ **Variable Jump Height** - Button hold duration affects jump  
✅ **Smooth Animation Integration** - Full animator support  
✅ **Tunable Parameters** - Easy difficulty adjustment  
✅ **Modular Design** - Each system can be implemented independently

The implementation order prioritizes functionality first, then polish, allowing for iterative development and testing at each stage.
