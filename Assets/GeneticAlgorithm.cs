using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GeneticAlgorithm : MonoBehaviour
{
    public GameObject unit;
    public int numberOfUnits = 10;
    public int unitsLeft = 10;
    [SerializeField] private BirdModel[] models;

    public float successStrictness = 0.2f; // portion of the best to be selected for the next generation
    public float geneticStrictness = 0.075f; // how much generations differ
    private int unitVersionsNumber;

    private float iterationBeginTime;
    public float timeSinceBegin { get { return Time.time - iterationBeginTime; } }

    public static GeneticAlgorithm instance;

    public int iteration = 0;
    public TextMeshProUGUI iterationText;

    private void Start()
    {
        unitVersionsNumber = (int)(1f / successStrictness);
        instance = this;
        models = new BirdModel[numberOfUnits];
        for (int i = 0; i < numberOfUnits; i++)
        {
            models[i] = new BirdModel();
            models[i].Randomize();
        }

        Begin();
    }
    private void Begin()
    {
        iteration++;
        iterationText.text = $"Iteration: {iteration}";

        unitsLeft = numberOfUnits; 
        iterationBeginTime = Time.time;
        for (int i = 0; i < numberOfUnits; i++)
        {
            var bird = Instantiate(unit);
            bird.GetComponent<Bird>().model = models[i];
        }
    }

    private float ModelSuccessEvaluation(BirdModel m)
    {
        return m.r_time + Mathf.Pow(m.r_score, 2);
    }
    private void Update()
    {
        if (unitsLeft <= 0)
        {
            int topn = (int)(numberOfUnits * successStrictness);
            List<BirdModel> topModels = models.OrderBy(m => ModelSuccessEvaluation(m)).ToList().GetRange(numberOfUnits-topn-1, topn);
            for (int p = 0; p < topModels.Count; p++)
            {
                for (int i = 0; i < unitVersionsNumber; i++)
                {
                    var m = models[p * unitVersionsNumber + i];
                    m.Inherit(topModels[p].weights);
                    m.Alter(geneticStrictness);
                }
            }
            unitsLeft = numberOfUnits;
            ScoreManager.instance.Restart();
            ColumnSpawner.instance.Restart();
            Begin();
        }
    }
}
