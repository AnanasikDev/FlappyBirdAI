using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ColumnSpawner : MonoBehaviour
{
    public GameObject column;
    public int number;
    public float distance;
    public int startX;
    public float heightVariance;

    private GameObject[] columns;
    public GameObject nextColumn { get { return columns.OrderBy(c => c.transform.position.x).FirstOrDefault(); } }

    public AnimationCurve speedCurve;
    public static ColumnSpawner instance;

    public float CalculateSpeed()
    {
        return speedCurve.Evaluate(ScoreManager.instance.score);
    }

    private void Start() => Begin();
    public void Begin()
    {
        instance = this;
        columns = new GameObject[number];
        for (int i = 0; i < number; i++)
        {
            columns[i] = Instantiate(column, new Vector3(startX + i * distance, (Random.value-0.5f) * heightVariance, 0), Quaternion.identity);
        }
    }

    private void Update()
    {
        foreach (var column in columns)
        {
            column.transform.Translate(Vector3.left * CalculateSpeed());
            if (column.transform.position.x < -20)
            {
                column.transform.position = new Vector3(startX, (Random.value - 0.5f) * heightVariance, 0);
            }
        }
    }

    public void Restart()
    {
        for (int i = 0; i < number; i++)
        {
            columns[i].transform.position = new Vector3(startX + i * distance, (Random.value - 0.5f) * heightVariance, 0);
        }
    }
}
