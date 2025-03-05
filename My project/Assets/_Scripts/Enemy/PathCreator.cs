using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCreator : MonoBehaviour
{
    [SerializeField] private List<Transform> pathPoints;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public Transform GetNextPoint(ref int idx)
    {
        if (pathPoints.Count == 0)
        {
            return null;
        }

        idx++;
        idx %= pathPoints.Count;
        return pathPoints[idx];
    }
}
