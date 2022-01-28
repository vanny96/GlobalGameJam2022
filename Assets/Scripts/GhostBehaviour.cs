using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

public class GhostBehaviour : NetworkBehaviour
{
    [Networked] public bool Angry { get; set; }
    [SerializeField] [Networked] private Vector3 currentDestination { get; set; }
    [SerializeField] [Networked] private float speed { get; set; }
    [HideInInspector] public Bounds destinationBounds;

    private new Rigidbody rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        currentDestination = GetNextDestination();
        Debug.Log(currentDestination);
    }

    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        if (Angry) { }
        else
        {
            CalmPattern();
        }
    }

    private void CalmPattern()
    {
        Vector3 direction = (currentDestination - transform.position);
        Vector3 movement = direction.normalized * speed * Runner.DeltaTime;

        if (movement.magnitude > direction.magnitude) movement = direction;

        Vector3 endPosition = transform.position + movement;
        endPosition.y = transform.position.y;

        rigidbody.MovePosition(endPosition);

        if (transform.position == currentDestination)
            currentDestination = GetNextDestination();
    }

    private Vector3 GetNextDestination()
    {
        return RandomCoordinates.FromBoundsAndY(destinationBounds, transform.position.y);
    }
}
