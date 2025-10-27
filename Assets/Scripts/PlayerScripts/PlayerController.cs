using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    //header just makes it so i can change all these variables in the inspector.

    [Header("Prefabs")]
    public GameObject GreenShooterprefab;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 8f;

    [Header("Shooting (base)")]
    public float shootCooldown = 0.5f;   // base cooldown
    public float bulletSpeed = 12f;      // base bullet speed

    [Header("Big Shot Power-up")]
    public bool hasBigShot = false;
    public float bigShotDuration = 6f;
    public float bigBulletSpeed = 20f;   // faster bullet
    public float bigBulletScale = 1.8f;  // visually larger
    public float cooldownMultiplier = 0.6f; // 40% faster fire rate while active

    [Header("Speed Increase")]
    public float speedIncreaseAmount = 1f; // how much to increase speed
    public float speedIncreaseDuration = 6f; //how long the speed increase lasts
    private bool hasSpeedIncrease = false;

    [Header("2 Frame Animation")]
    public SpriteRenderer sr;            // drag your SpriteRenderer here
    public Sprite[] walkFrames = new Sprite[2]; // put your 2 walking frames here
    public float animSpeed = 0.15f;      // time between flickers

    private float animTimer;
    private int animIndex;

    private Rigidbody2D rb;
    private bool grounded;
    private float lastShootTime = 0f;
    private float lastXInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (sr == null) sr = GetComponent<SpriteRenderer>();
    }

    //just checking to see if script is working within the console.
    void Start()
    {
        Debug.Log("PlayerController is working, player spawned at " + transform.position);
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        AnimateAndFlip();
        Bounds();
        HandleShooting();
    }

    void HandleMovement()
    {
        //regular movement
        float x = Input.GetAxisRaw("Horizontal");
        lastXInput = x;

        // this is for the speed power-up
        float currentSpeed = hasSpeedIncrease ? moveSpeed * 2f : moveSpeed; // double speed if active
        rb.velocity = new Vector2(x * currentSpeed, rb.velocity.y);
    }

    void HandleJump()
    {
        //If the player is grounded and presses Jump, set upward velocity to jumpForce.
        if (grounded && Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    //When colliding with anything, mark the player as grounded. When leaving a collision, mark as not grounded.
    void OnCollisionEnter2D(Collision2D col) { grounded = true; }
    void OnCollisionExit2D(Collision2D col) { grounded = false; }

    void HandleShooting()
    {
        float effectiveCooldown = hasBigShot ? shootCooldown * cooldownMultiplier : shootCooldown;

        if (Input.GetMouseButtonDown(0) && Time.time > lastShootTime + effectiveCooldown)
        {
            Vector3 spawnPos = transform.position + new Vector3(0, 0.6f, 0);
            GameObject proj = Instantiate(GreenShooterprefab, spawnPos, Quaternion.identity);

            // aim at mouse and feed speed/scale depending on power-up
            Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouse.z = 0f;

            float speedToUse = hasBigShot ? bigBulletSpeed : bulletSpeed;
            float scaleToUse = hasBigShot ? bigBulletScale : 1f;

            proj.GetComponent<GreenShooter>().Initialize(mouse, speedToUse, scaleToUse);

            lastShootTime = Time.time;
        }
    }

    // super simple idle/walk + flip
    void AnimateAndFlip()
    {
        // idle sprite if no movement
        if (Mathf.Abs(lastXInput) < 0.01f)
        {
            animIndex = 0;
            sr.sprite = walkFrames[0];
            return;
        }

        // flip left/right
        if (lastXInput > 0.01f) sr.flipX = true;
        if (lastXInput < -0.01f) sr.flipX = false;

        // walking animation
        animTimer += Time.deltaTime;
        if (animTimer >= animSpeed)
        {
            animTimer = 0f;
            animIndex = 1 - animIndex; // toggles 0 ↔ 1
            sr.sprite = walkFrames[animIndex];
        }
    }

    void Bounds()
    {
        //If the player falls below y = -7, teleport them back to(-6, -1)(a respawn point). VERY simple respawn system. Can be tweaked later to be better, fine for now.
        if (transform.position.y < -7f)
        {
            transform.position = new Vector3(-6f, -1f, transform.position.z);
        }
    }

    // Call this function to activate the big shot power-up... needs to be public or else it cannot be accessed by other scripts.
    public void BigShotPowerUp()
    {
        if (gameObject.activeInHierarchy)
            StartCoroutine(BigShotRoutine());
    }

    //IEnumerator is the return type Unity uses for Coroutines... not as fancy as it seems.
    private IEnumerator BigShotRoutine()
    {
        hasBigShot = true;
        yield return new WaitForSeconds(bigShotDuration);
        hasBigShot = false;
        //makes it so powerup does not last for ever!
    }

    //mushroom = speed :D
    public void SpeedIncrease()
    {
        if (gameObject.activeInHierarchy)
            StartCoroutine(SpeedIncreaseRoutine());
    }

    //IEnumerator is the return type Unity uses for Coroutines... not as fancy as it seems.
    private IEnumerator SpeedIncreaseRoutine()
    {
        hasSpeedIncrease = true;
        yield return new WaitForSeconds(speedIncreaseDuration);
        hasSpeedIncrease = false;
        //makes it so speedboost does not last for ever!
    }
}
