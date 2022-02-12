using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineBuilder : MonoBehaviour
{
    private LineRenderer lineRenderer;

    [SerializeField] private float dotLength;
    [SerializeField] private Transform[] objects;

    [SerializeField] float distanceToCover;


    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.positionCount = objects.Length;

        for(int i=0; i<objects.Length; i++)
        {
            lineRenderer.SetPosition(i, objects[i].position);
        }
    }
}
