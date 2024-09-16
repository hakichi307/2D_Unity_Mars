using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public float pushSpeed = 2.5f;
    public float liftForce = 2.5f;
    public float descentSpeed = 2.0f;
    public float timeBeforeFlying = 1.0f;

    private bool isGrounded;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

    private bool isJumping = false;
    private float jumpTimer = 0.0f;

    public Image _Energy;
    public float maxEnergy = 100f;
    private float currentEnergy;
    public float energyDrainRate = 20f;
    public float energyRecoveryRate = 5f;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private float normalCameraSize = 10f;
    [SerializeField] private float flyingCameraSize = 13f;
    [SerializeField] private float zoomSpeed = 1.0f;

    public ParticleSystem leftJetpack;
    public ParticleSystem rightJetpack;
    public ParticleSystem jetpack;
    public ParticleSystem smokeJetpack;

    private float jetpackDuration = 0.2f;
    private float jetpackTimer = 0.0f;

    private bool isJetpackActive = false;
    private uint coins = 0;
    public TextMeshProUGUI coinsCollectedLabel;
    private Animator playerAnimator;
    private Vector3 spawnPoint;
    private bool isDead = false;
    public AudioClip coinCollectSound;
    public AudioSource fallingHitAudio;
    public AudioSource rocketFlamesAudio;
    public AudioSource rocketSpaceAudio;
    public AudioSource deathAudio;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentEnergy = maxEnergy;
        UpdateEnergyBar();
        playerAnimator = GetComponent<Animator>();
        spawnPoint = transform.position;
    }
    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        bool pushRight = Input.GetKey(KeyCode.A);
        bool pushLeft = Input.GetKey(KeyCode.D);
        if (isGrounded && !isDead)
        {
            playerAnimator.SetBool("isGrounded", isGrounded);
            spawnPoint = transform.position;
        }
        else if (!isGrounded && !isDead)
        {
            playerAnimator.SetBool("isGrounded", isGrounded);
        }
        if (transform.position.y < -10 && !isDead)
        {
            HandlePlayerDeath();
        }
        if (isGrounded && !isJumping)
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
            {
                Jump();
                isJetpackActive = true;
                jetpackTimer = 0.0f;
            }
        }
        else if (isJumping)
        {
            jumpTimer += Time.deltaTime;
            if (jumpTimer >= timeBeforeFlying)
            {
                isJumping = false;
            }
        }
        if (!isGrounded && !isJumping)
        {
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, flyingCameraSize, zoomSpeed * Time.deltaTime);

            if (isJetpackActive)
            {
                jetpackTimer += Time.deltaTime;
                if (jetpackTimer >= jetpackDuration)
                {
                    isJetpackActive = false;
                }
            }
            if (pushRight || pushLeft)
            {
                if (currentEnergy > 0)
                {
                    if (pushRight)
                    {
                        rb.AddForce(new Vector2(pushSpeed * 0.2f, liftForce * 0.2f), ForceMode2D.Force);
                    }
                    else if (pushLeft)
                    {
                        rb.AddForce(new Vector2(-pushSpeed * 0.2f, liftForce * 0.2f), ForceMode2D.Force);
                    }
                    currentEnergy -= energyDrainRate * Time.deltaTime;
                    UpdateEnergyBar();
                }
            }
            if (pushRight && pushLeft && currentEnergy > 0)
            {
                rb.AddForce(new Vector2(0, (AdjustDescentSpeed(descentSpeed) - rb.velocity.y) * rb.mass), ForceMode2D.Force);
                rb.AddForce(new Vector2(rb.mass * (0.2f - rb.velocity.x * 2f), 0), ForceMode2D.Force);
            }
        }
        else
        {
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, normalCameraSize, zoomSpeed * Time.deltaTime);
        }

        if (!pushRight && !pushLeft && currentEnergy < maxEnergy)
        {
            currentEnergy += energyRecoveryRate * Time.deltaTime;
            UpdateEnergyBar();
        }
        AdjustJetpack(isJetpackActive);
        AdjustSideJetpacks();
    }
    void HandlePlayerDeath()
    {
        if (!isDead)  // Chỉ xử lý chết một lần
        {
            playerAnimator.SetTrigger("die");
            isDead = true;
            rb.velocity = Vector2.zero;

            StartCoroutine(RespawnPlayer());
        }
    }
    private IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(1.0f);
        playerAnimator.ResetTrigger("die");
        playerAnimator.Play("idle");
        transform.position = spawnPoint;
        rb.velocity = Vector2.zero;
        isDead = false;

    }
    void CollectCoin(Collider2D coinCollider)
    {
        coins++;
        coinsCollectedLabel.text = coins.ToString();
        if (coinCollider != null && coinCollider.gameObject != null)
        {
            coinCollider.gameObject.SetActive(false);
            AudioSource.PlayClipAtPoint(coinCollectSound, transform.position);
        }
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider != null && collider.gameObject.CompareTag("Coin"))
        {
            CollectCoin(collider);
        }
    }
    float AdjustDescentSpeed(float currentDescentSpeed)
    {
        float minDescentSpeed = 5.0f;
        currentDescentSpeed = Mathf.Lerp(currentDescentSpeed, minDescentSpeed, Time.deltaTime);
        return currentDescentSpeed;
    }
    void Jump()
    {
        rb.velocity = new Vector2(pushSpeed * 2f, liftForce * 3.5f);
        isJumping = true;
        jumpTimer = 0.0f;
    }
    private void AdjustJetpack(bool jetpackActive)
    {
        var jetpackEmission = jetpack.emission;
        jetpackEmission.enabled = jetpackActive;
    }
    private void AdjustSideJetpacks()
    {
        if (!isGrounded)
        {
            var leftJetpackEmission = leftJetpack.emission;
            leftJetpackEmission.enabled = Input.GetKey(KeyCode.A);
            var rightJetpackEmission = rightJetpack.emission;
            rightJetpackEmission.enabled = Input.GetKey(KeyCode.D);
        }
        var smokeEmission = smokeJetpack.emission;
        smokeEmission.enabled = !isGrounded;
    }
    void AdjustFootstepsAndJetpackSound(bool jetpackActive)
    {
        fallingHitAudio.enabled = isGrounded;
        if (!isGrounded && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
        {
            if (!rocketSpaceAudio.isPlaying)
            {
                rocketSpaceAudio.Play();
            }
            rocketSpaceAudio.volume = Mathf.Lerp(rocketSpaceAudio.volume, 1.0f, Time.deltaTime * 2.0f);  // Tăng volume dần lên
        }
        else
        {
            // Fade out âm thanh không gian khi không bay nữa
            rocketSpaceAudio.volume = Mathf.Lerp(rocketSpaceAudio.volume, 0.0f, Time.deltaTime * 2.0f);
            if (rocketSpaceAudio.volume <= 0.01f)
            {
                rocketSpaceAudio.Stop();  // Ngừng khi âm lượng gần bằng 0
            }
        }
        if (isJumping)
        {
            if (!rocketFlamesAudio.isPlaying)
            {
                rocketFlamesAudio.Play();
            }
            rocketFlamesAudio.volume = Mathf.Lerp(rocketFlamesAudio.volume, jetpackActive ? 1.0f : 0.5f, Time.deltaTime * 2.0f);  // Tăng volume dần lên
        }
        else
        {
            // Fade out âm thanh lửa khi không nhảy nữa
            rocketFlamesAudio.volume = Mathf.Lerp(rocketFlamesAudio.volume, 0.0f, Time.deltaTime * 2.0f);
            if (rocketFlamesAudio.volume <= 0.01f)
            {
                rocketFlamesAudio.Stop();  // Ngừng khi âm lượng gần bằng 0
            }
        }
        deathAudio.enabled = isDead;
    }
    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && !isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x * 0.98f, rb.velocity.y);
        }
        AdjustFootstepsAndJetpackSound(isJetpackActive);
    }
    void UpdateEnergyBar()
    {
        _Energy.fillAmount = currentEnergy / maxEnergy;
    }
}
