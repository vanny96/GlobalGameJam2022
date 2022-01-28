using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

public class GhostBehaviour : NetworkBehaviour
{
    [Networked] public bool Angry { get; set; }
    [Networked] private Vector3 currentDestination { get; set; }
    [Networked] private float speed { get; set; }

    [SerializeField] private List<Vector2> pathCoordinates;
    private Queue<Vector2> pathQueue;
    private new Rigidbody rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();

        FillPathQueue();
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
        Vector3 movement = transform.position +
            (direction.normalized * speed * Runner.DeltaTime);

        if (movement.magnitude > direction.magnitude) movement = direction;

        rigidbody.MovePosition(movement);

        if (transform.position == currentDestination)
            currentDestination = GetNextDestination();
    }

    private void FillPathQueue()
    {
        pathQueue = new Queue<Vector2>(pathCoordinates);
    }

    private Vector3 GetNextDestination()
    {
        if (pathQueue.Count == 0) FillPathQueue();
        return pathQueue.Dequeue();
    }
}
