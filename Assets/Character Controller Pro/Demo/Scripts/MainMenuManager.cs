using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lightbug.CharacterControllerPro.Demo
{

public class MainMenuManager : MonoBehaviour
{	
    string mainMenuName = "";


	static MainMenuManager instance = null;
	public static MainMenuManager Instance
	{
		get
		{			
			return instance;
		}
	}

	void Awake()
	{
		if( instance == null )
		{
			instance = this;
			DontDestroyOnLoad( gameObject );

			mainMenuName = SceneManager.GetActiveScene().name;
		}
		else
		{
			Destroy( gameObject );
		}
		
		// Shader.globalMaximumLOD = 201;

		/*	
		----------------------------------------------------------------------------------	
		VertexLit kind of shaders = 100
		Decal, Reflective VertexLit = 150
		Diffuse = 200
		Diffuse Detail, Reflective Bumped Unlit, Reflective Bumped VertexLit = 250
		Bumped, Specular = 300
		Bumped Specular = 400
		Parallax = 500
		Parallax Specular = 600
		----------------------------------------------------------------------------------
		*/
	}

	public void QuitApplication()
	{
		Application.Quit();
	}

	public void GoToScene( string sceneName )
	{
		if(sceneName == mainMenuName )
			Cursor.visible = true;
		else
			Cursor.visible = false;
		
		SceneManager.LoadScene( sceneName , LoadSceneMode.Single );
	}
	
	void Update()
	{
		if( Input.GetKeyDown(KeyCode.Escape))
		{
			if( SceneManager.GetActiveScene().name == mainMenuName )
				Application.Quit();
			else
				GoToScene( mainMenuName );
		}
	}
}

}
