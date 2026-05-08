using System.Collections.Generic;
using UnityEngine;

public class GridLogic : MonoBehaviour
{
    [SerializeField] private GameObject[] trapPrefab;
    private Dictionary<int, GameObject> grid = new Dictionary<int, GameObject>();

    void Start()
    {
        int index = 0;
        foreach (GameObject trap in trapPrefab)
        {
            index++;
            grid.Add(index, trap);
        }
        OnReceiveInput(3);
    }

    void OnReceiveInput(int index)
    {
        grid.TryGetValue(index, out GameObject trap);
        trap.SetActive(false);
    }
}
