using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UnitCanvasElement {
    MOVEMENT,
    ATTACK,
    HEALTH
}

public class UnitCanvasController : MonoBehaviour
{
    private Image[] progressBarImages;

    public void InitializeUnitCanvasController(GameObject progressCanvas){
        this.progressBarImages = progressCanvas.GetComponentsInChildren<Image>();
    }

    public void ToggleProgressBar(UnitCanvasElement element, bool value){
        int index = (int)element;
        if(progressBarImages[index * 2]){
            progressBarImages[index * 2].enabled = value;
        }
        if(progressBarImages[(index * 2) + 1]){
            progressBarImages[(index * 2) + 1].enabled = value;
        }
    }

    public void SetProgressBar(UnitCanvasElement element, float fraction){
        int index = (int)element;
        if(progressBarImages[(index * 2) + 1]){
            progressBarImages[(index * 2) + 1].gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(fraction * 5.6f, 0.6f);
        }
    }
}
