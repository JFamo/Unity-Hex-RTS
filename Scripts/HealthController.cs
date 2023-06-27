using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    public float currentHealth = 10.0f;
    public float maximumHealth = 10.0f;
    public float regenRate = 0.01f;
    public float armor = 1.0f;
    public float dodgeChance = 0.1f;
    public float healPeriod = 1.0f;

    private float counter;

    private BoardObject myBoardObject;
    private UnitCanvasController unitCanvasController;

    public void InitializeHealthController(BoardObject boardObject, UnitCanvasController unitCanvasController){
        this.myBoardObject = boardObject;
        this.unitCanvasController = unitCanvasController;

        this.currentHealth = this.maximumHealth;
        this.counter = 0.0f;

        this.unitCanvasController.ToggleProgressBar(UnitCanvasElement.HEALTH, true);
    }

    private void UpdateHealth(float value){
        // Set without going over
        this.currentHealth = value;
        if(this.currentHealth > this.maximumHealth){
            this.currentHealth = this.maximumHealth;
        }

        // Check death
        if(this.currentHealth <= 0.0f){
            this.myBoardObject.NotifyDead();
            return;
        }

        // Update healthbar
        this.unitCanvasController.SetProgressBar(UnitCanvasElement.HEALTH, this.currentHealth / this.maximumHealth);
    }

    private bool RollForDodge(){
        if(Random.Range(0, 100) < dodgeChance * 100){
            return true;
        }
        return false;
    }

    public void AddHealth(float value){
        this.UpdateHealth(this.currentHealth + value);
    } 

    public void NotifyTakeDamage(float value){
        if(RollForDodge()){
            // TODO
        }
        else{
            this.UpdateHealth(this.currentHealth - (value / this.armor));
        }
        
    }   

    void Update(){
        counter += Time.deltaTime;

        if(counter >= healPeriod){
            AddHealth(regenRate);
            counter = 0.0f;
        }
    }
}
