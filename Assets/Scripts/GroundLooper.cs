using UnityEngine;

public class GroundLooper : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;

    private BoxCollider2D[] chunkColliders;
    private float chunkWidth;

    private void Awake()
    {
        chunkColliders = GetComponentsInChildren<BoxCollider2D>();

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (chunkColliders.Length > 0)
        {
            chunkWidth = chunkColliders[0].bounds.size.x;
        }
    }

    private void LateUpdate()
    {
        if (targetCamera == null || chunkWidth <= 0f)
        {
            return;
        }

        float cameraLeftEdge = targetCamera.transform.position.x
            - targetCamera.orthographicSize * targetCamera.aspect;

        // A chunk can recycle at most once per frame. This bounded loop also
        // lets several different chunks catch up after a larger camera step.
        for (int i = 0; i < chunkColliders.Length; i++)
        {
            if (!RecycleLeftmostChunkBehind(cameraLeftEdge))
            {
                break;
            }
        }
    }

    private bool RecycleLeftmostChunkBehind(float cameraLeftEdge)
    {
        BoxCollider2D leftmostChunk = chunkColliders[0];

        foreach (BoxCollider2D chunkCollider in chunkColliders)
        {
            if (chunkCollider.transform.position.x < leftmostChunk.transform.position.x)
            {
                leftmostChunk = chunkCollider;
            }
        }

        float chunkRightEdge = leftmostChunk.transform.position.x + chunkWidth * 0.5f;

        if (chunkRightEdge >= cameraLeftEdge)
        {
            return false;
        }

        Vector3 position = leftmostChunk.transform.position;
        position.x = GetRightmostChunkX() + chunkWidth;
        leftmostChunk.transform.position = position;
        return true;
    }

    private float GetRightmostChunkX()
    {
        float rightmostX = chunkColliders[0].transform.position.x;

        foreach (BoxCollider2D chunkCollider in chunkColliders)
        {
            rightmostX = Mathf.Max(rightmostX, chunkCollider.transform.position.x);
        }

        return rightmostX;
    }
}
