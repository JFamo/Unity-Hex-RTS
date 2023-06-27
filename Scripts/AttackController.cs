using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    public GameObject[] attackEffects;

    public float damageRate = 2.0f;
    public float critChance = 0.1f;
    public float attackRate = 3.0f;
    public float critMultiplier = 2.0f;

    private BoardObject myBoardObject;
    private GridController myGridController;
    private HexGridLayout myGridLayout;
    private PointerController pointerController;
    private UnitCanvasController unitCanvasController;

    private bool engaged = false;
    private float counter;
    private BoardObject target;
    private Vector2Int targetedPosition;

    public void InitializeAttackController(BoardObject boardObject, GridController gridController, HexGridLayout gridLayout, PointerController pointerController, UnitCanvasController unitCanvasController){
        this.myBoardObject = boardObject;
        this.myGridController = gridController;
        this.myGridLayout = gridLayout;
        this.pointerController = pointerController;
        this.unitCanvasController = unitCanvasController;

        this.engaged = false;
        this.counter = 0.0f;
        ToggleAttackEffects(false);
    }   

    private Vector2Int FindEnemyPosition(){
        ObjectProperties opp = myBoardObject.HasProperty(ObjectProperties.FRIENDLY) ? ObjectProperties.HOSTILE : ObjectProperties.FRIENDLY;
        foreach(Vector2Int successor in myGridLayout.SuccessorsFromPosition(myBoardObject.GetPosition())){
            if(myGridController.GetObjectAtPosition(successor).HasProperty(opp)){
                return successor;
            }
        }
        return new Vector2Int(-1,-1);
    }

    private bool TargetNearestEnemy(){
        Vector2Int nearestOpp = FindEnemyPosition();
        if(nearestOpp.x >= 0 && nearestOpp.y >= 0){
            pointerController.AttemptPointToCoord(nearestOpp);
            target = myGridController.GetObjectAtPosition(nearestOpp);
            targetedPosition = nearestOpp;
            return true;
        }
        target = null;
        targetedPosition = new Vector2Int(-1,-1);
        return false;
    }

    private void ToggleAttackEffects(bool value){
        for(int i = 0; i < attackEffects.Length; i ++ ){
            attackEffects[i].SetActive(value);
        }
        unitCanvasController.ToggleProgressBar(UnitCanvasElement.ATTACK, value);
    }

    public bool GetEngaged(){ // I do
        return this.engaged;
    }

    public void SuspendEngage(){
        engaged = false;
        target = null;
        targetedPosition = new Vector2Int(-1,-1);
        ToggleAttackEffects(false);
    }

    public void TriggerEngage(){
        if(!engaged){
            if(TargetNearestEnemy()){
                engaged = true;
                counter = 0.0f;
                ToggleAttackEffects(true);
            }
        }
    }

    public void TargetExplicitPosition(Vector2Int posn){
        pointerController.AttemptPointToCoord(posn);
        target = myGridController.GetObjectAtPosition(posn);
        targetedPosition = posn;
        engaged = true;
        ToggleAttackEffects(true);
    }

    private void ReevaluateEngage(){
        if(!TargetNearestEnemy()){
            SuspendEngage();
        }
    }

    private bool RollForCritical(){
        if(Random.Range(0, 100) < critChance * 100){
            return true;
        }
        return false;
    }

    private void ExecuteAttack(){
        // Check target has not died
        if(target != null){
            
            // Check target has not fled
            if(target.GetPosition() == targetedPosition){
                float dmg = this.damageRate;

                // Attempt critical hit
                if(RollForCritical()){
                    dmg = dmg * this.critMultiplier;
                }

                // Notify enemy HealthController via enemy BoardObject
                target.NotifyDamageIncoming(dmg);
            }
            else{
                ReevaluateEngage();
            }
        }
        else{
            ReevaluateEngage();
        }
    }

    void Update(){
        if(engaged){
            counter += Time.deltaTime;
            unitCanvasController.SetProgressBar(UnitCanvasElement.ATTACK, counter / attackRate);
            if(counter >= attackRate){
                ExecuteAttack();
                counter = 0.0f;
            }
        }
    }
}
