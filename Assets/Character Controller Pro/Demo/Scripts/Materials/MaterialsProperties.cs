using UnityEngine;

namespace Lightbug.CharacterControllerPro.Demo
{

/// <summary>
/// This ScriptableObject contains all the properties used by the volumes and the surfaces. Create many instances as you want to create different environments.
/// </summary>
[CreateAssetMenu( menuName = "Character Controller Pro/Demo/Materials/Material Properties")]
public class MaterialsProperties : ScriptableObject
{
    [SerializeField]
    Surface defaultSurface = new Surface();

    [SerializeField]
    Volume defaultVolume = new Volume();

    [SerializeField]
    Surface[] surfaces = null;

    [SerializeField]
    Volume[] volumes = null;

    public Surface DefaultSurface
    {
        get
        {
            return defaultSurface;
        }
    }

    public Volume DefaultVolume
    {
        get
        {
            return defaultVolume;
        }
    }

    public Surface[] Surfaces
    {
        get
        {
            return surfaces;
        }
    }    

    public Volume[] Volumes
    {
        get
        {
            return volumes;
        }
    }

    public bool GetSurface( GameObject gameObject , ref Surface outputSurface )
    {        
        outputSurface = null;

        for( int i = 0 ; i < surfaces.Length ; i++ )
        {
            Surface surface = surfaces[i];
            
            if( gameObject.CompareTag( surface.tagName ) )
            {               
                outputSurface = surface; 
                return true;
            }                
        }

        return false;
    }

    public bool GetVolume( GameObject gameObject , ref Volume outputVolume )
    {
        outputVolume = null;
        
        for( int i = 0 ; i < volumes.Length ; i++ )
        {
            Volume volume = volumes[i];
            
            if( gameObject.CompareTag( volume.tagName ) )
            {               
                outputVolume = volume; 
                return true;
            }                
        }

        return false;
    }
}



[System.Serializable]
public class Surface
{
    public string tagName = "";
    
    [Header("Movement")]

    [Min( 0.01f )]
    public float accelerationMultiplier = 1f;

    [Min( 0.01f )]
    public float decelerationMultiplier = 1f;

    [Min( 0.01f )]
    public float speedMultiplier = 1f;

    [Header("Particles")]

    public Color color = Color.gray;
}


[System.Serializable]
public class Volume
{    
    public string tagName = "";

    [Header("Movement")]

    [Min( 0.01f )]
    public float accelerationMultiplier = 1f;

    [Min( 0.01f )]
    public float decelerationMultiplier = 1f;

    [Min( 0.01f )]
    public float speedMultiplier = 1f;
    
    [Range( 0.05f , 50f )]
    public float gravityAscendingMultiplier = 1f;

    [Range( 0.05f , 50f )]
    public float gravityDescendingMultiplier = 1f;
 

}

}

