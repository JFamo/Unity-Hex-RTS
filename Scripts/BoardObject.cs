using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardObject : MonoBehaviour
{
    public GridObject identity;
    public ObjectProperties[] properties;

    private GridController mainGridController;
    private HexGridLayout mainGrid;
    private GameObject groundCanvas;
    private GameObject progressCanvas;

    public float speed = 0.0f;

    private bool isSolid = false;
    private bool isMovable = false;
    private bool isSelectable = false;
    private bool selected = false;
    private Vector2Int myPosition;

    private MoverController moverChild;
    private AttackController attackerChild;
    private HealthController healthChild;

    public void InitializeBoardObject(GridController gc, HexGridLayout hgl, GameObject groundCanvasPrefab, GameObject progressCanvasPrefab){
        mainGridController = gc;
        mainGrid = hgl;
        groundCanvas = Instantiate(groundCanvasPrefab, gameObject.transform);

        myPosition = new Vector2Int(-1,-1);

        /* Initialize Unit Canvas Controller */
        UnitCanvasController unitCanvasController = GetComponent<UnitCanvasController>();
        if(unitCanvasController != null){
            progressCanvas = Instantiate(progressCanvasPrefab, gameObject.transform);
            unitCanvasController.InitializeUnitCanvasController(progressCanvas);
        }

        /* Initialize Pointer Controller */
        PointerController pointerController = GetComponent<PointerController>();
        if(pointerController != null){
            pointerController.InitializePointerController(mainGrid);
        }

        /* Initialize and Save Mover Controller */
        moverChild = GetComponent<MoverController>();
        if(moverChild != null){
            moverChild.InitializeMover(this, mainGridController, mainGrid, pointerController, unitCanvasController);
        }

        /* Initialize Spawner Controller */
        SpawnerController spawnerChild = GetComponent<SpawnerController>();
        if(spawnerChild != null){
            spawnerChild.InitializeSpawner(this, mainGridController, mainGrid);
        }

        /* Initialize Attack Controller */
        attackerChild = GetComponent<AttackController>();
        if(attackerChild != null){
            attackerChild.InitializeAttackController(this, mainGridController, mainGrid, pointerController, unitCanvasController);
        }

        /* Initialize Health Controller */
        healthChild = GetComponent<HealthController>();
        if(healthChild != null){
            healthChild.InitializeHealthController(this, unitCanvasController);
        }

        selected = false;
        RecomputeProperties();
    }

    private void RecomputeProperties(){
        isSolid = false;
        isSelectable = false;
        isMovable = false;
        for(int i = 0; i < properties.Length; i ++){
            if(properties[i] == ObjectProperties.SOLID){
                isSolid = true;
            }
            if(properties[i] == ObjectProperties.SELECTABLE){
                isSelectable = true;
            }
            if(properties[i] == ObjectProperties.MOVABLE){
                isMovable = true;
            }
        }
    }

    public bool IsSolid(){
        return isSolid;
    }

    public bool IsSelectable(){
        return isSelectable;
    }

    public bool IsMovable(){
        return isMovable;
    }

    public bool HasProperty(ObjectProperties prop){
        for(int i = 0; i < properties.Length; i ++){
            if(properties[i] == prop){
                return true;
            }
        }
        return false;
    }

    public Vector2Int GetPosition(){
        return myPosition;
    }

    public void NotifyPositionUpdate(Vector2Int newPosn){
        myPosition = newPosn;
    }

    public bool RequestMoveToPosition(Vector2Int posn){
        if(mainGridController.RequestMoveToPosition(this, posn)){
            myPosition = posn;
            Vector3 gamePosn = mainGridController.GetTransformForPosition(posn);
            transform.position = new Vector3(gamePosn.x, transform.position.y, gamePosn.z);

            return true;
        }
        else{
            return false;
        }
    }

    private void UpdateSelectionImage(){
        if(groundCanvas){
            Image selectedImage = groundCanvas.GetComponentInChildren<Image>();
            if(selectedImage){
                selectedImage.enabled = selected;
            }
        }
    }

    public void NotifySelected(){
        selected = true;
        UpdateSelectionImage();
    }

    public void NotifyDeselected(){
        selected = false;
        UpdateSelectionImage();
    }

    public void NotifyMoveOrder(Vector2Int target){
        if(IsMovable()){
            moverChild.MoveToPoint(target);
        }
        if(attackerChild != null){
            attackerChild.SuspendEngage();
        }
    }

    public void NotifyArrived(){
        if(attackerChild != null){
            attackerChild.TriggerEngage();
        }
    }

    public void NotifyDead(){
        mainGridController.NotifyDestruction(this);
        Destroy(gameObject);
    }

    public void NotifyDamageIncoming(float amount){
        healthChild.NotifyTakeDamage(amount);
        if(attackerChild != null){
            attackerChild.TriggerEngage();
        }
    }
}
