using UnityEngine;
using Lightbug.CharacterControllerPro.Core;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lightbug.CharacterControllerPro.Demo
{

public class VerticalDirectionModifier3D : VerticalDirectionModifier
{
    void OnTriggerEnter( Collider other )
    {
        if( !isReady )
            return;
        
        CharacterActor characterActor = GetCharacter( other.transform );
        if( characterActor != null )
        {
            ChangeGravitySettings( characterActor );
            characterActor.Teleport( reference.referenceTransform );
        }
    }
}

#if UNITY_EDITOR

[CustomEditor( typeof( VerticalDirectionModifier3D ) )]
public class VerticalDirectionModifier3DEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox( "The trigger will only start the transition. The character will be teleported using the reference transform information (position and rotation)." , MessageType.Warning );
        DrawDefaultInspector();
    }
}

#endif

}
