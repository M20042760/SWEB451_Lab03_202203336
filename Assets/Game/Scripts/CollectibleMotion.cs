using UnityEngine;

public class CollectibleMotion : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 60f;

    [Header("Bobbing")]
    [SerializeField] private bool enableBobbing = true;
    [SerializeField] private float bobHeight = 0.25f;
    [SerializeField] private float bobSpeed = 2f;

    private Vector3 startLocalPosition;

    private void Awake()
    {
        startLocalPosition = transform.localPosition;
    }

    private void OnEnable()
    {
        Debug.Log($"{name} is ready.");
    }

    private void Update()
    {
        // Rotation always runs.
        transform.Rotate(
            Vector3.up,
            rotationSpeed * Time.deltaTime,
            Space.Self);
        // Bobbing is optional, while rotation remains active.
        if (enableBobbing)
        {
            float offsetY =
                Mathf.Sin(Time.time * bobSpeed) * bobHeight;

            transform.localPosition =
                startLocalPosition + Vector3.up * offsetY;
        }
        else
        {
            // Return to the original position when bobbing is disabled.
            transform.localPosition = startLocalPosition;
        }
    }
}