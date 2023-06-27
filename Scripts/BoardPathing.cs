using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardPathing : MonoBehaviour
{
    // Reference to hex grid
    public HexGridLayout mainGrid;
    public GridController mainGridController;

    // Map of text object to a single position
    private Dictionary<Vector2Int, GameObject> textObjects = new Dictionary<Vector2Int, GameObject>();

    // Struct to store some node in calculated pathing history
    public struct PathingNode{
        public Vector2Int posn;
        public Vector2Int[] ancestors;
        public float fValue;
        public float gValue;

        public PathingNode(Vector2Int posn, Vector2Int[] ancestors, float gValue, float fValue){
            this.posn = posn;
            this.ancestors = ancestors;
            this.gValue = gValue;
            this.fValue = fValue;
        }
    }
    
    // Comparator for Pathing Nodes
    public class ByFValueComparator : IComparer<PathingNode>
    {
        public int Compare(PathingNode a, PathingNode b){
            // Return f value difference
            int fValueDifference = (int)a.fValue - (int)b.fValue;
            // Tie-break for different positions since we need unique values
            if(fValueDifference != 0){
                return fValueDifference;
            }
            else{
                int xValueDifference = a.posn.x - b.posn.x;
                int yValueDifference = a.posn.y - b.posn.y;
                if(xValueDifference != 0){
                    return xValueDifference;
                }
                if(yValueDifference != 0){
                    return yValueDifference;
                }
                return 0;
            }
        }
    }

    // Function to generate successors to some position
    private List<Vector2Int> SuccessorsFromPosition(Vector2Int pos){

        List<Vector2Int> posns = mainGrid.SuccessorsFromPosition(pos);

        // DEBUG
        //foreach(Vector2Int posn in posns){
        //    Debug.Log("Object " + mainGridController.GetObjectAtPosition(posn).identity + " at " + posn.x + ", " + posn.y + " w/ movable " + !mainGridController.GetObjectAtPosition(posn).IsSolid());
        //}

        // Ignore unmoveable
        posns.RemoveAll(thisPos => mainGridController.GetObjectAtPosition(thisPos).IsSolid());

        // DEBUG
        //foreach(Vector2Int posn in posns){
        //    Debug.Log("Added valid successor at " + posn.x + ", " + posn.y);
        //}

        return posns;
    }

    // Function to join arrays
    private Vector2Int[] JoinArrays(Vector2Int[] a, Vector2Int[] b){
        Vector2Int[] outArray = new Vector2Int[a.Length + b.Length];
        a.CopyTo(outArray, 0);
        b.CopyTo(outArray, a.Length);
        return outArray;
    }

    // Function to join value to array
    private Vector2Int[] AppendValueToArray(Vector2Int[] a, Vector2Int b){
        Vector2Int[] outArray = new Vector2Int[a.Length + 1];
        a.CopyTo(outArray, 0);
        outArray[a.Length] = b;
        return outArray;
    }

    // Function to compute diagonal distance in hex
    private float DiagonalDistance(Vector2Int a, Vector2Int b){
        // TODO upgrade to hex dist
        return Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y));
    }

    // Function to check min F of position in set
    private float MinimumDistanceOfPositionInSet(SortedSet<PathingNode> set, Vector2Int pos){
        float minDistance = float.MaxValue;
        foreach(var node in set){
            if(node.posn == pos){
                if(node.fValue < minDistance){
                    minDistance = node.fValue;
                }
            }
        }
        return minDistance;
    }

    // Function to show distance text at successor
    public void DrawSuccessorText(Vector2Int posn, float distance){

        // Delete old if exists
        if(textObjects.ContainsKey(posn)){
            Destroy(textObjects[posn]);
        }

        // Create text object and mesh
        var textObj = new GameObject();
        var textMesh = textObj.AddComponent<TextMesh>();

        // Set text value as distance
        textMesh.text = "" + distance;
        textMesh.fontSize = 6;

        // Set position of text from coord
        Vector3 realPosn = mainGrid.GetPositionForHexFromCoord(posn);
        textObj.transform.position = realPosn;

        // Add to dictionary
        textObjects[posn] = textObj;
    }

    // Function to execute an A* Pathing search
    public Vector2Int[] AStarPath(Vector2Int start, Vector2Int dest){

        // DEBUG
        //Debug.Log("Pathing from " + start.x + ", " + start.y + " to " + dest.x + ", " + dest.y);

        // Check for same position and exit
        if(start == dest){
            return new Vector2Int[1]{start};
        }

        // Check for solid end and abort
        if(mainGridController.GetObjectAtPosition(dest).IsSolid()){
            return new Vector2Int[0];
        }

        // Init open and closed lists as sorted sets with comparator on F
        SortedSet<PathingNode> openList = new SortedSet<PathingNode>(new ByFValueComparator());
        SortedSet<PathingNode> closedList = new SortedSet<PathingNode>(new ByFValueComparator());

        // Add start to open
        PathingNode first = new PathingNode(start, new Vector2Int[1], 0f, 0f);
        first.ancestors[0] = start;
        openList.Add(first);

        // Check for open list length
        while(openList.Count > 0){
            // Pop min
            PathingNode next = openList.Min;
            openList.Remove(next);

            /// DEBUG
            //Debug.Log("Evaluating open list at " + next.posn.x + ", " + next.posn.y + " with distance " + next.fValue);

            // Generate successors
            List<Vector2Int> successors = SuccessorsFromPosition(next.posn);
            foreach(Vector2Int successor in successors){

                // DEBUG
                //Debug.Log("Evaluating successor at " + successor.x + ", " + successor.y);

                // Check finished and return
                if(successor == dest){
                    return AppendValueToArray(next.ancestors, successor);
                }

                // Compute distances
                float succG = next.gValue + 1.0f;
                float succH = DiagonalDistance(successor, dest);
                float succF = succG + succH;

                // Check on open list
                if(succF > MinimumDistanceOfPositionInSet(openList, successor)){
                    // nothing
                }
                // Check on closed list
                else if(succF > MinimumDistanceOfPositionInSet(closedList, successor)){
                    // nothing
                }
                else{

                    // DEBUG draw text
                    //DrawSuccessorText(successor, succF);

                    // Add successor to open list
                    var story = openList.Add(new PathingNode(successor, AppendValueToArray(next.ancestors, successor), succG, succF));

                    // DEBUG
                    //Debug.Log("Added successor at " + successor.x + ", " + successor.y + " with distance " + succF );

                }
            }

            // Add next to closed list
            closedList.Add(next);

        }

        return new Vector2Int[0];
    }
}
