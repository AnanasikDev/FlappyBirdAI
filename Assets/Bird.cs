using UnityEngine;

// Neural network-controlled bird model
public class Bird : MonoBehaviour
{
    private Rigidbody2D rigidbody2d;
    private Vector3 initialPosition;
    public float jumpIntensity = 1f;
    public BirdModel model; // settings and results of this model

    public float horizontalDistance; // horizontal distance to the nearest column, if it is visible
    public float height;             // vertical distance to midline of the screen
    public float verticalSpeed;      // vertical speed
    public float bottomEdge;         // vertical distance to the top edge of the bottom column of the nearest obstacle, if visible
    public float topEdge;            // vertical distance to the bototm edge of the top column of the nearest obstacle, if visible. Used for success evaluation

    public float value;     // neural network output that is to decide whether to jump or not

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
        // Based on input values and weights decides, whether to jump or not
        value = height * model.weights[0] + 
                verticalSpeed * model.weights[1] +
                horizontalDistance * model.weights[2] +
                bottomEdge * model.weights[3];
        return value > 0;
    }

    private void FixedUpdate()
    {
        height = transform.position.y; // 0 is at the midheight of the screen
        verticalSpeed = rigidbody2d.velocity.y;

        horizontalDistance = Mathf.Clamp((Vector3.Distance(transform.position, new Vector3(ColumnSpawner.instance.nextColumn.transform.position.x, transform.position.y))), 0, 11);
        
        bottomEdge = horizontalDistance < 11 ? // only if next column is visible
                ColumnSpawner.instance.nextColumn.bottomEdge.transform.position.y - transform.position.y
                : 0;
        topEdge = horizontalDistance < 11 ?
                transform.position.y - ColumnSpawner.instance.nextColumn.topEdge.transform.position.y
                : 0;

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
        model.r_success = GeneticAlgorithm.instance.timeSinceBegin * Mathf.Pow(ScoreManager.instance.score, 2) / (topEdge - bottomEdge != 0 ? Mathf.Pow(Mathf.Abs(topEdge - bottomEdge), 2) : 1);
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

    public float r_success;

    public BirdModel Clone()
    {
        var clone = new BirdModel();
        clone.Inherit(weights);
        clone.r_score = r_score;
        clone.r_time = r_time;
        clone.r_success = r_success;
        return clone;
    }

    public void Randomize()
    {
        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] = (Random.value - 0.5f) * 2;
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
        // Mutation algorithm

        for (int w = 0; w < weights.Length; w++)
        {
            float v = 1 + Random.Range(-strictness, strictness);
            weights[w] = weights[w] * v;
        }
    }
}