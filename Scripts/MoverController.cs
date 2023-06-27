using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverController : MonoBehaviour
{
    private GridController mainGridController;
    private HexGridLayout mainGrid;

    private BoardPathing myPathing;
    private BoardObject myObject;
    private List<Vector2Int> queue;

    private float counter = 0.0f;
    private int consecutiveFailures = 0;
    private int failureThreshold = 3;
    private bool isMoving;

    private PointerController pointerController;
    private UnitCanvasController unitCanvasController;

    void Start()
    {
        queue = new List<Vector2Int>();
    }

    public void InitializeMover(BoardObject myBoardObject, GridController gc, HexGridLayout hgl, PointerController pointerController, UnitCanvasController unitCanvasController){
        queue = new List<Vector2Int>();

        this.mainGridController = gc;
        this.mainGrid = hgl;

        myPathing = GetComponent<BoardPathing>();
        myPathing.mainGridController = mainGridController;
        myPathing.mainGrid = mainGrid;

        this.pointerController = pointerController;
        this.unitCanvasController = unitCanvasController;

        this.myObject = myBoardObject;

        isMoving = false;
        unitCanvasController.ToggleProgressBar(UnitCanvasElement.MOVEMENT, false);
    }

    public void MoveToPoint(Vector2Int point){
        queue.Clear();
        consecutiveFailures = 0;
        isMoving = true;

        pointerController.AttemptPointToCoord(point);

        // Execute AStar pathing
        Vector2Int[] path = myPathing.AStarPath(myObject.GetPosition(), point);
        foreach(Vector2Int step in path){
            // Ignore the first step. It will always be our start and occupied by a solid
            if(step != myObject.GetPosition()){
                queue.Add(step);
                Debug.Log("Pathing to " + step);
            }
        }

        AttemptPointToNext();
    }

    private void AttemptPointToNext(){
        if(pointerController != null){
            if(queue.Count > 0){
                pointerController.AttemptPointToCoord(queue[0]);
            }
        }
    }

    private void CancelPathing(){
        queue.Clear();
        unitCanvasController.ToggleProgressBar(UnitCanvasElement.MOVEMENT, false);
        myObject.NotifyArrived();
    }

    private void MoveToNext(){
        if(queue.Count > 0){
            unitCanvasController.ToggleProgressBar(UnitCanvasElement.MOVEMENT, true);
            Vector2Int nextPosn = queue[0];
            if(!mainGridController.GetObjectAtPosition(nextPosn).IsSolid()){
                myObject.RequestMoveToPosition(new Vector2Int(nextPosn.x, nextPosn.y));
                queue.RemoveAt(0);
                consecutiveFailures = 0;
                AttemptPointToNext();
            }
            else{
                consecutiveFailures += 1;
                if(consecutiveFailures >= failureThreshold){
                    CancelPathing();
                }
            }
        }
        else{
            if(isMoving){
                myObject.NotifyArrived();
                isMoving = false;
            }
            unitCanvasController.ToggleProgressBar(UnitCanvasElement.MOVEMENT, false);
        }
    }

    void Update()
    {
        // Only step after init
        if(myObject != null){
            // Time increments and events
            counter += Time.deltaTime * myObject.speed;
            unitCanvasController.SetProgressBar(UnitCanvasElement.MOVEMENT, counter / 1.0f);
            if(counter > 1.0f){
                counter = 0.0f;
                MoveToNext();
            }
        }
    }
}
