using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PipeSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform bird;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private GameObject pipeObstaclePrefab;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private GameObject sonicPowerUpPrefab;
    [SerializeField, Range(0f, 1f)] private float sonicSpawnProbability = 0.12f;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField, Range(0f, 1f)] private float coinSpawnProbability = 0.28f;
    [SerializeField] private GroundLooper groundLooper;

    [Header("Obstacle Type Weights")]
    [SerializeField] private float standardPairWeight = 35f;
    [SerializeField] private float bottomOnlyWeight = 25f;
    [SerializeField] private float topOnlyWeight = 25f;
    [SerializeField] private float asymmetricPairWeight = 15f;

    [Header("Horizontal Placement")]
    [SerializeField] private float minimumSpacing = 5.5f;
    [SerializeField] private float maximumSpacing = 9.5f;
    [SerializeField] private float spawnAheadDistance = 34f;
    [SerializeField] private float cleanupBehindCameraMargin = 3f;

    [Header("Obstacle Density")]
    [SerializeField] private int minimumNearbyObstacles = 4;
    [SerializeField] private int maximumNearbyObstacles = 6;
    [SerializeField] private float nearbyBehindCameraMargin = 5f;

    [Header("Opening And Length Ranges")]
    [SerializeField] private float minimumGapCenter = -1.5f;
    [SerializeField] private float maximumGapCenter = 1.5f;
    [SerializeField] private float minimumGapSize = 2.6f;
    [SerializeField] private float maximumGapSize = 3.6f;
    [FormerlySerializedAs("minimumPipeLength")]
    [SerializeField] private float minimumVisiblePipeHeight = 2.2f;
    [SerializeField] private float maximumPipeLength = 5f;

    [Header("Validation")]
    [SerializeField] private float closeSpacingThreshold = 9.5f;
    [SerializeField] private float minimumCloseRouteOverlap = 1.1f;
    [SerializeField] private int maximumRerollAttempts = 8;

    [Header("World Bounds")]
    [SerializeField] private float groundOverlap = 0.05f;
    [SerializeField] private float upperScreenPadding = 0.5f;

    private readonly List<GameObject> activeObstacles = new List<GameObject>();
    private float nextSpawnX;
    private float previousObstacleX;
    private float previousRouteBottom;
    private float previousRouteTop;
    private bool hasPreviousObstacle;
    private int nearbyTarget;
    private float groundTopY;
    private float lowerPipeExtent;
    private float upperPipeExtent;
    private readonly List<float> recentObstacleXPositions = new List<float>();
    private PipeLayoutGenerator layoutGenerator;
    private PipeLayoutValidator layoutValidator;

    public int ActiveObstacleCount => activeObstacles.Count;
    public int MaximumActiveObstacleCount { get; private set; }
    public int TotalGeneratedObstacleCount { get; private set; }
    public int StandardPairCount { get; private set; }
    public int AsymmetricPairCount { get; private set; }
    public int BottomOnlyCount { get; private set; }
    public int TopOnlyCount { get; private set; }
    public int BottomPipesValidated { get; private set; }
    public int InvalidBottomPipeCount { get; private set; }
    public int TotalCoinsSpawned { get; private set; }
    public float GroundTopY => groundTopY;
    public float LowerPipeExtentY => lowerPipeExtent;
    public float UpperPipeExtentY => upperPipeExtent;
    public IReadOnlyList<float> RecentObstacleXPositions => recentObstacleXPositions;

    private void Start()
    {
        if (bird == null || pipeObstaclePrefab == null
            || scoreManager == null || gameManager == null
            || groundLooper == null)
        {
            enabled = false;
            return;
        }

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        BoxCollider2D[] groundColliders = groundLooper.GetComponentsInChildren<BoxCollider2D>();
        if (groundColliders.Length == 0)
        {
            enabled = false;
            return;
        }

        groundTopY = groundColliders[0].bounds.max.y;
        foreach (BoxCollider2D groundCollider in groundColliders)
        {
            groundTopY = Mathf.Max(groundTopY, groundCollider.bounds.max.y);
        }

        lowerPipeExtent = groundTopY - groundOverlap;
        upperPipeExtent = targetCamera.transform.position.y
            + targetCamera.orthographicSize
            + upperScreenPadding;

        layoutGenerator = new PipeLayoutGenerator(
            standardPairWeight,
            bottomOnlyWeight,
            topOnlyWeight,
            asymmetricPairWeight,
            minimumGapCenter,
            maximumGapCenter,
            minimumGapSize,
            maximumGapSize,
            minimumVisiblePipeHeight,
            maximumPipeLength,
            groundTopY,
            lowerPipeExtent,
            upperPipeExtent);
        layoutValidator = new PipeLayoutValidator(
            minimumVisiblePipeHeight,
            minimumGapSize,
            closeSpacingThreshold,
            minimumCloseRouteOverlap);

        nearbyTarget = Random.Range(minimumNearbyObstacles, maximumNearbyObstacles + 1);
        nextSpawnX = bird.position.x + Random.Range(minimumSpacing, maximumSpacing);
    }

    private void Update()
    {
        if (!gameManager.IsPlaying)
        {
            return;
        }

        RemoveObstaclesBehindCamera();

        if (CountNearbyObstacles() < minimumNearbyObstacles)
        {
            nearbyTarget = Random.Range(minimumNearbyObstacles, maximumNearbyObstacles + 1);
        }

        MaintainObstacleDensity();
    }

    private void MaintainObstacleDensity()
    {
        int nearbyCount = CountNearbyObstacles();
        float spawnLimitX = bird.position.x + spawnAheadDistance;

        for (int i = 0; i < maximumNearbyObstacles && nearbyCount < nearbyTarget; i++)
        {
            if (nextSpawnX > spawnLimitX)
            {
                break;
            }

            float spacing = hasPreviousObstacle ? nextSpawnX - previousObstacleX : maximumSpacing;
            SpawnObstacle(nextSpawnX, spacing);
            nextSpawnX += Random.Range(minimumSpacing, maximumSpacing);
            nearbyCount = CountNearbyObstacles();
        }
    }

    private void SpawnObstacle(float worldX, float spacingFromPrevious)
    {
        PipeLayout layout = GetValidatedLayout(spacingFromPrevious);
        GameObject obstacle = Instantiate(
            pipeObstaclePrefab,
            new Vector3(worldX, 0f, 0f),
            Quaternion.identity);

        obstacle.name = "PipeObstacle_" + layout.Type;
        ConfigureObstacle(obstacle, layout);
        RecordGeneratedObstacle(obstacle, layout);
        obstacle.GetComponent<PipeObstacle>().Initialize(
            bird,
            scoreManager,
            audioManager,
            layout.Type.ToString());
        activeObstacles.Add(obstacle);
        MaximumActiveObstacleCount = Mathf.Max(
            MaximumActiveObstacleCount,
            activeObstacles.Count);

        bool spawnedSonic = gameManager.ArcadeFeaturesEnabled
            && sonicPowerUpPrefab != null
            && Random.value < sonicSpawnProbability;
        if (spawnedSonic)
        {
            SpawnSonicPowerUp(obstacle.transform, layout);
        }
        else if (gameManager.ArcadeFeaturesEnabled
            && coinPrefab != null
            && Random.value < coinSpawnProbability)
        {
            SpawnCoin(obstacle.transform, layout);
        }

        previousObstacleX = worldX;
        previousRouteBottom = layout.RouteBottom;
        previousRouteTop = layout.RouteTop;
        hasPreviousObstacle = true;
    }

    private void RecordGeneratedObstacle(GameObject obstacle, PipeLayout layout)
    {
        TotalGeneratedObstacleCount++;
        recentObstacleXPositions.Add(obstacle.transform.position.x);
        if (recentObstacleXPositions.Count > 32)
        {
            recentObstacleXPositions.RemoveAt(0);
        }

        switch (layout.Type)
        {
            case PipeObstacleType.StandardPair:
                StandardPairCount++;
                break;
            case PipeObstacleType.AsymmetricPair:
                AsymmetricPairCount++;
                break;
            case PipeObstacleType.BottomOnly:
                BottomOnlyCount++;
                break;
            case PipeObstacleType.TopOnly:
                TopOnlyCount++;
                break;
        }

        if (!layout.HasBottomPipe)
        {
            return;
        }

        BottomPipesValidated++;
        BoxCollider2D bottomCollider = obstacle.transform.Find("BottomPipe")
            .GetComponent<BoxCollider2D>();
        bool lowerEdgeIsValid = bottomCollider.bounds.min.y
            >= lowerPipeExtent - 0.001f;
        bool capIsAboveGround = bottomCollider.bounds.max.y > groundTopY;
        if (!lowerEdgeIsValid || !capIsAboveGround)
        {
            InvalidBottomPipeCount++;
        }
    }

    private void SpawnSonicPowerUp(Transform obstacleRoot, PipeLayout layout)
    {
        float safeBottom = layout.RouteBottom + 0.5f;
        float safeTop = layout.RouteTop - 0.5f;
        float worldY = Random.Range(safeBottom, safeTop);
        GameObject powerUp = Instantiate(
            sonicPowerUpPrefab,
            new Vector3(obstacleRoot.position.x, worldY, 0f),
            Quaternion.identity,
            obstacleRoot);
        powerUp.name = "SonicPowerUp";
        powerUp.GetComponent<SonicPowerUp>().Initialize(audioManager);
    }

    private void SpawnCoin(Transform obstacleRoot, PipeLayout layout)
    {
        float safeBottom = layout.RouteBottom + 0.45f;
        float safeTop = layout.RouteTop - 0.45f;
        float worldY = Random.Range(safeBottom, safeTop);
        GameObject coin = Instantiate(
            coinPrefab,
            new Vector3(obstacleRoot.position.x, worldY, 0f),
            Quaternion.identity,
            obstacleRoot);
        coin.name = "Coin";
        coin.GetComponent<CoinCollectible>().Initialize(scoreManager, audioManager);
        TotalCoinsSpawned++;
    }

    private PipeLayout GetValidatedLayout(float spacingFromPrevious)
    {
        PipeObstacleType type = layoutGenerator.ChooseObstacleType();

        for (int attempt = 0; attempt < maximumRerollAttempts; attempt++)
        {
            PipeLayout layout = layoutGenerator.CreateRandomLayout(type);
            if (IsLayoutPlayable(layout, spacingFromPrevious))
            {
                return layout;
            }
        }

        bool mustOverlapPrevious = hasPreviousObstacle
            && spacingFromPrevious < closeSpacingThreshold;
        PipeLayout fallback = layoutGenerator.CreateSafeFallbackLayout(
            type,
            mustOverlapPrevious,
            previousRouteBottom,
            previousRouteTop);
        if (IsLayoutPlayable(fallback, spacingFromPrevious))
        {
            return fallback;
        }

        PipeLayout emergencyFallback = layoutGenerator.CreateEmergencyFallbackLayout();
        if (IsLayoutPlayable(emergencyFallback, spacingFromPrevious))
        {
            return emergencyFallback;
        }

        throw new System.InvalidOperationException(
            "Pipe configuration cannot produce a validated fallback layout.");
    }

    private bool IsLayoutPlayable(PipeLayout layout, float spacingFromPrevious)
    {
        return layoutValidator.IsPlayable(
            layout,
            spacingFromPrevious,
            hasPreviousObstacle,
            previousRouteBottom,
            previousRouteTop);
    }

    private void ConfigureObstacle(GameObject obstacle, PipeLayout layout)
    {
        Transform topPipe = obstacle.transform.Find("TopPipe");
        Transform bottomPipe = obstacle.transform.Find("BottomPipe");

        topPipe.gameObject.SetActive(layout.HasTopPipe);
        bottomPipe.gameObject.SetActive(layout.HasBottomPipe);

        if (layout.HasTopPipe)
        {
            ConfigurePipe(topPipe, upperPipeExtent - layout.TopPipeLength, upperPipeExtent);
        }

        if (layout.HasBottomPipe)
        {
            ConfigurePipe(bottomPipe, lowerPipeExtent, lowerPipeExtent + layout.BottomPipeLength);
        }
    }

    private void ConfigurePipe(Transform pipe, float bottomEdge, float topEdge)
    {
        float pipeLength = topEdge - bottomEdge;
        Vector3 position = pipe.localPosition;
        position.y = (bottomEdge + topEdge) * 0.5f;
        pipe.localPosition = position;

        SpriteRenderer spriteRenderer = pipe.GetComponent<SpriteRenderer>();
        spriteRenderer.drawMode = SpriteDrawMode.Tiled;
        spriteRenderer.size = new Vector2(2f, pipeLength);

        BoxCollider2D pipeCollider = pipe.GetComponent<BoxCollider2D>();
        pipeCollider.size = new Vector2(1.75f, pipeLength);
        pipeCollider.isTrigger = false;
    }

    private int CountNearbyObstacles()
    {
        if (targetCamera == null)
        {
            return 0;
        }

        float cameraLeft = targetCamera.transform.position.x
            - targetCamera.orthographicSize * targetCamera.aspect
            - nearbyBehindCameraMargin;
        float nearbyRight = bird.position.x + spawnAheadDistance;
        int count = 0;

        foreach (GameObject obstacle in activeObstacles)
        {
            if (obstacle != null
                && obstacle.transform.position.x >= cameraLeft
                && obstacle.transform.position.x <= nearbyRight)
            {
                count++;
            }
        }

        return count;
    }

    private void RemoveObstaclesBehindCamera()
    {
        float cameraLeftEdge = targetCamera.transform.position.x
            - targetCamera.orthographicSize * targetCamera.aspect;
        float cleanupX = cameraLeftEdge - cleanupBehindCameraMargin;

        for (int i = activeObstacles.Count - 1; i >= 0; i--)
        {
            if (activeObstacles[i] == null || activeObstacles[i].transform.position.x < cleanupX)
            {
                if (activeObstacles[i] != null)
                {
                    Destroy(activeObstacles[i]);
                }

                activeObstacles.RemoveAt(i);
            }
        }
    }
}
