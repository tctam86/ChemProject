Ground detection — Editor UI setup and verification (for MiniPlatformerController)

This document shows exact Unity Editor steps to create and assign the ground check Transform and set the ground Layer/LayerMask used by [`Assets/Scripts/MiniPlatformerController.cs`](Assets/Scripts/MiniPlatformerController.cs:1).

1. Create the ground layer (if not already present)

- In the Unity Editor menu: Edit → Project Settings → Tags and Layers.
- Under "User Layers", pick an empty slot and add a new layer named: Ground
- Press Enter / click away to save.

2. Assign the Ground layer to ground tiles / level geometry

- In the Hierarchy, select the tilemap or ground GameObjects that represent floor/platforms.
- In the Inspector, set Layer → choose "Ground".
- If prompted, choose "Yes, change children" if you want child objects to use the same layer.

3. Add a groundCheck Transform to the Player prefab / GameObject

- In the Hierarchy, expand your Player GameObject (for example the prefab used in the scene).
- Right-click the Player GameObject → Create Empty. A child GameObject will be created.
- Rename the child to GroundCheck (exact name not required, but helpful).
- Select the GroundCheck child and in the Inspector set its Position (local) to be at the player's feet.
  - Typical Y offset values: around -0.5 to -1.0 depending on the sprite pivot / scale.
  - Example: if Player transform is (0,0,0), set GroundCheck localPosition to (0, -0.6, 0).

4. Configure the groundCheck radius and LayerMask in the script Inspector

- Select the Player GameObject (the one with the [`Assets/Scripts/MiniPlatformerController.cs`](Assets/Scripts/MiniPlatformerController.cs:1) component).
- In the Inspector, find the MiniPlatformerController component.
- Assign the new child to the "Ground Check" field by dragging the GroundCheck child into the Ground Check slot.
  - This assigns the Transform reference used by Physics2D.OverlapCircle.
- Set "Ground Check Radius" to a small value like 0.18–0.25 (default recommended 0.2).
- For "Ground Layer" (LayerMask) open the dropdown and select only the "Ground" layer you created.
  - Make sure other layers (Player, Enemies, Walls) are not selected unless you want them treated as ground.

5. Verify the GroundCheck placement visually

- With the GroundCheck child selected, enable the Scene Gizmos button (top-right of Scene view).
- Optionally add a small visual: add a SpriteRenderer temporarily or use a visible gizmo script to show the check position.
- Ensure GroundCheck sits just below the player's feet and the radius doesn't overlap walls or ceilings unnecessarily.

6. Common pitfalls and how to check them

- groundLayer not set: If LayerMask includes no layers, OverlapCircle will always return false.
- Ground tiles not on Ground layer: If ground objects are on Default or other layers, ground check will miss them.
- GroundCheck too high/low: If positioned inside the player's collider or far below the ground, detection will be incorrect.
- Radius too large: Large radius can cause false positives (e.g., detecting walls); reduce radius and re-test.
- Player/GameObject mismatch: If you assign PlayerInput or run the player prefab instance, ensure the same prefab instance in the Scene has the configured MiniPlatformerController.

7. Quick manual verification in Play mode (no code changes)

- Open Window → General → Console and clear logs.
- Enter Play mode.
- Walk the player onto a flat ground and stand still. If you added the Debug.Log entry suggested earlier, you should see logs like:
  - "[JumpTest] CheckGroundStatus isGrounded=True"
  - If you did not add logs, visually confirm the character can jump when pressing Space (after binding).
- Walk off a platform and observe whether the controller registers leaving ground (if logs present, see isGrounded flip to False).

8. If ground check still fails — checklist to report back

- Confirm you created a Layer named "Ground" and assigned it to ground tiles.
- Confirm the Player's MiniPlatformerController inspector shows:
  - Ground Check = [`Assets/Prefabs/Player.prefab`](Assets/Prefabs/Player.prefab:1) (or scene instance) → GroundCheck child Transform assigned
  - Ground Check Radius = 0.18–0.25 (or your tuned value)
  - Ground Layer = "Ground" LayerMask selected
- Confirm ground tiles' GameObjects Layer = Ground
- Confirm Player has a Rigidbody2D (Body Type = Dynamic, Gravity Scale > 0)
- If you see unexpected behavior, copy the exact MiniPlatformerController Inspector values and a screenshot of the Player and GroundCheck positions.

9. Optional: visualize the OverlapCircle in Scene view (one-time code helper)

- If you want a persistent visual, add this small method to the script temporarily:
  - Gizmos.color = Color.red; Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
- This helps ensure the check area is placed and sized correctly before running Play.

References

- Implementation file: [`Assets/Scripts/MiniPlatformerController.cs`](Assets/Scripts/MiniPlatformerController.cs:1)
- Player prefab (example): [`Assets/Prefabs/Player.prefab`](Assets/Prefabs/Player.prefab:1)

Follow the above Editor steps and report the Inspector values or any differences you see. I will use that information to diagnose why the jump isn't occurring.
