using UnityEngine;

public class Column : MonoBehaviour
{
    [HideInInspector] public bool passed = false; // prevents extra score increases
    [Tooltip("Top columm")]                         public Transform top;
    [Tooltip("Bottom column")]                      public Transform bottom;
    [Tooltip("Bottom edge of top column anchor")]   public Transform topEdge;
    [Tooltip("Top edge of bottom column anchor")]   public Transform bottomEdge;
}
