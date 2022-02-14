using UnityEngine;
using UnityEngine.AI;

public class LineBuilder : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;

    [HideInInspector] public Transform destination;

    private NavMeshPath path;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Start()
    {
        path = new NavMeshPath();
    }

    void Update()
    {
        NavMesh.CalculatePath(this.transform.position, destination.position, NavMesh.AllAreas, path);
        Vector3[] corners = path.corners;

        lineRenderer.positionCount = corners.Length;
        lineRenderer.SetPositions(corners);
    }
}
