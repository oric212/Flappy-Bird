using UnityEngine;

public class BackgroundLooper : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;

    private SpriteRenderer[] chunks;
    private float chunkWidth;

    private void Awake()
    {
        chunks = GetComponentsInChildren<SpriteRenderer>();

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (chunks.Length > 0)
        {
            chunkWidth = chunks[0].bounds.size.x;
        }
    }

    private void LateUpdate()
    {
        if (targetCamera == null || chunks.Length == 0 || chunkWidth <= 0f)
        {
            return;
        }

        float cameraLeft = targetCamera.transform.position.x
            - targetCamera.orthographicSize * targetCamera.aspect;

        for (int i = 0; i < chunks.Length; i++)
        {
            SpriteRenderer leftmost = GetLeftmostChunk();
            if (leftmost.bounds.max.x >= cameraLeft)
            {
                break;
            }

            Vector3 position = leftmost.transform.position;
            position.x = GetRightmostChunk().transform.position.x + chunkWidth;
            leftmost.transform.position = position;
        }
    }

    private SpriteRenderer GetLeftmostChunk()
    {
        SpriteRenderer result = chunks[0];
        foreach (SpriteRenderer chunk in chunks)
        {
            if (chunk.transform.position.x < result.transform.position.x)
            {
                result = chunk;
            }
        }

        return result;
    }

    private SpriteRenderer GetRightmostChunk()
    {
        SpriteRenderer result = chunks[0];
        foreach (SpriteRenderer chunk in chunks)
        {
            if (chunk.transform.position.x > result.transform.position.x)
            {
                result = chunk;
            }
        }

        return result;
    }
}
