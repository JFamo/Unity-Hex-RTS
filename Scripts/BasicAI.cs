using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAI : MonoBehaviour
{
    public float thinkRate = 20.0f;
    public int defendTriggerDistance = 5;

    private List<BoardObject> minions;
    private BoardObject objective;
    private BoardObject defense;
    private ObjectProperties oppProperty = ObjectProperties.FRIENDLY;
    
    private HexGridLayout mainGridLayout;
    private GridController mainGridController;
    private List<Vector2Int> defendPoints;

    private float counter = 0.0f;

    public void InitializeBasicAI(GridController mainGridController, HexGridLayout mainGridLayout){
        counter = 0.0f;

        this.mainGridLayout = mainGridLayout;
        this.mainGridController = mainGridController;

        minions = new List<BoardObject>();
        defendPoints = new List<Vector2Int>();
    }

    public void NotifySpawn(BoardObject newSpawn){
        RegisterMinion(newSpawn);
    }

    public void RegisterMinion(BoardObject minion){
        minions.Add(minion);

        // Check for spawner and subscribe
        if(minion.HasSpawner()){
            minion.GetSpawner().RegisterSpawnSubscriber(this);
        }
    }

    public void RegisterDefense(BoardObject defense){
        this.defense = defense;
        RegisterMinion(defense);

        if(defense != null){
            this.defendPoints = mainGridLayout.GetCircleAroundPoint(defense.GetPosition(), defendTriggerDistance);
        }
    }

    public void RegisterObjective(BoardObject objective){
        this.objective = objective;
    }

    // Function to compute diagonal distance in hex
    private float DiagonalDistance(Vector2Int a, Vector2Int b){
        return Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y));
    }

    private Vector2Int CheckDefend(){
        if(defense != null){
            foreach(Vector2Int posn in defendPoints){
                if(mainGridController.GetObjectAtPosition(posn).HasProperty(oppProperty)){
                    return posn;
                }
            }
        }
        return new Vector2Int(-1,-1);
    }

    private void OrderMinionsToPosition(Vector2Int posn){
        foreach(BoardObject minion in minions){
            if(minion != null){
                minion.RequestScriptedPathToPosition(posn);
            }
        }
    }

    private void ThinkStep(){
        Vector2Int defendPosition = CheckDefend();

        if(defendPosition.x < 0 && defendPosition.y < 0){
            if(objective != null){
                OrderMinionsToPosition(objective.GetPosition());
            }
        }
        else{
            OrderMinionsToPosition(defendPosition);
        }
    }

    void Update(){
        counter += Time.deltaTime;
        if(counter >= thinkRate){
            counter = 0.0f;
            ThinkStep();
        }
    }
}
