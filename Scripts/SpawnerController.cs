using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerController : MonoBehaviour
{
    public GameObject spawn;
    public float frequency;

    private GridController mainGridController;
    private HexGridLayout mainGridLayout;
    private BoardObject myBoardObject;

    private float counter = 0.0f;

    private Vector2Int GetPosition(){
        if(myBoardObject != null){
            return myBoardObject.GetPosition();
        }
        else{
            Debug.LogError("Spawner does not have attached BoardObject!");
            return Vector2Int.zero;
        }
    }

    public void InitializeSpawner(BoardObject boardObject, GridController mainGridController, HexGridLayout mainGridLayout){
        this.mainGridController = mainGridController;
        this.mainGridLayout = mainGridLayout;
        counter = 0.0f;
        this.myBoardObject = boardObject;
    }

    public void InitializeSpawner(BoardObject boardObject, GridController mainGridController, HexGridLayout mainGridLayout, GameObject spawn){
        this.mainGridController = mainGridController;
        this.mainGridLayout = mainGridLayout;
        this.spawn = spawn;
        counter = 0.0f;
        this.myBoardObject = boardObject;
    }

    private void AttemptSpawn(){
        foreach(Vector2Int successor in mainGridLayout.SuccessorsFromPosition(GetPosition())){
            if(!mainGridController.GetObjectAtPosition(successor).HasProperty(ObjectProperties.SOLID)){
                mainGridController.SpawnAtPosition(spawn, successor);
                return;
            }
        }
    }

    void Update(){
        // Only step after init
        if(spawn != null){
            counter += Time.deltaTime;

            if(counter >= frequency){
                AttemptSpawn();
                counter = 0.0f;
            }
        }
    }
}
