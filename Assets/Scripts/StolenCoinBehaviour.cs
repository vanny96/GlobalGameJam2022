using UnityEngine;
using Fusion;

public class StolenCoinBehaviour : MonoBehaviour
{
    public Transform target;
    public float speed;
    public float marginOfDestruction;

    // Update is called once per frame
    void Update()
    {
        Move();
        DestroyIfArrived();
    }

    private void Move()
    {
        Vector3 direction = target.position - transform.position;
        transform.Translate(direction.normalized * speed * Time.deltaTime);
    }

    private void DestroyIfArrived()
    {
        Vector3 distanceVector = target.position - transform.position;
        if(distanceVector.magnitude < marginOfDestruction)
        {
            Destroy(this.gameObject);
        }
    }
}
