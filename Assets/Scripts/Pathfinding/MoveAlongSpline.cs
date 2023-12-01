using Assets.Scripts.Pathfinding;
using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class MoveAlongSpline : MonoBehaviour
{
    public SplineContainer splines;
    public ResourceManager manager;
    public ColorAsset[] pathAssets;
    public ColorEnum[] pathTypes;
    public ColorEnum[] resource;
    public Transform[] nodes;
    public int starterNodeIndex;
    public int finishNodeIndex;
    public Transform ship;
    public float speed = 1;

    private Dictionary<int, List<int>> nodeNeighbourSplines = new Dictionary<int, List<int>>();
    private Dictionary<SplineKnotIndex, int> nodeOfKnot = new Dictionary<SplineKnotIndex, int>();
    private List<ColorEnum> availResources = new List<ColorEnum>();
    private int currentSpline = 0;
    private float distancePercentage = 0f;
    bool stopMove = true;


    // Start is called before the first frame update
    void Start()
    {
        InstatiatePathAsset();
        findCorrespondKnots();
        availResources = resource.ToList();
        currentSpline = getNextSpline(starterNodeIndex);
        if(currentSpline == -1)
        {
            stopMove = true;
        }
        checkReverseSpline(starterNodeIndex, currentSpline);
        ship.position = nodes[starterNodeIndex].position;
    }

    // Update is called once per frame
    void Update()
    {
        if(!stopMove)
        {
            distancePercentage += speed * Time.deltaTime / splines.CalculateLength(currentSpline);
            Vector3 currPos = splines.EvaluatePosition(currentSpline, distancePercentage);
            ship.position = currPos;
            if(distancePercentage > 1f)
            {
                
                int currNodeIndex = nodeOfKnot[new SplineKnotIndex(currentSpline, 1)];
                availResources.RemoveAt(0);
                if(availResources.Count > 0)
                {
                    currentSpline = getNextSpline(currNodeIndex);
                    if(currentSpline > -1)
                    {
                        checkReverseSpline(currNodeIndex, currentSpline);

                    }
                    else {
                        stopMove = true;
                        Debug.Log("Gagal");
                    }

                }else if(currNodeIndex == finishNodeIndex && availResources.Count == 0)
                {
                    stopMove = true;
                    Debug.Log("Berhasil");
                }
                else
                {
                    stopMove = true;
                    ship.position = nodes[starterNodeIndex].position;
                    manager.ClearResources();
                    Debug.Log("Gagal");
                }
                
                distancePercentage = 0;


            }

        }
        
    }

    private void findCorrespondKnots()
    {
        // Populate node neighbour splines and node of knots
        for (int nodeIndex = 0; nodeIndex < nodes.Length; nodeIndex++)
        {
            Vector3 splineNodePos = worldToSplinePosition(nodes[nodeIndex].position);
            for (global::System.Int32 i = 0; i < splines.Splines.Count(); i++)
            {
                List<BezierKnot> knots = splines.Splines[i].Knots.ToList();
                for (global::System.Int32 j = 0; j < knots.Count(); j++)
                {

                    if (knots.ElementAt(j).Position.Equals(splineNodePos))
                    {
                        // Add corresponds node of knots
                        nodeOfKnot.Add(new SplineKnotIndex(i, j), nodeIndex);

                        // Find coresponds node neighbour   
                        if (!nodeNeighbourSplines.ContainsKey(nodeIndex))
                        {
                            List<SplineKnotIndex> linked = splines.KnotLinkCollection.GetKnotLinks(new SplineKnotIndex(i, j)).ToList();
                            List<int> neighbourSplines = new List<int>();
                            linked.ForEach(knot => { neighbourSplines.Add(knot.Spline); });
                            nodeNeighbourSplines.Add(nodeIndex, neighbourSplines);

                        }
                    }
                }
            }
        }
    }
    private int getNextSpline(int nodeIndex)
    {
        List<int> movableSplines = nodeNeighbourSplines[nodeIndex];

        foreach (var index in movableSplines)
        {
            if (pathTypes[index] == availResources.ElementAt(0))
            {
                return index;
            }
        }
        return -1;
    }

    private void checkReverseSpline(int nodeIndex, int splineIndex)
    {
        SplineKnotIndex splineKnotIndex = new SplineKnotIndex(splineIndex, 1);
        if(nodeOfKnot[splineKnotIndex] == nodeIndex)
        {
            SplineUtility.ReverseFlow(splines, splineIndex);
            int reversedNode = nodeOfKnot[splineKnotIndex];
            nodeOfKnot[splineKnotIndex] = nodeOfKnot[new SplineKnotIndex(splineIndex, 0)];
            nodeOfKnot[new SplineKnotIndex(splineIndex, 0)] = reversedNode;
        }

    }

    private Vector3 worldToSplinePosition(Vector3 position)
    {
        return new Vector3(position.x - transform.position.x, position.y - transform.position.y, 0);
    }
    public void StartMove()
    {
        if(manager.Resource.Count != manager.MaxResource)
        {
            Debug.Log("Belum Semua Resource Terpakai");
            return;
        }
        resource = manager.Resource.ToArray();
        availResources = resource.ToList();
        currentSpline = getNextSpline(starterNodeIndex);
        if (currentSpline == -1)
        {
            stopMove = true;
        }
        checkReverseSpline(starterNodeIndex, currentSpline);
        stopMove = false;
    }

    private void InstatiatePathAsset()
    {
        for (int i = 0; i < splines.Splines.Count; i++)
        {
            float splineLength = splines.CalculateLength(i);
            int numAssets = (int) Math.Round(splineLength);
            float incPercentage = 1 / splineLength;
            GameObject assetToUse = new GameObject();
            for (global::System.Int32 j = 0; j < pathAssets.Length; j++)
            {
                if (pathAssets[j].color == pathTypes[i])
                {
                    assetToUse = pathAssets[j].pathAssets;
                    break;
                }
            }
            float distancePercentage = 0;
            distancePercentage += incPercentage;
            for (global::System.Int32 k = 0; k < numAssets - 1; k++)
            {
                Vector3 newPos = splines.EvaluatePosition(splines.Splines[i], distancePercentage);
                Instantiate(assetToUse, newPos, Quaternion.identity);
                distancePercentage += incPercentage;
            }
        }
    }
    
}
