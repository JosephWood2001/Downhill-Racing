using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Path : MonoBehaviour
{

    Dictionary<int,int> choices = new Dictionary<int, int>();
    public int seedMax;

    public void DebugStart()
    {
        Start();
    }

    private void Start()
    {
        BakeCrudeDistances();

        int uniqueDivider = 1;
        foreach (BezierCurve curve in curves)
        {
            if (curve.alive)
            {
                if(curve.Node2.EgressCurves.Count > 1)
                {
                    choices.Add(curves.IndexOf(curve), uniqueDivider);
                    uniqueDivider *= curve.Node2.EgressCurves.Count;
                }
            }
        }
        seedMax = uniqueDivider - 1;
    }

    public int drawQuality = 10;
    public int lengthQuality = 100;
    //things for editor
    [SerializeField]
    private int selectedId;
    public Node Selected
    {
        get
        {
            if (selectedId >= 0 && selectedId < nodes.Count)
            {
                return nodes[selectedId];
            }
            else
            {
                return null;
            }
        }
        set
        {
            selectedId = nodes.IndexOf(value);
        }
    }

    [SerializeField]
    private int mergeSelectedId = -1;
    public Node MergeSelected
    {
        get
        {
            if (mergeSelectedId >= 0 && mergeSelectedId < nodes.Count)
            {
                return nodes[mergeSelectedId];
            }
            else
            {
                return null;
            }
            
        }
        set
        {
            mergeSelectedId = nodes.IndexOf(value);
        }
    }

    [SerializeField]
    public Vector3 cursor;

    
    [SerializeField]
    public int selectedCurveId;
    public BezierCurve SelectedCurve
    {
        get
        {
            if(selectedCurveId >= 0 && selectedCurveId < curves.Count)
            {
                return curves[selectedCurveId];
            }
            else
            {
                return null;
            }
                
        }

        set
        {
            selectedCurveId = curves.IndexOf(value);
        }
    }

    public void SelectNextNode()
    {
        if (Selected.EgressCurves.IndexOf(SelectedCurve) != -1)
        {
            Selected = SelectedCurve.Node2;
            return;
        }

        if (Selected.IngressCurves.IndexOf(SelectedCurve) != -1)
        {
            Selected = SelectedCurve.Node1;
            return;
        }

        //shouldn't make it here, but if it does...
        Debug.Log("oops (selectnextnode)");
        selectedId = 0;
    }

    
    public void SelectNextCurve()
    {
        List<BezierCurve> queue = new List<BezierCurve>();
        foreach (BezierCurve curve in Selected.EgressCurves)
        {
            if (curve != null)
                if(curve.alive)
            {
                    queue.Add(curve);
            }
        }
        foreach (BezierCurve curve in Selected.IngressCurves)
        {
            if (curve != null)
                if (curve.alive)
                {
                    queue.Add(curve);
                }
        }
        int index = queue.IndexOf(SelectedCurve) + 1;
        if (index >= queue.Count)
        {
            SelectedCurve = queue[0];
        }
        else
        {
            SelectedCurve = queue[index];
        }
    }
    //-------


    [Serializable]
    public class Node
    {

        public float crudeDistance = -1;

        public bool alive;
        public Node(Vector3 location, Path path)
        {
            alive = true;
            this.path = path;
            this.location = location;
            ingressCurvesId = new List<int>();
            egressCurvesId = new List<int>();
        }
        [SerializeField]
        Vector3 location;
        public Vector3 Location
        {
            get
            {
                return location;
            }

            set
            {
                location = value;
            }
        }

        [SerializeField]
        [HideInInspector]
        private Path path;

        //a list of all the curves entering this node
        [SerializeField]
        private List<int> ingressCurvesId = new List<int>();
        public List<BezierCurve> IngressCurves
        {
            get
            {
                List<BezierCurve> list = new List<BezierCurve>();
                foreach(int id in ingressCurvesId)
                {
                    list.Add(path.curves[id]);
                }
                return list;
            }
        }

        public void RemoveIngress(BezierCurve curve)
        {
            ingressCurvesId.Remove(path.curves.IndexOf(curve));
        }
        public void AddIngressId(int id)
        {
            ingressCurvesId.Add(id);
        }
        public void RemoveIngressId(int id)
        {
            ingressCurvesId.Remove(id);
        }
        //a list of all the curves leaving this node
        [SerializeField]
        private List<int> egressCurvesId = new List<int>();
        public List<BezierCurve> EgressCurves
        {
            get
            {
                List<BezierCurve> list = new List<BezierCurve>();
                foreach (int id in egressCurvesId)
                {
                    list.Add(path.curves[id]);
                }
                return list;
            }
        }

        public void RemoveEgress(BezierCurve curve)
        {
            egressCurvesId.Remove(path.curves.IndexOf(curve));
        }

        public void AddEgressId(int id)
        {
            egressCurvesId.Add(id);
        }
        public void RemoveEgressId(int id)
        {
            egressCurvesId.Remove(id);
        }
    }
    [Serializable]
    public class BezierCurve
    {

        [SerializeField]
        public Path path;

        public bool alive;
        public BezierCurve(Node start, Vector3 startAnchor, Node end, Vector3 endAnchor, Path path)
        {
            alive = true;
            this.path = path;
            Node1 = start;
            Node2 = end;
            Anchor1 = startAnchor;
            Anchor2 = endAnchor;

            RebakeLength();

        }

        private void RebakeLength()
        {
            length = 0f;
            //makes sure that lengthQuality is even
            if (path.lengthQuality % 2 == 1)
            {
                path.lengthQuality--;
            }

            length += TangentAtPoint(0).magnitude * (1f / path.lengthQuality);
            for (float i = 1; i < path.lengthQuality; i++) //TODO: change to simsons later for better approximation
            {
                if (i % 2 == 1)//if odd
                {
                    length += 2f * TangentAtPoint(((float)i) / ((float)path.lengthQuality)).magnitude * (1f / path.lengthQuality);
                }
                else
                {
                    length += 4f * TangentAtPoint(((float)i) / ((float)path.lengthQuality)).magnitude * (1f / path.lengthQuality);
                }
            }
            length += TangentAtPoint(1).magnitude * (1f / path.lengthQuality);
            length /= 3f;
        }

        //the start point of the curve
        [SerializeField]
        private int point1Id;
        public Vector3 Point1
        {
            get
            {
                return path.nodes[point1Id].Location;
            }
            set
            {
                path.nodes[point1Id].Location = value;
                RebakeLength();
            }
        }

        public Node Node1
        {
            get
            {
                return path.nodes[point1Id];
            }
            set
            {
                point1Id = path.nodes.IndexOf(value);
                RebakeLength();
            }
        }

        //the start anchor of the curve (the curve will start by heading to the anchor point)
        [SerializeField]
        private Vector3 anchor1;
        public Vector3 Anchor1
        {
            get
            {
                return anchor1;
            }
            set
            {
                anchor1 = value;
                RebakeLength();
            }
        }

        //the end point of the curve
        [SerializeField]
        private int point2Id;
        public Vector3 Point2
        {
            get
            {
                return path.nodes[point2Id].Location;
            }
            set
            {
                path.nodes[point2Id].Location = value;
                RebakeLength();
            }
        }

        public Node Node2
        {
            get
            {
                return path.nodes[point2Id];
            }
            set
            {
                point2Id = path.nodes.IndexOf(value);
                RebakeLength();
            }
        }

        //the end anchor of the curve (the curve will end by heading to the anchor point)
        [SerializeField]
        private Vector3 anchor2;
        public Vector3 Anchor2
        {
            get
            {
                return anchor2;
            }
            set
            {
                anchor2 = value;
                RebakeLength();
            }
        }

        //the length of the curve. This should be baked pre-runtime
        [SerializeField]
        float length;
        public float Length
        {
            get
            {
                return length;
            }
            set
            {
                length = value;
            }
        }

        /**
         * t: a dummy var for distance along the curve, 0 being the start, 1 being the end
         * returns the tangent of the curve at location t (in the form of a Vector3)
         */
        public Vector3 TangentAtPoint(float t)
        {
            return
                -3f * Mathf.Pow(1f - t, 2) * Point1
                + 3f * Mathf.Pow(1f - t, 2) * Anchor1
                - 6f * (1f - t) * t * Anchor1
                + 6f * (1f - t) * t * Anchor2
                - 3f * Mathf.Pow(t, 2) * Anchor2
                + 3 * Mathf.Pow(t, 2) * Point2;
        }

        /**
         * t: a dummy var for distance along the curve, 0 being the start, 1 being the end
         * returns the point on the curve at location t
         */
        public Vector3 Point(float t)
        {
            return
                Mathf.Pow((1f - t), 3) * Point1
                + 3f * Mathf.Pow((1f - t), 2) * t * Anchor1
                + 3f * (1f - t) * Mathf.Pow(t, 2) * Anchor2
                + Mathf.Pow((t), 3) * Point2;
        }

        /**
         * vector3: the point being projected onto the curve
         * quality: the search is within quality amount of meters
         * returns the point on the curve clostest to the point being projected
         */
        public Vector3 PointAtProjection(Vector3 vector3, float quality)
        {
            
            return Point(LocationAtProjection(vector3,quality));
        }
        public Vector3 PointAtProjection(Vector3 vector3)
        {

            return Point(LocationAtProjection(vector3, path.defaultSearchQuality));
        }

        /**
         * vector3: the point being projected onto the curve
         *quality: the search is within quality amount of meters
         * returns the dummy var t for where on the curve the point being projected
         */
        public float LocationAtProjection(Vector3 vector3, float quality)
        {
            return GoldenSectionSearch(vector3, 0f, 1f, (vector3 - Point(goldenRatio)).magnitude , (vector3 - Point(1f - goldenRatio)).magnitude , quality);
        }
        public float LocationAtProjection(Vector3 vector3)
        {
            return GoldenSectionSearch(vector3, 0f, 1f, (vector3 - Point(goldenRatio)).magnitude, (vector3 - Point(1f - goldenRatio)).magnitude, path.defaultSearchQuality);
        }


        /**recursive search that narrows a and b for a curtain number of times
         * 
         * point: the point whose distance to the curve is being minimised
         * a:the leftmost part being searched
         * b:the rightmost part being searched
         * x1: the distance at a + goldenRatio
         * x2: the distance at b - goldenRatio
         * quality: the search is within quality amount of meters
         * 
         * returns an estimate for the dummy var t for the point on the curve
         */
        [HideInInspector]
        [SerializeField]
        private float goldenRatio = (Mathf.Pow(5f, .5f) - 1f) / 2f;
        float newA;
        float newB;
        float newX1;
        float newX2;
        private float GoldenSectionSearch(Vector3 point, float a, float b, float x1, float x2, float quality)
        {

            if (x1 < x2)
            {
                newA = b - (b - a) * goldenRatio;
                newB = b;

                //the area being searched is smaller then quality
                if ((newB - newA) * length < quality)
                {
                    return (newA + newB) / 2f;
                }
                else
                {
                    newX1 = (point - Point(newA + (newB - newA) * goldenRatio)).magnitude;
                    newX2 = x1;
                    return GoldenSectionSearch(point, newA, newB , newX1, newX2, quality);
                }

            }
            else
            {
                newA = a;
                newB = a + (b - a) * goldenRatio;

                //the area being searched is smaller then quality
                if ((newB - newA) * length < quality)
                {
                    return (newA + newB) / 2f;
                }
                else
                {
                    newX1 = x2;
                    newX2 = (point - Point(newB - (newB - newA) * goldenRatio)).magnitude;
                    return GoldenSectionSearch(point, newA, newB, newX1, newX2, quality);
                }
            }
        }
    }

    
    [SerializeField]
    public int headId;
    public Node Head
    {
        get
        {
            return nodes[headId];
        }

        set
        {
            headId = nodes.IndexOf(value);
        }
    }
    [SerializeField]
    public List<Node> nodes;
    [SerializeField]
    public List<BezierCurve> curves;

    public float defaultSearchQuality = .01f;

    public void Initalize(Vector3 startLocation)
    {
        nodes = new List<Node>();
        curves = new List<BezierCurve>();
        Node head = new Node(startLocation, this);
        selectedCurveId = -1;
        selectedId = 0;
        mergeSelectedId = -1;
        nodes.Add(head);
        Head = head;
    }

    public BezierCurve AddSegment(Node start, Vector3 startAnchor, Vector3 endLocation, Vector3 endAnchor)
    {
        Node end = new Node(endLocation, this);
        nodes.Add(end);
        BezierCurve newCurve = new BezierCurve(start, startAnchor, end, endAnchor, this);
        curves.Add(newCurve);
        start.AddEgressId(curves.IndexOf(newCurve));
        end.AddIngressId(curves.IndexOf(newCurve));

        
        return newCurve;
    }

    public BezierCurve AddSegment(Node start, Vector3 startAnchor, Node end, Vector3 endAnchor)
    {
        BezierCurve newCurve = new BezierCurve(start, startAnchor, end, endAnchor, this);
        curves.Add(newCurve);
        start.AddEgressId(curves.IndexOf(newCurve));
        end.AddIngressId(curves.IndexOf(newCurve));


        return newCurve;
    }

    public void RemoveSegment(BezierCurve curve)
    {
        if (curve.Node2.IngressCurves.Count == 1)
        {
            foreach (BezierCurve egressCurve in curve.Node2.EgressCurves)
            {
                egressCurve.Node1 = curve.Node1;
                curve.Node1.AddEgressId(curves.IndexOf(egressCurve));
            }

            nodes[nodes.IndexOf(curve.Node2)] = null;
            curves[curves.IndexOf(curve)] = null;
            return;
        }
        else
        {
            curve.Node1.RemoveEgress(curve);
            curve.Node2.RemoveIngress(curve);

            curves[curves.IndexOf(curve)] = null;
            return;
        }
        
    }

    public Vector3 ClosestPoint(Vector3 vector)
    {
        Vector3 best = Vector3.positiveInfinity;
        Vector3 temp;
        foreach (BezierCurve curve in curves)
        {
            if (curve.alive)
            {
                temp = curve.PointAtProjection(vector, defaultSearchQuality);
                if (Vector3.Distance(temp, vector) < Vector3.Distance(best, vector))
                {
                    best = temp;
                }
            }
            
        }

        return best;
    }
    
    public void BakeCrudeDistances()
    {
        foreach(Node node in nodes)
        {
            if (node.alive)
            {
                node.crudeDistance = -1;
            }
            
        }
        Head.crudeDistance = 0;

        CrudeRec(Head, 0, 0);
    }

    //helper method for BakeCrudeDistances
    private void CrudeRec(Node node, int depth, float botVal)
    {
        if (node.EgressCurves.Count == 0)//is the end of the road
        {
            node.crudeDistance = 1;
            return;
        }

        foreach(BezierCurve curve in node.EgressCurves)
        {
            if (curve.Node2.crudeDistance >= 0)
            {
                node.crudeDistance = botVal + (curve.Node2.crudeDistance - botVal) * ((float)(depth)) / ((float)depth + 1f);
                botVal = node.crudeDistance;
                depth = 0;
            }
            else
            {
                CrudeRec(curve.Node2, depth + 1, botVal);
                node.crudeDistance = botVal + (curve.Node2.crudeDistance - botVal) * ((float)(depth)) / ((float)depth + 1f);
                botVal = node.crudeDistance;
                depth = 0;
            }
        }
    }

    //is a crude number, should only be compared against itself i.e. not used for any kind of unit distance traveled
    public float DistanceAtClostedPoint(Vector3 vector)
    {
        Vector3 best = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
        float distAtBest = -1f ;
        Vector3 temp;
        foreach (BezierCurve curve in curves)
        {
            temp = curve.PointAtProjection(vector, defaultSearchQuality);
            if (Vector3.Distance(temp, vector) < Vector3.Distance(best, vector))
            {
                best = temp;
                distAtBest = Mathf.Lerp(curve.Node1.crudeDistance, curve.Node2.crudeDistance, curve.LocationAtProjection(vector, defaultSearchQuality));
            }
        }

        return distAtBest;
    }

    //is a crude number, should only be compared against itself i.e. not used for any kind of unit distance traveled
    public float DistanceAtPathPoint(float pathPoint)
    {
        return Mathf.Lerp(curves[Mathf.FloorToInt(pathPoint)].Node1.crudeDistance, curves[Mathf.FloorToInt(pathPoint)].Node2.crudeDistance, pathPoint - (float)Mathf.FloorToInt(pathPoint));

    }

    public Vector3 TangentAtPathPoint(float pathPoint)
    {
        return curves[Mathf.FloorToInt(pathPoint)].TangentAtPoint(pathPoint - (float)Mathf.FloorToInt(pathPoint)).normalized;

    }

    public Vector3 PointAtPathPoint(float pathPoint)
    {
        return curves[Mathf.FloorToInt(pathPoint)].Point(pathPoint - (float)Mathf.FloorToInt(pathPoint));

    }

    public float PathPointAtPathPointPlusDistanceSeeded(float pathPoint, float distance, int seed, int quality)
    {
        int currentCurve = Mathf.FloorToInt(pathPoint);
        float t = pathPoint - (float)Mathf.FloorToInt(pathPoint);
        float deltaT = 0;
        for (int i = 0; i < quality; i++)
        {
            deltaT = (distance / ((float)quality)) / curves[currentCurve].TangentAtPoint(t).magnitude;
            t += deltaT;

            if (t > 1)//has surpassed current curve
            {
                t = t - 1;

                if (curves[currentCurve].Node2.EgressCurves.Count <= 1)
                {
                    currentCurve = curves.IndexOf(curves[currentCurve].Node2.EgressCurves[0]);
                }
                else //there is more then on egress curve
                {
                    int divider;
                    if(choices.TryGetValue(currentCurve, out divider))
                    {
                        currentCurve = curves.IndexOf(curves[currentCurve].Node2.EgressCurves[

                        (seed / divider) % curves[currentCurve].Node2.EgressCurves.Count

                        ]);
                    }
                    
                }


            }
        }

        return ((float) currentCurve) + t;

    }

    //a number corisponding to a unique point on the path
    public float PathPointAtClostedPoint(Vector3 vector)
    {
        Vector3 best = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
        Vector3 temp;

        int bestCurveId = -1;
        float locationAlongBest = 0;
        foreach (BezierCurve curve in curves)
        {
            if (curve.alive)
            {
                temp = curve.PointAtProjection(vector, defaultSearchQuality);
                if (Vector3.Distance(temp, vector) < Vector3.Distance(best, vector))
                {
                    bestCurveId = curves.IndexOf(curve);
                    best = temp;
                    locationAlongBest = curve.LocationAtProjection(vector, defaultSearchQuality);
                }
            }
            
        }

        return (float) bestCurveId + locationAlongBest;
    }

    private void OnDrawGizmosSelected()
    {
        
        if (nodes != null)
        {
            foreach (Node node in nodes)
            {
                if (node != null)
                    if(node.alive)
                {
                    Gizmos.color = Color.green;
                    if (MergeSelected != null)
                        if (MergeSelected.alive)
                            if (node == MergeSelected)
                                Gizmos.color = Color.yellow;
                    Gizmos.DrawSphere(node.Location, .7f);
                }
                
            }
        }

        if(curves != null)
        {
            foreach(BezierCurve curve in curves)
            {
                if (curve != null)
                    if(curve.alive)
                {
                    Gizmos.color = Color.green;
                    if (SelectedCurve != null)
                        if (SelectedCurve.alive)
                            if (curve == SelectedCurve)
                                Gizmos.color = Color.cyan;
                    for (int i = 0; i < drawQuality; i++)
                    {
                        Gizmos.DrawLine(curve.Point(((float)i) / ((float)drawQuality)), curve.Point(((float)(i + 1)) / ((float)drawQuality)));

                    }
                }
                
            }

        }
    }



    public int AIQuality = 20;
    public void GetAIData(Vector3 point, Vector3 pointRight, float farDist, float longDist, int seed ,out bool right, out float distanceToPoint, out Vector3 vectorToLine, out float offAngleRightValue, out float offAngleRightFarValue, out float offAngleRightLongValue)
    {
        float pathPoint = PathPointAtClostedPoint(point);
        float pathPointFar = PathPointAtPathPointPlusDistanceSeeded(pathPoint, farDist, seed, AIQuality);
        float pathPointLong = PathPointAtPathPointPlusDistanceSeeded(pathPoint, longDist, seed, AIQuality);

        distanceToPoint = Vector3.Distance(PointAtPathPoint(pathPoint), point);

        vectorToLine = PointAtPathPoint(pathPoint) - point;
        right = (Vector3.Dot(pointRight, vectorToLine) > 0);

        offAngleRightValue = Vector3.Dot(TangentAtPathPoint(pathPoint), -pointRight);
        offAngleRightLongValue = Vector3.Dot(TangentAtPathPoint(pathPointFar), -pointRight);
        offAngleRightFarValue = Vector3.Dot(TangentAtPathPoint(pathPointLong), -pointRight);

    }

    public int testSeed = 17;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawSphere(ClosestPoint(cursor), 2f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere( PointAtPathPoint( PathPointAtPathPointPlusDistanceSeeded( PathPointAtClostedPoint(cursor), 10f, testSeed, AIQuality ) ), 2f);
        

        /*
        foreach(BezierCurve curve in curves)
        {
            if (curve.alive)
            {
                if (curve.Node2.EgressCurves.Count <= 1)
                {

                }
                else //there is more then on egress curve
                {
                    int divider;
                    if (choices.TryGetValue(curves.IndexOf(curve), out divider))
                    {
                        Vector3 middle = curve.Node2.EgressCurves[

                        (testSeed / divider) % curve.Node2.EgressCurves.Count

                        ].Point(.5f);
                        Gizmos.DrawSphere(middle, 2f);
                    }

                }
            }
            
        }*/
    }


}



/* KEEPING THIS HERE FOR NOW INCASE I WANT IT LATER
 * 
        **
         * point: the point being compaired to the curve
         * returns the minimum distance that this point could be to the curve
         *

public float MinDistance(Vector3 point)
{
    //checks if the target is between the 4 points
    if (IsBetween(point, Point1, Point2, Anchor1, Anchor2))
    {
        return 0;
    }
    List<Entry> distances = new List<Entry>();
    distances.Add(new Entry((Point1 - point).magnitude, Point1));
    distances.Add(new Entry((Point2 - point).magnitude, Point2));
    distances.Add(new Entry((Anchor1 - point).magnitude, Anchor1));
    distances.Add(new Entry((Anchor2 - point).magnitude, Anchor2));

    //List<Entry> smallestDistances = from pair in distances where pair < distances.M

}

/**
 * returns the shortest distance of target to the plane defined by point1 point2 and point3
 *
private float DistanceToPlane(Vector3 target, Vector3 point1, Vector3 point2, Vector3 point3)
{
    //checks if the target isn't between 1 2 and 3
    if (!IsBetween(target, point1, point2, point3))
    {
        float min = (point1 - target).magnitude;
        if ((point2 - target).magnitude < min)
        {
            min = (point2 - target).magnitude;
        }
        if ((point3 - target).magnitude < min)
        {
            min = (point3 - target).magnitude;
        }
        return min;
    }

    Vector3 v1 = point2 - point1;
    Vector3 v2 = point3 - point1;
    Vector3 normal = Vector3.Cross(v1, v2);
    float distance = Vector3.Dot(target - point1, normal.normalized);
    return distance;

}

/**returns true only if target is between points 1 2 and 3
 * 
 *
private bool IsBetween(Vector3 target, Vector3 point1, Vector3 point2, Vector3 point3)
{
    float dotProduct12 = Vector3.Dot(point2 - point1, target - point1);
    float dotProduct21 = Vector3.Dot(point1 - point2, target - point2);
    float dotProduct13 = Vector3.Dot(point3 - point1, target - point1);
    float dotProduct31 = Vector3.Dot(point1 - point3, target - point3);
    float dotProduct23 = Vector3.Dot(point3 - point2, target - point2);
    float dotProduct32 = Vector3.Dot(point2 - point3, target - point3);
    if (dotProduct12 > 0 && dotProduct21 > 0 && dotProduct13 > 0 && dotProduct31 > 0 && dotProduct23 > 0 && dotProduct32 > 0)
    {
        return true;
    }
    else
    {
        return false;
    }

}

/**returns true only if target is between points 1 2 3 and 4
 * 
 *
private bool IsBetween(Vector3 target, Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4)
{
    float dotProduct12 = Vector3.Dot(point2 - point1, target - point1);
    float dotProduct21 = Vector3.Dot(point1 - point2, target - point2);
    float dotProduct13 = Vector3.Dot(point3 - point1, target - point1);
    float dotProduct31 = Vector3.Dot(point1 - point3, target - point3);
    float dotProduct23 = Vector3.Dot(point3 - point2, target - point2);
    float dotProduct32 = Vector3.Dot(point2 - point3, target - point3);
    float dotProduct41 = Vector3.Dot(point1 - point4, target - point4);
    float dotProduct14 = Vector3.Dot(point4 - point1, target - point1);
    float dotProduct42 = Vector3.Dot(point2 - point4, target - point4);
    float dotProduct24 = Vector3.Dot(point4 - point2, target - point2);
    float dotProduct43 = Vector3.Dot(point3 - point4, target - point4);
    float dotProduct34 = Vector3.Dot(point4 - point3, target - point3);
    if (dotProduct12 > 0 && dotProduct21 > 0 && dotProduct13 > 0 && dotProduct31 > 0 && dotProduct23 > 0 && dotProduct32 > 0 &&
        dotProduct42 > 0 && dotProduct41 > 0 && dotProduct43 > 0 && dotProduct34 > 0 && dotProduct24 > 0 && dotProduct34 > 0)
    {
        return true;
    }
    else
    {
        return false;
    }

}

/**
 * point: the point being compaired to the curve
 * returns the maximum distance that this point could be to the curve
 *
float max;
public float MaxDistance(Vector3 point)
{
    max = (point - Point1).magnitude;
    if ((point - Point2).magnitude > max)
    {
        max = (point - Point2).magnitude;
    }
    if ((point - Anchor1).magnitude > max)
    {
        max = (point - Anchor1).magnitude;
    }
    if ((point - Anchor2).magnitude > max)
    {
        max = (point - Anchor2).magnitude;
    }

    return max;
}
*/