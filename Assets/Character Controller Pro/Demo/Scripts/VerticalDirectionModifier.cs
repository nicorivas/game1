using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.CharacterControllerPro.Core;



namespace Lightbug.CharacterControllerPro.Demo
{

public abstract class VerticalDirectionModifier : MonoBehaviour
{
    
    [SerializeField]
    protected CharacterReferenceObject reference = new CharacterReferenceObject();

    [Tooltip("The duration this modifier will be inactive once it is activated. " + 
    "Use this to prevent the character from re-activating the effect over and over again (the default value of 1 second should be enough.)")]
    [SerializeField]
    float waitTime = 1f;

    protected bool isReady = true;
    float time = 0f;

    protected Dictionary< Transform , CharacterActor > characters = new Dictionary< Transform , CharacterActor >();

    
    void Update()
    {
        if( isReady )
            return;

        time += Time.deltaTime;

        if( time >= waitTime )
        {
            time = 0f;
            isReady = true;
        }
    }
    
    protected void ChangeGravitySettings( CharacterActor character )
    {
        if( reference == null )
            return;                
        
        
        if( reference.verticalAlignmentReference != null )
        {                        
            character.VerticalAlignmentReference = reference.verticalAlignmentReference;      
        }
        else
        {
            
            character.VerticalAlignmentReference = null;
            character.VerticalAlignmentDirection = reference.referenceTransform.up;
        }
        
        isReady = false;
    }
    
    protected CharacterActor GetCharacter( Transform objectTransform )
    {
        CharacterActor characterActor;
		bool found = characters.TryGetValue( objectTransform , out characterActor );

		if( !found )
        {		
			characterActor = objectTransform.GetComponent<CharacterActor>();

			if( characterActor != null )
				characters.Add( objectTransform , characterActor );			
			
		}

        return characterActor;
    }
}




}


