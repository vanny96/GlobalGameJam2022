using UnityEngine;
using UnityEngine.AI;

public class LineBuilder : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;

    [HideInInspector] public NavMeshAgent playerAgent;
    [HideInInspector] public Transform destination;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        playerAgent.destination = destination.position;
        Vector3[] corners = playerAgent.path.corners;

        lineRenderer.positionCount = corners.Length;
        lineRenderer.SetPositions(corners);
    }
}
