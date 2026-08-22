using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float horizontalOffset = 1.5f;
    [SerializeField] private GameManager gameManager;

    private float fixedY;
    private float fixedZ;

    private void Awake()
    {
        fixedY = transform.position.y;
        fixedZ = transform.position.z;
    }

    private void LateUpdate()
    {
        if (target == null || !gameManager.IsPlaying)
        {
            return;
        }

        transform.position = new Vector3(target.position.x + horizontalOffset, fixedY, fixedZ);
    }
}
