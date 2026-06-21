# Player FSM — State Transition Reference

## Inheritance Structure

```
PlayerState (base)
├── PlayerGroundedState        ← shared grounded logic
│   ├── PlayerIdleState
│   └── PlayerMoveState
├── PlayerAirBoneState         ← shared airborne logic
│   ├── PlayerJumpState
│   ├── PlayerFallState
│   └── PlayerWallJumpState
├── PlayerWallSlideState
├── PlayerAttackState
├── PlayerJumpAttackState
├── PlayerDashState
├── PlayerCounterState
├── PlayerDismantleState
├── PlayerDomainExpansionState
├── PlayerKnockBackState
└── PlayerDeadState
```

> **Ghi chú kế thừa:** `PlayerGroundedState.Update()` và `PlayerAirBoneState.Update()` được gọi qua `base.Update()`,
> nghĩa là mọi transition được định nghĩa ở class cha đều **có hiệu lực trong tất cả class con** của nó.

---

## Transition toàn bộ

---

### IDLE
> `PlayerIdleState.cs` — kế thừa transitions từ `PlayerGroundedState.cs`

| # | Condition | → State | Nguồn |
|---|---|---|---|
| 1 | `movementInput.x != 0 && !(isTouchingWall && direction == movementInput.x sign)` | **Move** | `PlayerIdleState.Update()` |
| 2 | `Jump pressed + canAttack + BasicAttack pressed` | **JumpAttack** *(IsFromGround=true, IsUpAttack=false)* | `PlayerGroundedState.Update()` |
| 3 | `Jump pressed` | **Jump** | `PlayerGroundedState.Update()` |
| 4 | `canAttack && BasicAttack pressed` | **Attack** | `PlayerGroundedState.Update()` |
| 5 | `!isGrounded` | **Fall** | `PlayerGroundedState.Update()` |
| 6 | Skill slot Dash được kích hoạt | **Dash** | `PlayerGroundedState.Update()` — `SkillButtonHandler.TryConsumeSkill` |
| 7 | Skill slot Counter được kích hoạt | **Counter** | `PlayerGroundedState.Update()` — `SkillButtonHandler.TryConsumeSkill` |
| 8 | Skill slot Domain được kích hoạt | **DomainExpansion** | `PlayerGroundedState.Update()` — `SkillButtonHandler.TryConsumeSkill` |
| 9 | Skill slot Dismantle được kích hoạt | **Dismantle** | `PlayerGroundedState.Update()` — `SkillButtonHandler.TryConsumeSkill` |
| 10 | *(external)* `PlayerHealth`: nhận đòn knockback | **KnockBack** | `PlayerHealth.cs` — gọi trực tiếp `stateMachine.ChangeState` |
| 11 | *(external)* `Player`: hp ≤ 0 | **Dead** | `Player.cs` — gọi trực tiếp `stateMachine.ChangeState` |

---

### MOVE
> `PlayerMoveState.cs` — kế thừa transitions từ `PlayerGroundedState.cs`

| # | Condition | → State | Nguồn |
|---|---|---|---|
| 1 | `movementInput.x == 0 \|\| isTouchingWall` | **Idle** | `PlayerMoveState.Update()` |
| 2 | `Jump pressed + canAttack + BasicAttack pressed` | **JumpAttack** *(IsFromGround=true, IsUpAttack=false)* | `PlayerGroundedState.Update()` |
| 3 | `Jump pressed` | **Jump** | `PlayerGroundedState.Update()` |
| 4 | `canAttack && BasicAttack pressed` | **Attack** | `PlayerGroundedState.Update()` |
| 5 | `!isGrounded` | **Fall** | `PlayerGroundedState.Update()` |
| 6 | Skill slot Dash/Counter/Domain/Dismantle được kích hoạt | *(xem Idle #6–9)* | `PlayerGroundedState.Update()` |
| 7 | *(external)* nhận đòn knockback | **KnockBack** | `PlayerHealth.cs` |
| 8 | *(external)* hp ≤ 0 | **Dead** | `Player.cs` |

---

### JUMP
> `PlayerJumpState.cs` — kế thừa transitions từ `PlayerAirBoneState.cs`

| # | Condition | → State | Nguồn |
|---|---|---|---|
| 1 | `isTouchingWall && !isGrounded && direction != LastWallJumpDirection` | **WallSlide** | `PlayerJumpState.Update()` |
| 2 | `rb.velocity.y < 0` | **Fall** | `PlayerJumpState.Update()` |
| 3 | `canAirAttack && BasicAttack pressed` | **JumpAttack** *(IsFromGround=false, IsUpAttack = movementInput.y > 0)* | `PlayerAirBoneState.Update()` |
| 4 | Skill slot Dash được kích hoạt | **Dash** | `PlayerAirBoneState.Update()` — `TryConsumeSkill(ButtonSkillName.Dash)` |
| 5 | Skill slot Domain được kích hoạt | **DomainExpansion** | `PlayerAirBoneState.Update()` — `TryConsumeSkill(ButtonSkillName.Domain)` |
| 6 | Skill slot Dismantle được kích hoạt | **Dismantle** | `PlayerAirBoneState.Update()` — `TryConsumeSkill(ButtonSkillName.Dismantle)` |
| 7 | *(external)* nhận đòn knockback | **KnockBack** | `PlayerHealth.cs` |

> **Ghi chú Enter:** Mỗi lần vào Jump → `JumpCount++`, set `velocity.y = jumpForce`.

---

### FALL
> `PlayerFallState.cs` — kế thừa transitions từ `PlayerAirBoneState.cs`

| # | Condition | → State | Nguồn |
|---|---|---|---|
| 1 | `Jump pressed && JumpCount < MaxJumpCount` | **Jump** *(double jump)* | `PlayerFallState.Update()` |
| 2 | `isTouchingWall && !isGrounded && direction != LastWallJumpDirection` | **WallSlide** | `PlayerFallState.Update()` |
| 3 | `isGrounded && movementInput.x != 0` | **Move** | `PlayerFallState.Update()` |
| 4 | `isGrounded && movementInput.x == 0` | **Idle** | `PlayerFallState.Update()` |
| 5 | `canAirAttack && BasicAttack pressed` | **JumpAttack** *(IsFromGround=false)* | `PlayerAirBoneState.Update()` |
| 6 | Skill slot Dash/Domain/Dismantle được kích hoạt | *(xem Jump #4–6)* | `PlayerAirBoneState.Update()` |
| 7 | *(external)* nhận đòn knockback | **KnockBack** | `PlayerHealth.cs` |

> **Ghi chú đặc biệt:** Nếu `previousState == Dash` khi Enter Fall → set `velocity.x = 0`.
> Khi land (`isGrounded`) → reset `JumpCount = 0` và `LastWallJumpDirection = 0`.

---

### WALL SLIDE
> `PlayerWallSlideState.cs`

| # | Condition | → State | Nguồn |
|---|---|---|---|
| 1 | `Jump pressed && isTouchingWall` | **WallJump** | `PlayerWallSlideState.Update()` |
| 2 | `!isTouchingWall` | **Fall** | `PlayerWallSlideState.Update()` |
| 3 | `isGrounded` | **Idle** *(+ gọi Flip(-direction))* | `PlayerWallSlideState.Update()` |
| 4 | *(external)* nhận đòn knockback | **KnockBack** | `PlayerHealth.cs` |

> **Ghi chú:** Khi `movementInput.y < 0` → gravity tự nhiên tăng tốc. Khi `movementInput.y >= 0` → `velocity.y *= slideDownSpeed` (trượt chậm).

---

### WALL JUMP
> `PlayerWallJumpState.cs` — kế thừa transitions từ `PlayerAirBoneState.cs`

| # | Condition | → State | Nguồn |
|---|---|---|---|
| 1 | `isGrounded && movementInput.x != 0` | **Move** | `PlayerWallJumpState.Update()` |
| 2 | `isGrounded && movementInput.x == 0` | **Idle** | `PlayerWallJumpState.Update()` |
| 3 | `rb.velocity.y < 0 && isTouchingWall && direction != LastWallJumpDirection` | **WallSlide** | `PlayerWallJumpState.Update()` |
| 4 | `rb.velocity.y < 0` *(không chạm wall)* | **Fall** | `PlayerWallJumpState.Update()` |
| 5 | `canAirAttack && BasicAttack pressed` | **JumpAttack** | `PlayerAirBoneState.Update()` |
| 6 | Skill slot Dash/Domain/Dismantle được kích hoạt | *(xem Jump #4–6)* | `PlayerAirBoneState.Update()` |
| 7 | *(external)* nhận đòn knockback | **KnockBack** | `PlayerHealth.cs` |

> **Ghi chú Enter:** Set `LastWallJumpDirection = direction` để tránh re-enter WallSlide cùng bức tường. Velocity = `WallJumpForce * -wallDirection`. Gọi `HandleFlip(-wallDirection)`.

---

### ATTACK
> `PlayerAttackState.cs`
> *Tất cả transitions chỉ được check **sau khi** animation trigger (`isTriggered == true`).*

| # | Condition | → State | Nguồn |
|---|---|---|---|
| 1 | `isTriggered && Dash pressed` | **Dash** | `PlayerAttackState.Update()` |
| 2 | `isTriggered && Jump pressed` | **Jump** | `PlayerAttackState.Update()` |
| 3 | `isTriggered && attackIndex >= ComboLimit` | **Idle** *(+ StartAttackCooldown)* | `PlayerAttackState.HandleStateExit()` |
| 4 | `isTriggered && BasicAttack pressed → _attackComboQueue = true` | **Attack** *(tiếp tục combo, EnterAttackComboCoroutine)* | `PlayerAttackState.HandleStateExit()` |
| 5 | `isTriggered && không input nào khác` | **Idle** | `PlayerAttackState.HandleStateExit()` |
| 6 | *(external)* nhận đòn knockback | **KnockBack** | `PlayerHealth.cs` |
| 7 | *(external)* hp ≤ 0 | **Dead** | `Player.cs` |

> **Ghi chú combo:** `_attackIndex` tăng dần theo mỗi lần Exit. Reset về 1 nếu `Time.time > lastAttackTime + timeResetCombo`. Attack velocity lấy từ `attackVelocity[attackIndex - 1]`.

---

### JUMP ATTACK
> `PlayerJumpAttackState.cs`

| # | Condition | → State | Nguồn |
|---|---|---|---|
| 1 | `isGrounded` | **Idle** | `PlayerJumpAttackState.Update()` |
| 2 | *(animation event)* `TriggerFallState()` được gọi | **Fall** | `PlayerJumpAttackState.TriggerFallState()` — gọi từ Animation Event |
| 3 | *(external)* nhận đòn knockback | **KnockBack** | `PlayerHealth.cs` |

> **Ghi chú 2 variant:**
> - `IsFromGround = true` → áp dụng `jumpForce` khi Enter (JumpAttack từ mặt đất).
> - `IsUpAttack = true` → play animation `JumpAttack_2` (nhấn attack + giữ up khi trên không).
> Khi Exit → gọi `StartAirAttackCooldown()`.

---

### DASH
> `PlayerDashState.cs`
> *Transitions chỉ được check sau khi `stateTimer <= 0`.*

| # | Condition | → State | Nguồn |
|---|---|---|---|
| 1 | `stateTimer ≤ 0 && isGrounded && movementInput.x != 0` | **Move** | `PlayerDashState.Update()` |
| 2 | `stateTimer ≤ 0 && isGrounded && movementInput.x == 0` | **Idle** | `PlayerDashState.Update()` |
| 3 | `stateTimer ≤ 0 && !isGrounded` | **Fall** | `PlayerDashState.Update()` |
| 4 | *(external)* nhận đòn knockback | **KnockBack** | `PlayerHealth.cs` |

> **Ghi chú Enter:** `gravityScale = 0`, velocity = `dashSpeed * dashDirection`. Duration lấy từ `SkillButtonHandler.GetSkill(Dash).Duration`.
> **Ghi chú Exit:** `gravityScale = 3.5f`, restore `canDash = true`.

---

### COUNTER
> `PlayerCounterState.cs`

| # | Condition | → State | Nguồn |
|---|---|---|---|
| 1 | `isTriggered` *(counter thành công, animation hoàn thành)* | **Idle** | `PlayerCounterState.Update()` |
| 2 | `stateTimer ≤ 0 && !isCounter` *(hết counter window, không parry được)* | **Idle** | `PlayerCounterState.Update()` |
| 3 | *(external)* nhận đòn knockback | **KnockBack** | `PlayerHealth.cs` |

> **Ghi chú:** `isCounter = PlayerCombat.IsPerformedCounter()` được check khi Enter. `animator.SetBool("CanCounter", isCounter)` để animation biết có parry không. Duration lấy từ `SkillButtonHandler.GetSkill(CounterSkill).Duration`.

---

### DISMANTLE
> `PlayerDismantleState.cs`
> *Input bị disable hoàn toàn trong suốt state này.*

| # | Condition | → State | Nguồn |
|---|---|---|---|
| 1 | `stateTimer ≤ 0` | **Idle** | `PlayerDismantleState.Update()` |

> **Ghi chú:** Duration lấy từ `PlayerSkillManager.skillDismantle.SkillBaseDefinition.Duration`. `input.Disable()` khi Enter, `input.Enable()` khi Exit.

---

### DOMAIN EXPANSION
> `PlayerDomainExpasionState.cs`
> *Input và physics (rb.simulated) bị tắt hoàn toàn.*

| # | Condition | → State | Nguồn |
|---|---|---|---|
| 1 | `stateTimer ≤ 0` | **Idle** | `PlayerDomainExpasionState.Update()` |

> **Ghi chú:** `rb.simulated = false` + `input.Disable()` khi Enter. Restore khi Exit. Duration từ `PlayerSkillManager.domainExpasionSkill.SkillBaseDefinition.Duration`.

---

### KNOCKBACK
> `PlayerKnockBackState.cs`
> *Input bị disable. Không thể thoát sớm, phải chờ cả `stateTimer` và `IsKnocked` đều clear.*

| # | Condition | → State | Nguồn |
|---|---|---|---|
| 1 | `stateTimer ≤ 0 && !IsKnocked && isGrounded` | **Idle** | `PlayerKnockBackState.Update()` |
| 2 | `stateTimer ≤ 0 && !IsKnocked && !isGrounded` | **Fall** | `PlayerKnockBackState.Update()` |
| 3 | *(external)* hp ≤ 0 | **Dead** | `Player.cs` |

> **Ghi chú:** `stateTimer = entityStat.StunDuration` khi Enter. `input.Disable()` khi Enter, `input.Enable()` khi Exit.

---

### DEAD
> `PlayerDeadState.cs`
> *State cuối, không có transition tự động.*

| # | Condition | → State | Nguồn |
|---|---|---|---|
| 1 | `R pressed` *(debug only)* | **Idle** | `PlayerDeadState.Update()` |

> **Ghi chú:** `rb.simulated = false` + `input.Disable()` khi Enter. Không có respawn logic thực tế hiện tại.

---

## External Triggers (gọi từ ngoài FSM)

Các transition này có thể xảy ra từ **bất kỳ state nào** và được gọi trực tiếp vào `stateMachine.ChangeState`:

| Event | → State | File gọi |
|---|---|---|
| Player nhận đòn có knockback | **KnockBack** | `PlayerHealth.cs:46` |
| Player hp ≤ 0 | **Dead** | `Player.cs:214` |
| `EnterAttackComboCoroutine` hoàn thành | **Attack** *(combo tiếp theo)* | `Player.cs:208` |

---

## Quick Reference — Điều kiện chung tái sử dụng

| Điều kiện | Ý nghĩa |
|---|---|
| `isGrounded` | Raycast chạm mặt đất |
| `isTouchingWall` | Raycast chạm tường |
| `direction` | Hướng mặt hiện tại của player (1 hoặc -1) |
| `LastWallJumpDirection` | Hướng tường vừa wall jump, tránh re-enter WallSlide cùng tường |
| `JumpCount < MaxJumpCount` | Còn lượt nhảy (double jump) |
| `canAttack` | Cooldown attack đã hết |
| `canAirAttack` | Cooldown air attack đã hết |
| `isTriggered` | Animation event đã fire (dùng trong Attack) |
| `stateTimer` | Đếm ngược thời gian của state, set trong `Enter()` |
| `SkillButtonHandler.TryConsumeSkill(i)` | Kiểm tra và consume skill ở slot `i`, trả về state tương ứng |
