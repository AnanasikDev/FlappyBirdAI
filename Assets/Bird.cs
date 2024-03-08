using UnityEngine;

public class Bird : MonoBehaviour
{
    private Rigidbody2D rigidbody2d;
    public float jumpIntensity = 1f;
    public BirdModel model;

    public float horizontalDistance;
    //public float distanceToEscape;
    public float height;
    public float verticalSpeed;
    public float bottomEdge;
    public float topEdge;
    //public float verticalMiss;

    public float value;
    private Vector3 initialPosition;

    public void Init()
    {
        initialPosition = transform.position;
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    public void Jump()
    {
        rigidbody2d.velocity = new Vector2(0, jumpIntensity);
    }

    public bool Decide()
    {
        /*value = Mathf.Pow(height, model.weights[0]) * model.weights[1] +
                Mathf.Pow(verticalSpeed, model.weights[2]) * model.weights[3];// +
                //Mathf.Pow(horizontalDistance, model.weights[2]) * model.weights[3] +
                //Mathf.Pow(bottomEdge, model.weights[4]) * model.weights[5];// *
                //Mathf.Pow(topEdge, model.weights[5]) * model.weights[6];*/

        value = height * model.weights[0] + 
                verticalSpeed * model.weights[1] +
                horizontalDistance * model.weights[2] *
                bottomEdge * model.weights[3];
        return value > 0;
    }

    private void FixedUpdate()
    {
        height = transform.position.y;
        verticalSpeed = rigidbody2d.velocity.y;

        horizontalDistance = Mathf.Clamp((Vector3.Distance(transform.position, new Vector3(ColumnSpawner.instance.nextColumn.transform.position.x, transform.position.y))), 0, 11);
        //distanceToEscape = (Vector3.Distance(transform.position, ColumnSpawner.instance.nextColumn.transform.position));
        /*verticalMiss = horizontalDistance < 11 ?
            Mathf.Abs(ColumnSpawner.instance.nextColumn.top.transform.position.y - transform.position.y) - 
            Mathf.Abs(ColumnSpawner.instance.nextColumn.bottom.transform.position.y - transform.position.y) - 1
            : 0;*/
        bottomEdge = horizontalDistance < 11 ?
                ColumnSpawner.instance.nextColumn.bottomEdge.transform.position.y - transform.position.y
                : 0;
        /*topEdge = horizontalDistance < 11 ?
                transform.position.y - ColumnSpawner.instance.nextColumn.topEdge.transform.position.y
                : 0;*/

        if (Decide())
            Jump();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("ColumnCollider"))
        {
            Die();
        }
    }

    public void Revive()
    {
        gameObject.SetActive(true);
        rigidbody2d.simulated = true;
    }
    private void Die()
    {
        model.r_time = GeneticAlgorithm.instance.timeSinceBegin;
        model.r_score = ScoreManager.instance.score;
        GeneticAlgorithm.instance.unitsLeft--;
        gameObject.SetActive(false);
        gameObject.transform.position = initialPosition;
        rigidbody2d.velocity = Vector3.zero;
        rigidbody2d.simulated = false;
    }
}

[System.Serializable]
public class BirdModel
{
    public float[] weights = new float[4];
    public float r_time;
    public int r_score;

    public void Randomize()
    {
        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] = (Random.value - 0.5f) * 50;
        }
    }
    public void Inherit(float[] _weights)
    {
        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] = _weights[i];
        }
    }
    public void Alter(float strictness)
    {
        for (int w = 0; w < weights.Length; w++)
        {
            float v = Random.Range(-strictness, strictness);
            //weights[w] *= (Random.value < 0.5f ? 1f / v : v); // Random.Range(1f - strictness, 1f + strictness);
            weights[w] = weights[w] * v - v;
        }
    }
}