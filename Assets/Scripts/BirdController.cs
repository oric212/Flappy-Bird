using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class BirdController : MonoBehaviour
{
    [SerializeField] private float flapStrength = 5f;
    [SerializeField] private float forwardSpeed = 2.5f;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Camera gameplayCamera;
    [SerializeField] private float topBoundaryTolerance = 0.1f;

    private Rigidbody2D birdRigidbody;
    private CircleCollider2D birdCollider;

    private void Awake()
    {
        birdRigidbody = GetComponent<Rigidbody2D>();
        birdCollider = GetComponent<CircleCollider2D>();

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

        bool spacePressed = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
        bool mousePressed = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;

        if (spacePressed || mousePressed)
        {
            birdRigidbody.linearVelocity = new Vector2(birdRigidbody.linearVelocity.x, flapStrength);
        }
    }

    private void FixedUpdate()
    {
        if (gameManager.IsGameOver)
        {
            birdRigidbody.linearVelocity = new Vector2(0f, birdRigidbody.linearVelocity.y);
            return;
        }

        birdRigidbody.linearVelocity = new Vector2(forwardSpeed, birdRigidbody.linearVelocity.y);
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
        gameManager.TriggerGameOver();
        birdRigidbody.linearVelocity = new Vector2(0f, birdRigidbody.linearVelocity.y);
    }
}
