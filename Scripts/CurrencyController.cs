using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyController : MonoBehaviour
{
    public static CurrencyController PlayerCurrencyController;

    public int currency;

    // Singleton paradigm
    void Awake(){
        if(PlayerCurrencyController == null){
            PlayerCurrencyController = this;
        }
        else{
            Destroy(gameObject);
        }
    }

    // Function to add currency
    public void AddCurrency(int amount){
        currency += amount;
    }

    // Function to remove currency
    public void RemoveCurrency(int amount){
        currency -= amount;
        if(currency < 0){
            currency = 0;
        }
    }

    // Function to get currency
    public int GetCurrency(){
        return currency;
    }

    // Function to check if we can afford something
    public bool CanAfford(int amount){
        if(currency - amount >= 0){
            return true;
        }
        return false;
    }
}
