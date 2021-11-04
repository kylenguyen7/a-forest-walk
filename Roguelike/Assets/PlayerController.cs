using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Effectable {
    #region singleton
    public static PlayerController instance;

    private new void Awake() {
        base.Awake();
        if (instance != null) {
            Debug.LogError("Found more than one PlayerController in the scene!");
            return;
        }
        instance = this;
    }
    #endregion

    public Transform origin;

    public bool doUpdate = true;
    public bool canAttack = true;
    public Vector2Int currentRoomIndex;

    public Animator animator;
    public ParticleSystem dashParticles;
    public Transform abilitiesPivot;

    public bool debugInvincible = false;
    bool invincible = false;
    public float invincibilityTime = 1f;

    enum states {
        walking,
        dashing,
        // damaged,
        frozen,
        // attacking,
        dead
    }
    private states state = states.walking;

    // Character stats
    public float hp;
    public static int gold = 0;

    public float HPPercentage { get { return hp / GameManager.MAX_HP; } }

    public override bool IsEnemy => false;

    public bool isAlive = true;

    // Physics variables
    public Vector2 lookingStart = new Vector2(0, -1);
    private Vector2 looking;
    private Rigidbody2D rb;

    public GameObject colliderProjection;

    // Walking variables
    private Vector2 movement;
    public float moveSpeed = 4f;

    // Dash variables
    public float dashSpeed = 5f;
    public float dashLength = 5f;
    // public float dashForce = 100f;
    private Vector2 dashStartingPosition = Vector2.zero;
    private bool dashQueued = false;

    // Attacking variables
    private float attackTimer;

    // Polish
    private Material matWhite;

    // Speaking
    public bool speaking = false;

    private const float RESET_TIMER = 3f;
    private float resetTime;

    // Start is called before the first frame update
    void Start() {
        // animator = gameObject.GetComponent<Animator>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        looking = lookingStart;
        hp = GameManager.MAX_HP;

        currentRoomIndex = Vector2Int.zero;

        matWhite = Resources.Load<Material>("WhiteFlash");

        Textbox.instance.OnDialogueFinishedCallback += FinishSpeaking;

        resetTime = RESET_TIMER;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            dashQueued = true;
        }

        if(Input.GetKey(KeyCode.K) && GameManager.instance != null) {
            resetTime -= Time.unscaledDeltaTime;

            if(resetTime <= 0) {
                Vector2 randomVector = UnityEngine.Random.insideUnitCircle;
                randomVector.Normalize();

                Die(randomVector);
            }
        } else {
            resetTime = RESET_TIMER;
        }
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (!doUpdate || speaking) {
            return;
        }

        switch (state) {
            case states.walking: {
                    StateWalking();
                    CheckInteraction();
                }
                break;
            case states.dashing: StateDashing(); break;
            // case states.damaged: StateDamaged(); break;
            // case states.attacking: StateAttacking(); break;
            case states.frozen: StateFrozen(); break;
            case states.dead: StateDead(); break;
        }

        rb.velocity += knockback * (1 - slowModifier);

        UpdateFlip();
        UpdateColliderProjection();
        DevTools();
        DampenKnockback();
    }

    private void UpdateColliderProjection() {
        if (colliderProjection == null) {
            Debug.LogError("PlayerController is missing a collider projection!");
            return;
        }

        var adjustedLooking = Mathf.Abs(looking.x) == Mathf.Abs(looking.y) ? new Vector2(looking.x, 0) : looking;

        var pos = (Vector2)transform.position + new Vector2(0, 0.3f) + adjustedLooking * 0.5f;
        colliderProjection.transform.position = pos;
    }

    // Stops any current movement and freezes input until resume is called
    public void Stop() {
        animator.SetBool("isRunning", false);
        rb.velocity = Vector2.zero;
        doUpdate = false;
    }

    public void Resume() {
        doUpdate = true;
    }

    private void DevTools() {

        // Testing freeze/unfreeze
        /*if(Input.GetKeyDown(KeyCode.E)) {
            if(state == states.frozen) {
                Unfreeze();
            } else {
                Freeze();
            }
        }*/
    }

    /* Handles only looking in new direction when input is given */
    private void updateLooking() {
        Vector2 test = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (test.magnitude == 0) return;

        looking = test;
        animator.SetFloat("lookingHorizontal", looking.x);
        animator.SetFloat("lookingVertical", looking.y);
    }

    private float stepSoundTime = 0.2f;
    private float stepSoundTimer;
    public AudioSource stepSfxSource;
    public AudioClip stepSfx;
    public AudioClip rollSfx;
    private void walkSfxLoop(bool isRunning) {
        if (!isRunning) {
            stepSoundTimer = stepSoundTime;
            return;
        }

        if (stepSoundTimer <= 0) {
            if (stepSfx != null) stepSfxSource.PlayOneShot(stepSfx);
            stepSoundTimer = stepSoundTime;
        }
        stepSoundTimer -= Time.deltaTime;
    }

    private void StateWalking() {
        // Get movement, smoothing for diagonal movement
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float horizontalRaw = Input.GetAxisRaw("Horizontal");
        movement = new Vector2(horizontal, vertical);

        if (horizontalRaw == 0) movement.x = 0;
        if (Input.GetAxisRaw("Vertical") == 0) movement.y = 0;

        if (movement.magnitude > 1) {
            movement = movement.normalized * 1.1f;
        }

        rb.velocity = movement * moveSpeed * (1 - slowModifier);

        // Set animations
        bool isRunning = rb.velocity.magnitude > 0.01;
        animator.SetBool("isRunning", isRunning);
        updateLooking();

        if ((horizontal > 0 && !lookingRight) || (horizontal < 0 && lookingRight)) Flip();

        if (isRunning && horizontalRaw != 0) {
            tilt(-horizontalRaw * 3f, 0.1f);
        } else {
            tilt(0, 0.3f);
        }

        // Play running sound
        walkSfxLoop(isRunning);

        // Start dash
        // for mouse click: if(Input.GetMouseButtonDown(0)) {
        if (dashQueued) {
            InitiateDash();
            dashQueued = false;
        }
    }

    /* Responsible for adding dashForce in correct direction
     * and setting state */
    private void InitiateDash() {
        state = states.dashing;
        Vector2 direction;

        // get direction of dash w/ mouse click
        // Vector2 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;

        // get direction of dash
        if (Mathf.Approximately(rb.velocity.magnitude, 0)) {
            direction = looking.normalized;
        } else {
            direction = rb.velocity.normalized;
        }

        // set velocity of dash
        // 1.5f adds extra oomph, to decay quickly during StateDashing()
        rb.velocity = direction * 1.5f * dashSpeed * (1 - slowModifier);
        dashStartingPosition = rb.transform.position;
        dashDir = rb.velocity.normalized;

        // initiate particles (stopped during StateDashing())
        dashParticles.Play();
        dashParticles.transform.forward = -direction;

        // squash n stretch
        // GetComponentInChildren<Animator>().SetTrigger("big squash");
        // LeanTween.scale(squashAndStretch, )

        // sound effect
        stepSfxSource.PlayOneShot(rollSfx);

        // screen shake
        SimpleCameraShakeInCinemachine.Shake(0.05f, 0.2f, 0.5f);
    }

    private Vector2 dashDir;
    private void EndDash() {
        Vector2 overhangVelocity = dashDir * moveSpeed * (1 - slowModifier);

        // overhang from dash, as if stopping from normal movement in that direction
        if (rb.velocity.magnitude >= overhangVelocity.magnitude)
            rb.velocity = overhangVelocity;

        dashParticles.Stop();
        state = states.walking;
    }

    private void StateDashing() {
        animator.SetBool("isRunning", true);
        animator.SetFloat("lookingHorizontal", dashDir.x);
        animator.SetFloat("lookingVertical", dashDir.y);

        // decay dash speed
        rb.velocity = Vector2.Lerp(rb.velocity, dashDir * dashSpeed * (1 - slowModifier), 1 / 3);

        // end dash, stop particle system
        if (Vector2.Distance(dashStartingPosition, rb.transform.position) >= dashLength * (1 - slowModifier)) {
            EndDash();
        }

        // set animations
        if (dashDir.x != 0) {
            tilt(-Mathf.Sign(dashDir.x) * 10f, 0.2f);
        }
    }

    /* Responsible for decrementing hp, setting knockback,
     * and either initiating StateDamaged() which times input freeze,
     * or setting player to dead. */
    public override void TakeDamage(float damage, Vector2 forceDirection, float kb) {
        if (state == states.dashing || invincible || debugInvincible) {
            return;
        }

        mySprite.material = matWhite;
        Invoke("ResetMaterial", 0.2f);
        GetComponentInChildren<Animator>().SetTrigger("big squash");

        Debug.Log($"Damage: {damage}");

        hp -= damage;
        
        // special exit case: player is dead
        if(hp <= 0) {
            Die(forceDirection);
            return;
        }

        //damageTimer = time;

        // for no knockback, pass (0, 0)
        if (forceDirection != Vector2.zero) {
            knockback += forceDirection.normalized * kb;
        }

        SetPlayerInvincible();
        StartCoroutine(StunnedSequence());
        state = states.walking;
    }

    public void SetPlayerInvincible() {
        invincible = true;
        StartCoroutine(ResetInvincibility());
        Debug.Log($"Setting player to invincible for {invincibilityTime} seconds.");
    }

    IEnumerator StunnedSequence() {
        float spd = moveSpeed;
        moveSpeed = 0f;
        yield return new WaitForSeconds(0.1f);

        moveSpeed = spd;
    }

    private IEnumerator ResetInvincibility() {
        yield return new WaitForSeconds(invincibilityTime);
        invincible = false;
    }

    #region consumable functions
    public void Heal(float amount) {

        var hearts = Resources.Load("Prefabs/Hearts", typeof(GameObject)) as GameObject;
        Instantiate(hearts, abilitiesPivot.position, Quaternion.Euler(-90, 0, 0));

        mySprite.material = matWhite;
        Invoke("ResetMaterial", 0.2f);

        hp = Mathf.Min(hp + amount, GameManager.MAX_HP);
        GetComponentInChildren<Animator>().SetTrigger("big squash");
    }

    public void Deploy(GameObject objectToSpawn) {
        Instantiate(objectToSpawn, transform.position, Quaternion.identity);
        GetComponentInChildren<Animator>().SetTrigger("squash");
    }
    #endregion

    public void AttackKnockback(Vector2 kb) {
        // Debug.Log($"Adding {kb.x},{kb.y} to knockback");
        knockback += kb;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(state == states.dashing) {
            EndDash();
        }

        if(collision.gameObject.CompareTag("Enemy")) {
            TakeDamage(10f, transform.position - collision.transform.position, 3f);
        }

        if (collision.gameObject.CompareTag("Loot")) {
            GameObject sparkle = Resources.Load("Prefabs/Sparkle") as GameObject;
            Instantiate(sparkle, collision.transform.position, Quaternion.identity);
            Destroy(collision.gameObject);
        }
    }

    private void OnCollisionStay2D(Collision2D collision) {
        if (state == states.dashing) {
            EndDash();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Loot")) {
            LootController loot = collision.gameObject.GetComponent<LootController>();
            if(loot == null) {
                Debug.LogError("An object with the 'loot' tag doesn't have a LootController (no defined beahvior for pickup).");
                return;
            }
            loot.Pickup();
        }
    }

    /* Responsible for setting dead state and animations and setting
     * rb variables to drift away dead character */
    private void Die(Vector2 forceDirection) {
        if(state == states.dead) {
            Debug.LogWarning("Already dead!");
            return;
        }

        state = states.dead;
        SimpleCameraShakeInCinemachine.Freeze(0.4f);
        SimpleCameraShakeInCinemachine.Shake(0.5f, 2f, 3f);

        rb.drag = 3f;
        rb.velocity = forceDirection.normalized * 10f;

        GameManager.instance.StatTotalDeaths += 1;
        GameManager.instance.RestartSequence();
    }

    private void StateDead()
    {
        animator.SetTrigger("die");
        isAlive = false;
        invincible = true;
    }

    private void Freeze() {
        animator.SetBool("gotItem", true);
        state = states.frozen;
    }

    private void Unfreeze() {
        animator.SetBool("gotItem", false);
        state = states.walking;
    }

    private void StateFrozen()
    {
        tilt(0, 0.2f);
        rb.velocity = Vector2.zero;
    }

    private float targetXScale = 1f;
    // private float targetYRot = 0f;
    public bool lookingRight = true;
    private void Flip() {
        targetXScale = -targetXScale;
        // targetYRot = (targetYRot + 180) % 360;
        lookingRight = !lookingRight;
    }

    private void UpdateFlip() {
        var scale = transform.localScale;
        scale.x += (targetXScale - scale.x) / 3f;
        transform.localScale = scale;

        // Debug.Log(targetYRot);
        // float currentYRot = transform.eulerAngles.y;
        // float rotateAmountY = (targetYRot - currentYRot) * 0.2f;
        // transform.Rotate(0, rotateAmountY, 0, Space.World);
    }

    private void tilt(float targetZRot, float interpolationFactor) {
        float currentZRot = transform.eulerAngles.z < 180 ? transform.eulerAngles.z :
                                                            transform.eulerAngles.z - 360;

        // 0 is no movement, 1 is one frame to complete
        interpolationFactor = Mathf.Clamp(interpolationFactor, 0, 1);
        float rotateAmountZ = (targetZRot - currentZRot) * interpolationFactor;
        transform.Rotate(0, 0, rotateAmountZ, Space.World);
    }

    private void experimentalTiltIncludeX() {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        float targetZRot = 0f;
        float targetXRot = 0f;
        if (vertical < 0) {
            targetXRot = vertical * 15f;
        } else if (horizontal != 0) {
            targetZRot = -horizontal * 5f;
        } else if (vertical > 0) {
            targetXRot = vertical * 15f;
        }

        float currentZRot = transform.eulerAngles.z < 180 ? transform.eulerAngles.z :
                                                            transform.eulerAngles.z - 360;

        float currentXRot = transform.eulerAngles.x < 180 ? transform.eulerAngles.x :
                                                            transform.eulerAngles.x - 360;

        float rotateAmountZ = (targetZRot - currentZRot) / 10;
        float rotateAmountX = (targetXRot - currentXRot) / 10;
        transform.Rotate(rotateAmountX, -transform.eulerAngles.y, rotateAmountZ, Space.World);
    }

    #region interaction
    private void CheckInteraction() {
        if(Input.GetKeyDown(KeyCode.E)) {
            var projection = colliderProjection.GetComponent<ColliderProjection>();
            if (projection.IsColliding()) {
                
                if(projection.Object.GetComponent<NPC>() != null) {
                    projection.Object.GetComponent<NPC>().Speak();
                    speaking = true;
                }
            }
        }
    }
    #endregion

    public void FinishSpeaking() {
        speaking = false;
    }
}
