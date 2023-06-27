using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GridObject {
    NONE,
    TREE,
    ROCK,
    REDSOLDIER,
    BLUESOLDIER,
    REDTENT,
    BLUETENT,
    REDCANNON,
    BLUECANNON
}

public enum ObjectProperties {
    SOLID,
    SELECTABLE,
    MOVABLE,
    FRIENDLY,
    HOSTILE
}

public class GridController : MonoBehaviour
{
    public HexGridLayout mainLayout;
    private int width;
    private int height;

    public GameObject unitGroundCanvas;
    public GameObject unitProgressCanvas;

    public GameObject[] randomPrefabs;
    public float randomPrefabFrequency;

    public GameObject friendlyUnit;
    public GameObject enemyUnit;
    public GameObject friendlyCannon;
    public GameObject friendlyTent;
    public GameObject enemyTent;

    private BoardObject nullObject;
    private BoardObject[] grid;

    void Start(){
        width = mainLayout.gridSize.x;
        height = mainLayout.gridSize.y;

        grid = new BoardObject[width * height];
        nullObject = GetComponent<BoardObject>();

        for(int i = 0; i < width * height; i ++){
            grid[i] = nullObject;
        }

        SpawnUnits();
        RandomPopulation();
    }

    // Method to convert an index back to a position
    public Vector2Int IndexToPosition(int idx){
        int x = idx % width;
        int y = idx / width;
        return new Vector2Int(x,y);
    }

    // Methods to convert a position to an index
    public int PositionToIndex(int x, int y){
        return y * width + x;
    }

    public int PositionToIndex(Vector2Int posn){
        return PositionToIndex(posn.x, posn.y);
    }

    // Methods to get the transform location for a position
    public Vector3 GetTransformForPosition(int position){
        return mainLayout.GetPositionForHexFromCoord(IndexToPosition(position));
    }

    public Vector3 GetTransformForPosition(int x, int y){
        return mainLayout.GetPositionForHexFromCoord(new Vector2Int(x,y));
    }

    public Vector3 GetTransformForPosition(Vector2Int posn){
        return mainLayout.GetPositionForHexFromCoord(posn);
    }

    // Methods to spawn an object at some position
    public void SpawnAtPosition(GameObject item, int position){
        GameObject newObject = Instantiate(item);

        Vector3 gamePosn = mainLayout.GetPositionForHexFromCoord(IndexToPosition(position));
        newObject.transform.position = new Vector3(gamePosn.x, newObject.transform.position.y, gamePosn.z);

        BoardObject newBoardObject = newObject.GetComponent<BoardObject>();
        grid[position] = newBoardObject;
        newBoardObject.InitializeBoardObject(this, mainLayout, unitGroundCanvas, unitProgressCanvas);
        newBoardObject.NotifyPositionUpdate(IndexToPosition(position));
    }

    public void SpawnAtPosition(GameObject item, int x, int y){
        SpawnAtPosition(item, PositionToIndex(x,y));
    }

    public void SpawnAtPosition(GameObject item, Vector2Int posn){
        SpawnAtPosition(item, posn.x, posn.y);
    }

    // Method to spawn in some units for testing
    public void SpawnUnits(){
        SpawnAtPosition(friendlyUnit, 20, 20);
        SpawnAtPosition(friendlyUnit, 22, 20);
        SpawnAtPosition(friendlyUnit, 20, 22);
        SpawnAtPosition(friendlyTent, 10, 10);
        SpawnAtPosition(friendlyCannon, 11, 10);
        SpawnAtPosition(friendlyCannon, 11, 12);

        SpawnAtPosition(enemyUnit, 35, 35);
        SpawnAtPosition(enemyUnit, 32, 35);
        SpawnAtPosition(enemyUnit, 35, 33);
        SpawnAtPosition(enemyTent, 29, 35);
        SpawnAtPosition(enemyUnit, 30, 30);
        SpawnAtPosition(enemyUnit, 30, 31);
        SpawnAtPosition(enemyTent, 37, 38);
    }

    // Method to randomly spawn specified prefabs throughout
    public void RandomPopulation(){
        for(int i = 0; i < width * height; i ++){
            if(grid[i] == nullObject){
                if(Random.Range(0, 100) < randomPrefabFrequency){

                    SpawnAtPosition(randomPrefabs[Random.Range(0, randomPrefabs.Length)], i);

                }
            }
        }
    }

    // Method from a BoardObject to request to move
    public bool RequestMoveToPosition(BoardObject moverObject, Vector2Int newPosn){
        grid[PositionToIndex(moverObject.GetPosition())] = nullObject;
        grid[PositionToIndex(newPosn)] = moverObject;
        return true;
    }

    // Method for a BoardObject to notify that it has been destroyed
    public void NotifyDestruction(BoardObject destroyedObject){
        grid[PositionToIndex(destroyedObject.GetPosition())] = nullObject;
    }

    // Methods to get the object at some position
    public BoardObject GetObjectAtPosition(int idx){
        if(idx < width * height && idx >= 0){
            return grid[idx];
        }
        else{
            return nullObject;
        }
    }

    public BoardObject GetObjectAtPosition(int x, int y){
        return GetObjectAtPosition(PositionToIndex(x,y));
    }

    public BoardObject GetObjectAtPosition(Vector2Int posn){
        return GetObjectAtPosition(posn.x, posn.y);
    }
}
