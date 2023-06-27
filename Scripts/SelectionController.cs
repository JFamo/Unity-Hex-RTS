using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionController : MonoBehaviour
{
    public HexGridLayout mainGrid;
    public GridController mainGridController;

    private int start_x, start_y, end_x, end_y;
    private bool dragging;
    private List<BoardObject> selectedObjects;

    void Start(){
        selectedObjects = new List<BoardObject>();
        dragging = false;
    }

    private Vector2Int CalculateMousePosition(){
        Ray myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit myHit;

        if(Physics.Raycast(myRay, out myHit)){
            Vector2Int hexComputed = mainGrid.GetCoordinateFromGamePosition(myHit.point);
            return hexComputed;
        }
        return new Vector2Int(-1, -1); // Trigger validation error on no hit
    }

    private void LogSelectionIdentities(){
        foreach(BoardObject someObj in selectedObjects){
            Debug.Log("SELECT: " + someObj.identity);
        }
    }

    private void ResetSelection(){
        foreach(BoardObject selObj in selectedObjects){
            selObj.NotifyDeselected();
        }
        selectedObjects = new List<BoardObject>();
    }

    private void SelectCursorObjects(){
        ResetSelection();
        Vector2Int cursorPosition = CalculateMousePosition();
        if(mainGrid.ValidatePointInGameGrid(cursorPosition)){
            selectedObjects.Add(mainGridController.GetObjectAtPosition(cursorPosition));
        }
    }

    private void SelectBoxedObjects(){
        ResetSelection();
        for(int x = start_x; x <= end_x; x ++){
            for(int y = start_y; y <= end_y; y ++){
                BoardObject thisObj = mainGridController.GetObjectAtPosition(x,y);
                if(thisObj.identity != GridObject.NONE && thisObj.IsSelectable()){
                    selectedObjects.Add(thisObj);
                    thisObj.NotifySelected();
                }
            }
        }

        LogSelectionIdentities();
    }

    private void HandleMouseDown(){
        Vector2Int start_posn = CalculateMousePosition();
        if(mainGrid.ValidatePointInGameGrid(start_posn)){
            dragging = true;
            start_x = start_posn.x;
            start_y = start_posn.y;
        }
    }

    private void HandleMouseUp(){
        if(dragging){
            Vector2Int end_posn = CalculateMousePosition();
            if(mainGrid.ValidatePointInGameGrid(end_posn)){
                dragging = false;
                end_x = end_posn.x;
                end_y = end_posn.y;
            }

            SelectBoxedObjects();
        }
    }

    private void HandleRMBDown(){
        //Debug.Log("RMB Down"); // DEBUG
        Vector2Int target_posn = CalculateMousePosition();
        if(mainGrid.ValidatePointInGameGrid(target_posn)){
            //Debug.Log("RMB Down at valid " + target_posn.x + ", " + target_posn.y); // DEBUG
            foreach(BoardObject selectedObject in selectedObjects){
                //Debug.Log("Selected obj " + selectedObject.identity); // DEBUG
                if(selectedObject.IsMovable()){
                    //Debug.Log("Selected movable obj " + selectedObject.identity); // DEBUG
                    selectedObject.NotifyMoveOrder(target_posn);
                }
            }
        }
    }

    void Update(){
        if(Input.GetMouseButtonDown(0)){
            HandleMouseDown();
        }
        if(Input.GetMouseButtonUp(0)){
            HandleMouseUp();
        }
        if(Input.GetMouseButtonDown(1)){
            HandleRMBDown();
        }
    }
}
