using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Lightbug.CharacterControllerPro.Implementation
{

/// <summary>
/// This class reads the actions of a 2D UI button and then sends the states flags to a mobile input component.
/// </summary>
[AddComponentMenu("Character Controller Pro/Implementation/UI/Input Button")]
public class InputButton : MonoBehaviour , IPointerUpHandler , IPointerDownHandler
{
    [SerializeField]
    string actionName = ""; 
    
    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
       

    public string ActionName
    {
        get
        {
            return actionName;
        }
    }

    BoolAction boolAction = new BoolAction();

    public bool ButtonValue
    {
        get
        {
            return boolAction.value;
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        boolAction.value = true;           
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        boolAction.value = false;        
    }

    
}


}