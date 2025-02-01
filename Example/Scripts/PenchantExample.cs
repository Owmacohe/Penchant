using System;
using System.Collections.Generic;
using Penchant.Runtime;
using UnityEngine;

public class PenchantExample : MonoBehaviour
{
    [SerializeField] Transform tree;
    [SerializeField] int size = 10;

    SeededRandom treeRandom;

    List<Transform> trees = new();
    int treeCount;

    void Start()
    {
        treeRandom = new SeededRandom("penchant_example_seed");
        SetTreeCount(0.5f);
    }

    public void SetSeed(string seed)
    {
        treeRandom.Seed = seed;
        GenerateTrees();
    }

    public void SetTreeCount(float count)
    {
        treeCount = (int)(count * 50);
        treeRandom.Reset();
        GenerateTrees();
    }

    void GenerateTrees()
    {
        foreach (var oldTree in trees)
            Destroy(oldTree.gameObject);
        
        trees.Clear();
        
        for (int i = 0; i < treeCount; i++)
        {
            var newTree = Instantiate(
                tree,
                new Vector3(treeRandom.RandomRange(-(float)size, (float)size), 0, treeRandom.RandomRange(-(float)size, (float)size)),
                Quaternion.Euler(Vector3.up * (treeRandom.RandomValue * 360)),
                transform);
            
            trees.Add(newTree);
        }
    }
}