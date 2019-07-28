using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaypointMatrix : MonoBehaviour
{
    static public WaypointMatrix Instance { get; private set; }

    [Header("Refs")]
    public float cellSize;
    public Transform gridCenter;

    [Header("Parameters")]
    [Range(1, 50)]
    public int AI_GridSizeX;
    [Range(1, 50)]
    public int AI_GridSizeY;
    public ContactFilter2D obstacleDetectionFilter = new ContactFilter2D();
    public bool useRadius;
    public float obstacleDetectionRadius = 0.5f;
    public float gridUpdateTime = 0.5f;

    public int maxPathfindSteps = 30;

    [Header("Gizmos")]
    public bool drawGizmos;


    private Vector2 halfTileSize = Vector2.one;
    int AI_GridHalfSizeX, AI_GridHalfSizeY;
    private Vector2 centerPos;
    private AI_Node[] _nodes;

    private void Awake()
    {
        if (Instance == null)
        { Instance = this; Initialize(); }
        else
            Destroy(gameObject);
    }


    private void Initialize()
    {
        halfTileSize = Vector2.one * cellSize * 0.5f;
        AI_GridHalfSizeX = AI_GridSizeX / 2;
        AI_GridHalfSizeY = AI_GridSizeY / 2;

        CreateNodeGrid();
        UpdateNodePosition();
    }

    private void CreateNodeGrid()
    {
        _nodes = new AI_Node[AI_GridSizeX * AI_GridSizeY];
        AI_Node currentNode;
        for (int y = 0; y < AI_GridSizeY; y++)
        {
            for (int x = 0; x < AI_GridSizeX; x++)
            {
                currentNode = new AI_Node(true, Vector2.zero);
                currentNode.MatrixPos = new Vector2Int(x, y);
                _nodes[x + y * AI_GridSizeX] = currentNode;


                if (x > 0)
                {
                    _nodes[x - 1 + y * AI_GridSizeX].AddNeighbour(currentNode);
                    currentNode.AddNeighbour(_nodes[x - 1 + y * AI_GridSizeX]);
                }
                if (y > 0)
                {
                    _nodes[x + (y - 1) * AI_GridSizeX].AddNeighbour(currentNode);
                    currentNode.AddNeighbour(_nodes[x + (y - 1) * AI_GridSizeX]);
                }
            }
        }
    }


    Collider2D[] result = new Collider2D[1];


    private void UpdateNodePosition()
    {
        centerPos = gridCenter.position;
        centerPos.x /= cellSize;
        centerPos.x = Mathf.FloorToInt(centerPos.x) * cellSize;
        centerPos.y /= cellSize;
        centerPos.y = Mathf.FloorToInt(centerPos.y) * cellSize;

        AI_Node currentNode;
        for (int y = 0; y < AI_GridSizeY; y++)
        {
            for (int x = 0; x < AI_GridSizeX; x++)
            {
                currentNode = _nodes[x + y * AI_GridSizeX];
                currentNode.NodePos = centerPos + new Vector2((x + 1 - AI_GridHalfSizeX) * cellSize, (y + 1 - AI_GridHalfSizeY) * cellSize);
                if (useRadius ? Physics2D.OverlapCircle(currentNode.NodePos, obstacleDetectionRadius, obstacleDetectionFilter, result) > 0 : Physics2D.OverlapPoint(currentNode.NodePos, obstacleDetectionFilter, result) > 0)
                    currentNode.IsValid = false;
                else
                    currentNode.IsValid = true;
            }
        }
    }

    private List<AI_Node> GetPathTo(AI_Node startNode, AI_Node endNode)
    {
        //return ThetaStar.RunWithoutWeight(startNode, n => n == endNode, n => n.Neighbours, n => Vector2.Distance(n.NodePos, endNode.NodePos), IsNodeInSight, (n1, n2) => Vector2.Distance(n1.NodePos, n2.NodePos), n => n.IsValid, maxPathfindSteps);
        return AStar.Run(startNode, n => n == endNode, n => n.Neighbours.Select(neighbour => (neighbour, 1f)), n => Vector2.Distance(endNode.NodePos, n.NodePos), n => n.IsValid, maxPathfindSteps);
    }

    private bool IsNodeInSight(AI_Node startNode, AI_Node targetNode)
    {
        float t;
        int xDist = startNode.MatrixPos.x - targetNode.MatrixPos.x;
        xDist = xDist > 0 ? xDist : -xDist;
        int yDist = startNode.MatrixPos.y - targetNode.MatrixPos.y;
        yDist = yDist > 0 ? yDist : -yDist;
        int diagonalDist = Mathf.Max(xDist, yDist);
        bool sideSight;
        for (int step = 0; step < diagonalDist; step++)
        {
            sideSight = false;
            t = (step == 0) ? 0 : step / (float)diagonalDist;
            Vector2Int pointMatrixPos = Vector2Int.FloorToInt(Vector2.Lerp(startNode.MatrixPos, targetNode.MatrixPos, t));
            if (!_nodes[pointMatrixPos.x + pointMatrixPos.y * AI_GridSizeX].IsValid) return false;

            foreach (var side in _nodes[pointMatrixPos.x + pointMatrixPos.y * AI_GridSizeX].Neighbours)
            { if (side.IsValid) { sideSight = true; break; } }
            if (!sideSight) return false;
        }
        return true;
    }

    public AI_Node GetNodeClosestTo(Vector2 worldPos)
    {
        var localPos = (worldPos - centerPos) + new Vector2(AI_GridHalfSizeX * cellSize, AI_GridHalfSizeY * cellSize);
        var xIndex = Mathf.Clamp(Mathf.RoundToInt(localPos.x / cellSize) - 1, 0, AI_GridSizeX - 1);
        var yIndex = Mathf.Clamp(Mathf.RoundToInt(localPos.y / cellSize) - 1, 0, AI_GridSizeY - 1);

        return _nodes[xIndex + yIndex * AI_GridSizeX];
    }
    //
    // PUBLIC
    //

    public List<Vector3> GetPositionPathTo(Vector3 startPos, Vector3 endPos)
    {
        List<Vector3> path = new List<Vector3>();
        AI_Node startNode = GetNodeClosestTo(new Vector2(startPos.x, startPos.z)), endNode = GetNodeClosestTo(new Vector2(endPos.x, endPos.z));
        //if (!startNode.IsValid)
        //    startNode = startNode.GetValidNeighbour() ?? startNode;

        if (!endNode.IsValid)
            endNode = endNode.GetValidNeighbour() ?? endNode;

        var nodePath = GetPathTo(startNode, endNode);
        if (nodePath == null) return path;

        foreach (var node in nodePath)
        { path.Add(new Vector3(node.NodePos.x, 0, node.NodePos.y)); }

        return path;
    }

    public bool IsPositionOnSight(Vector2 startPos, Vector2 endPos)
    {
        AI_Node startNode = GetNodeClosestTo(startPos), endNode = GetNodeClosestTo(endPos);
        return IsNodeInSight(startNode, endNode);
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;
        if (_nodes == null) return;

        foreach (var node in _nodes)
        {
            Gizmos.color = node.IsValid ? Color.white : Color.red;
            Gizmos.DrawWireSphere(new Vector3(node.NodePos.x, 0, node.NodePos.y), 0.1f);

            Gizmos.color = Color.green;
            //foreach (var n in node.GetNeighbours())
            //    Gizmos.DrawLine(node.NodePos, n.NodePos);
        }
    }
}
