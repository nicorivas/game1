using UnityEngine;
using Lightbug.CharacterControllerPro.Core;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lightbug.CharacterControllerPro.Demo
{

public class VerticalDirectionModifier2D : VerticalDirectionModifier
{  
    
    void OnTriggerEnter2D( Collider2D other )
    {
        if( !isReady )
            return;
        
        CharacterActor characterActor = GetCharacter( other.transform );
        
        if( characterActor != null )
        {
            ChangeGravitySettings( characterActor );
            characterActor.Up = reference.referenceTransform.up;
        }
    }
}

#if UNITY_EDITOR

[CustomEditor( typeof( VerticalDirectionModifier2D ) )]
public class VerticalDirectionModifier2DEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox( "The trigger will only start the transition. The character will be teleported using the reference transform up direction." , MessageType.Warning );
        DrawDefaultInspector();
    }
}

#endif

}
