using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Node
{
    public Vector2Int MatrixPos { get; set; }
    public Vector2 NodePos { get; set; }
    public bool IsValid { get; set; }
    public List<AI_Node> Neighbours { get; private set; }

    public AI_Node(bool isValid, Vector2 pos)
    {
        NodePos = pos; IsValid = isValid;
        Neighbours = new List<AI_Node>();
    }

    public void AddNeighbour(AI_Node node)
    {
        Neighbours.Add(node);
    }

    public AI_Node GetValidNeighbour()
    {
        foreach(var n in Neighbours)
        {
            if (n.IsValid) return n;
        }
        return null;
    }
}