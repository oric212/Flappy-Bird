using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class BirdController : MonoBehaviour
{
    [SerializeField] private float flapStrength = 5f;
    [SerializeField] private float forwardSpeed = 3f;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Camera gameplayCamera;
    [SerializeField] private float topBoundaryTolerance = 0.1f;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private float boostedForwardSpeed = 4.9f;
    [SerializeField] private float speedBoostDuration = 5f;

    private Rigidbody2D birdRigidbody;
    private CircleCollider2D birdCollider;
    private float normalGravityScale;
    private float speedBoostTimeRemaining;

    public bool IsSpeedBoosted => speedBoostTimeRemaining > 0f;
    public float SpeedBoostTimeRemaining => Mathf.Max(0f, speedBoostTimeRemaining);
    public float SpeedBoostProgress => speedBoostDuration > 0f
        ? Mathf.Clamp01(SpeedBoostTimeRemaining / speedBoostDuration)
        : 0f;

    private void Awake()
    {
        birdRigidbody = GetComponent<Rigidbody2D>();
        birdCollider = GetComponent<CircleCollider2D>();
        normalGravityScale = birdRigidbody.gravityScale;
        birdRigidbody.gravityScale = 0f;

        if (gameplayCamera == null)
        {
            gameplayCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (gameManager.IsGameOver)
        {
            return;
        }

        bool spacePressed = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
        bool mousePressed = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        bool flapPressed = spacePressed || mousePressed;

        if (gameManager.IsWaitingToStart)
        {
            return;
        }

        if (birdRigidbody.gravityScale == 0f)
        {
            birdRigidbody.gravityScale = normalGravityScale;
        }

        if (speedBoostTimeRemaining > 0f)
        {
            speedBoostTimeRemaining -= Time.deltaTime;
        }

        float cameraTop = gameplayCamera.transform.position.y + gameplayCamera.orthographicSize;
        float allowedColliderTop = cameraTop + topBoundaryTolerance;

        if (birdCollider.bounds.max.y > allowedColliderTop)
        {
            float excessHeight = birdCollider.bounds.max.y - allowedColliderTop;
            Vector2 clampedPosition = birdRigidbody.position;
            clampedPosition.y -= excessHeight;
            birdRigidbody.position = clampedPosition;

            if (birdRigidbody.linearVelocity.y > 0f)
            {
                birdRigidbody.linearVelocity = new Vector2(
                    birdRigidbody.linearVelocity.x,
                    0f);
            }

            StopRun();
            return;
        }

        if (flapPressed && gameManager.CanAcceptGameplayInput)
        {
            birdRigidbody.linearVelocity = new Vector2(birdRigidbody.linearVelocity.x, flapStrength);
            audioManager?.PlayFlap();
        }
    }

    private void FixedUpdate()
    {
        if (!gameManager.IsPlaying)
        {
            birdRigidbody.linearVelocity = Vector2.zero;
            return;
        }

        float currentSpeed = IsSpeedBoosted ? boostedForwardSpeed : forwardSpeed;
        birdRigidbody.linearVelocity = new Vector2(currentSpeed, birdRigidbody.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        bool hitPipe = collision.collider.GetComponentInParent<PipeObstacle>() != null;
        bool hitGround = collision.collider.GetComponentInParent<GroundLooper>() != null;

        if (hitPipe || hitGround)
        {
            StopRun();
        }
    }

    private void StopRun()
    {
        speedBoostTimeRemaining = 0f;
        birdRigidbody.linearVelocity = Vector2.zero;
        birdRigidbody.angularVelocity = 0f;
        birdRigidbody.simulated = false;
        gameManager.TriggerGameOver();
    }

    public bool ApplySpeedBoost()
    {
        if (!gameManager.IsPlaying)
        {
            return false;
        }

        speedBoostTimeRemaining = speedBoostDuration;
        return true;
    }
}
