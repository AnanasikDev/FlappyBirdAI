using System.IO;
using System.Text;
using UnityEngine;

public class RecordTracker : MonoBehaviour
{

    public static RecordTracker instance;
    private void Start()
    {
        instance = this;
    }
    public static void RecordData(BirdModel model)
    {
        StringBuilder data = new StringBuilder();
        foreach (var w in model.weights)
        {
            data.Append(w.ToString() + "\n");
        }
        using (StreamWriter outputFile = new StreamWriter("bestModel.txt"))
        {
            outputFile.WriteLine(data.ToString());
        }
    }
}
