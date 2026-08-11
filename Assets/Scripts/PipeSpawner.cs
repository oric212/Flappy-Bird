using System.Collections.Generic;
using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    private enum ObstacleType
    {
        StandardPair,
        BottomOnly,
        TopOnly,
        AsymmetricPair
    }

    private struct ObstacleLayout
    {
        public ObstacleType type;
        public bool hasTopPipe;
        public bool hasBottomPipe;
        public float topPipeLength;
        public float bottomPipeLength;
        public float routeBottom;
        public float routeTop;
    }

    [Header("References")]
    [SerializeField] private Transform bird;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private GameObject pipeObstaclePrefab;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private GameManager gameManager;

    [Header("Obstacle Type Weights")]
    [SerializeField] private float standardPairWeight = 35f;
    [SerializeField] private float bottomOnlyWeight = 25f;
    [SerializeField] private float topOnlyWeight = 25f;
    [SerializeField] private float asymmetricPairWeight = 15f;

    [Header("Horizontal Placement")]
    [SerializeField] private float minimumSpacing = 6f;
    [SerializeField] private float maximumSpacing = 11f;
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
    [SerializeField] private float minimumPipeLength = 2.2f;
    [SerializeField] private float maximumPipeLength = 5f;

    [Header("Validation")]
    [SerializeField] private float closeSpacingThreshold = 9.5f;
    [SerializeField] private float minimumCloseRouteOverlap = 1.1f;
    [SerializeField] private int maximumRerollAttempts = 8;

    [Header("World Extents")]
    [SerializeField] private float lowerPipeExtent = -5.5f;
    [SerializeField] private float upperPipeExtent = 5.5f;

    private readonly List<GameObject> activeObstacles = new List<GameObject>();
    private float nextSpawnX;
    private float previousObstacleX;
    private float previousRouteBottom;
    private float previousRouteTop;
    private bool hasPreviousObstacle;
    private int nearbyTarget;

    private void Start()
    {
        if (bird == null || pipeObstaclePrefab == null
            || scoreManager == null || gameManager == null)
        {
            enabled = false;
            return;
        }

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        nearbyTarget = Random.Range(minimumNearbyObstacles, maximumNearbyObstacles + 1);
        nextSpawnX = bird.position.x + Random.Range(minimumSpacing, maximumSpacing);
        MaintainObstacleDensity();
    }

    private void Update()
    {
        if (gameManager.IsGameOver)
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
        ObstacleLayout layout = GetValidatedLayout(spacingFromPrevious);
        GameObject obstacle = Instantiate(
            pipeObstaclePrefab,
            new Vector3(worldX, 0f, 0f),
            Quaternion.identity);

        obstacle.name = "PipeObstacle_" + layout.type;
        ConfigureObstacle(obstacle, layout);
        obstacle.GetComponent<PipeObstacle>().Initialize(
            bird,
            scoreManager,
            layout.type.ToString());
        activeObstacles.Add(obstacle);

        previousObstacleX = worldX;
        previousRouteBottom = layout.routeBottom;
        previousRouteTop = layout.routeTop;
        hasPreviousObstacle = true;
    }

    private ObstacleLayout GetValidatedLayout(float spacingFromPrevious)
    {
        for (int attempt = 0; attempt < maximumRerollAttempts; attempt++)
        {
            ObstacleLayout layout = CreateRandomLayout();

            if (IsLayoutPlayable(layout, spacingFromPrevious))
            {
                return layout;
            }
        }

        return CreateSafeFallbackLayout();
    }

    private ObstacleLayout CreateRandomLayout()
    {
        ObstacleType type = ChooseObstacleType();

        if (type == ObstacleType.BottomOnly)
        {
            float length = Random.Range(minimumPipeLength, maximumPipeLength);
            return new ObstacleLayout
            {
                type = type,
                hasBottomPipe = true,
                bottomPipeLength = length,
                routeBottom = lowerPipeExtent + length,
                routeTop = maximumGapCenter + maximumGapSize * 0.5f
            };
        }

        if (type == ObstacleType.TopOnly)
        {
            float length = Random.Range(minimumPipeLength, maximumPipeLength);
            return new ObstacleLayout
            {
                type = type,
                hasTopPipe = true,
                topPipeLength = length,
                routeBottom = minimumGapCenter - maximumGapSize * 0.5f,
                routeTop = upperPipeExtent - length
            };
        }

        if (type == ObstacleType.AsymmetricPair)
        {
            float topLength = Random.Range(minimumPipeLength, maximumPipeLength);
            float bottomLength = Random.Range(minimumPipeLength, maximumPipeLength);
            return new ObstacleLayout
            {
                type = type,
                hasTopPipe = true,
                hasBottomPipe = true,
                topPipeLength = topLength,
                bottomPipeLength = bottomLength,
                routeBottom = lowerPipeExtent + bottomLength,
                routeTop = upperPipeExtent - topLength
            };
        }

        float gapCenter = Random.Range(minimumGapCenter, maximumGapCenter);
        float gapSize = Random.Range(minimumGapSize, maximumGapSize);
        float gapBottom = gapCenter - gapSize * 0.5f;
        float gapTop = gapCenter + gapSize * 0.5f;

        return new ObstacleLayout
        {
            type = ObstacleType.StandardPair,
            hasTopPipe = true,
            hasBottomPipe = true,
            topPipeLength = upperPipeExtent - gapTop,
            bottomPipeLength = gapBottom - lowerPipeExtent,
            routeBottom = gapBottom,
            routeTop = gapTop
        };
    }

    private ObstacleType ChooseObstacleType()
    {
        float totalWeight = standardPairWeight + bottomOnlyWeight
            + topOnlyWeight + asymmetricPairWeight;
        float choice = Random.Range(0f, totalWeight);

        if (choice < standardPairWeight)
        {
            return ObstacleType.StandardPair;
        }

        choice -= standardPairWeight;
        if (choice < bottomOnlyWeight)
        {
            return ObstacleType.BottomOnly;
        }

        choice -= bottomOnlyWeight;
        if (choice < topOnlyWeight)
        {
            return ObstacleType.TopOnly;
        }

        return ObstacleType.AsymmetricPair;
    }

    private bool IsLayoutPlayable(ObstacleLayout layout, float spacingFromPrevious)
    {
        float routeHeight = layout.routeTop - layout.routeBottom;
        if (routeHeight < minimumGapSize)
        {
            return false;
        }

        if (!hasPreviousObstacle || spacingFromPrevious >= closeSpacingThreshold)
        {
            return true;
        }

        float sharedRouteBottom = Mathf.Max(previousRouteBottom, layout.routeBottom);
        float sharedRouteTop = Mathf.Min(previousRouteTop, layout.routeTop);
        return sharedRouteTop - sharedRouteBottom >= minimumCloseRouteOverlap;
    }

    private ObstacleLayout CreateSafeFallbackLayout()
    {
        const float fallbackGapSize = 3.2f;
        return new ObstacleLayout
        {
            type = ObstacleType.StandardPair,
            hasTopPipe = true,
            hasBottomPipe = true,
            topPipeLength = upperPipeExtent - fallbackGapSize * 0.5f,
            bottomPipeLength = fallbackGapSize * 0.5f - lowerPipeExtent,
            routeBottom = -fallbackGapSize * 0.5f,
            routeTop = fallbackGapSize * 0.5f
        };
    }

    private void ConfigureObstacle(GameObject obstacle, ObstacleLayout layout)
    {
        Transform topPipe = obstacle.transform.Find("TopPipe");
        Transform bottomPipe = obstacle.transform.Find("BottomPipe");

        topPipe.gameObject.SetActive(layout.hasTopPipe);
        bottomPipe.gameObject.SetActive(layout.hasBottomPipe);

        if (layout.hasTopPipe)
        {
            ConfigurePipe(topPipe, upperPipeExtent - layout.topPipeLength, upperPipeExtent);
        }

        if (layout.hasBottomPipe)
        {
            ConfigurePipe(bottomPipe, lowerPipeExtent, lowerPipeExtent + layout.bottomPipeLength);
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
