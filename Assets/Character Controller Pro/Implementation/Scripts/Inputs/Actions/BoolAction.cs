using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lightbug.CharacterControllerPro.Implementation
{

/// <summary>
/// This struct contains all the button states, which are updated frame by frame.
/// </summary>
[System.Serializable]
public struct BoolAction
{       
    
    public bool value;  

    /// <summary>
    /// 
    /// </summary>
	public bool Started { get; private set; }

    /// <summary>
    /// 
    /// </summary>
	public bool Canceled { get; private set; }

    bool previousValue;

    /// <summary>
    /// Resets the values.
    /// </summary>
    public void Reset()
    {
        Started = false;
        Canceled = false; 
    }

    // /// <summary>
    // /// Updates the fields based on the current button state.
    // /// </summary>
    // public void SetValue( bool value )
    // {                
    //     this.value = value;
    // }  

    /// <summary>
    /// Updates the fields based on the current button state.
    /// </summary>
    public void Update()
    {
        this.Started |= !previousValue && value;
        this.Canceled |= previousValue && !value; 
        
        previousValue = value;
    }    
    
    
}


// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
// EDITOR ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

#if UNITY_EDITOR

[CustomPropertyDrawer( typeof( BoolAction ) )]
public class BoolActionEditor : PropertyDrawer
{
    GUIStyle style = new GUIStyle();

    Texture arrowTexture = null;

    // string[] enumOptions = new string[]
    // { 
    //     " ----- " ,
    //     "Pressed" ,
    //     "Released" ,
    //     "HeldDown" 
    // };

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty( position , label , property );
        
        SerializedProperty value = property.FindPropertyRelative("value");
        // SerializedProperty started = property.FindPropertyRelative("Started");        
        // SerializedProperty canceled = property.FindPropertyRelative("Canceled");

        if( arrowTexture == null )
            arrowTexture = Resources.Load<Texture>("whiteArrowFilledIcon");


        Vector2 labelSize = style.CalcSize( label );
        
        Rect fieldRect = position;
        fieldRect.height = EditorGUIUtility.singleLineHeight;
        fieldRect.width = 100;

        EditorGUI.LabelField( fieldRect , label );
        
        fieldRect.x += 110;        

        EditorGUI.PropertyField( fieldRect , value , GUIContent.none );
        // int selected = 0;

        // if( pressed.boolValue )
        //     selected = 3;
        // else
        //     selected = 0;

        // selected = EditorGUI.Popup( fieldRect , selected , enumOptions );

        // started.boolValue = selected == 1;
        // canceled.boolValue = selected == 2;
        // pressed.boolValue = selected == 3;
       
        

        EditorGUI.EndProperty();
    }

    void DrawToggleWithIcon( ref Rect fieldRect , SerializedProperty property , Texture iconTexture , float iconAngle )
    {
        fieldRect.width = fieldRect.height;

        
        GUI.color = Color.gray;
        GUIUtility.RotateAroundPivot( iconAngle , fieldRect.center );
        GUI.DrawTexture( fieldRect , iconTexture );
        GUIUtility.RotateAroundPivot( - iconAngle , fieldRect.center );
        GUI.color = Color.white;

        
        fieldRect.x += fieldRect.width;       

        fieldRect.width = 50;  // <-- Important!
        EditorGUI.PropertyField( fieldRect , property , GUIContent.none );
        fieldRect.x += 50;        
    }

 
}

#endif

}