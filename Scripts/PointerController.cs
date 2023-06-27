using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerController : MonoBehaviour
{
    public Transform pointerTransform;

    private HexGridLayout mainGrid;

    public void InitializePointerController(HexGridLayout mainGrid){
        this.mainGrid = mainGrid;
    }

    private void LookAtPoint(Vector3 point){
        pointerTransform.LookAt(new Vector3(point.x, pointerTransform.position.y, point.z));
    }

    public void AttemptPointToCoord(Vector2Int coord){
        Vector3 nextWorldPosn = mainGrid.GetPositionForHexFromCoord(coord);
        LookAtPoint(nextWorldPosn);
    }
    
}
