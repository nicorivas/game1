using UnityEngine;
using Lightbug.CharacterControllerPro.Core;

namespace Lightbug.CharacterControllerPro.Demo
{
	
public enum CameraPositionMode
{
	Bounds , 
	Point
}

[AddComponentMenu("Character Controller Pro/Demo/Camera/Camera 2D")]
public class Camera2D : MonoBehaviour
{
	[Header("Target")]

	[SerializeField] Transform target = null;	

	[Header("Camera size")]

	[SerializeField]
	Vector2 cameraAABBSize = new Vector2(3,4);
	
	[SerializeField] 
	Vector2 targetAABBSize = new Vector2(1,1);

	[Header("Position")]

	[SerializeField]
	CameraPositionMode mode = CameraPositionMode.Bounds;

	[SerializeField]
	Vector3 offset = new Vector3( 0f , 0f , - 10f );

	[SerializeField]
	float smoothTargetTime = 0.25f;

	[Header("Rotation")]

	[SerializeField] 
	bool followRotation = true;

	[Min(0.1f)] 
	[SerializeField] 
	float rotationSlerpSpeed = 5f;

	

	[Header("Look ahead")]
	[SerializeField] 
	float lookAheadSpeed = 4;

	[SerializeField] 
	float xLookAheadAmount = 1;

	[SerializeField] 
	float yLookAheadAmount = 1;

	// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
	// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
	// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
	

	float xCurrentLookAheadAmount = 0;
	float yCurrentLookAheadAmount = 0;

	Vector3 targetCameraPosition;
	
	Vector3 smoothDampVelocity;
	Bounds cameraAABB;
	Bounds targetBounds;
	
	CharacterActor characterActor;

	void Start()
	{

		if( target == null )		
			Debug.Log("Missing camera target");			
		

		characterActor = target.GetComponent<CharacterActor>();
		
		transform.position = target.position + transform.TransformVector( offset ); 

		targetBounds = new Bounds(target.position , new Vector3(targetAABBSize.x,targetAABBSize.y,1));
		targetBounds.center = target.position;

		cameraAABB = new Bounds(target.position , new Vector3(cameraAABBSize.x,cameraAABBSize.y,1) );

		targetCameraPosition = new Vector3( cameraAABB.center.x , cameraAABB.center.y , transform.position.z );
	}
	
	void OnDrawGizmos()
	{
		if( target == null )
			return;

		if( mode != CameraPositionMode.Bounds )
			return;
		
		Gizmos.color = new Color( 0f , 0f , 1f , 0.2f );

		Bounds bounds = new Bounds(target.position , new Vector3( cameraAABBSize.x , cameraAABBSize.y , 1f ) );
		Gizmos.DrawCube( bounds.center , new Vector3( bounds.size.x , bounds.size.y , 1f ) );		
	}

	void LateUpdate()
	{				
		if( target == null )
			return;		
		
		float dt = Time.deltaTime;

		UpdateTargetAABB();
		UpdateCameraAABB(dt);		

		if( followRotation )
			UpdateRotation(dt);		
		
		UpdatePosition(dt);
	}



	void UpdateTargetAABB()
	{
		targetBounds.center = target.position;
	}

	void UpdateCameraAABB( float dt )
	{
		float deltaLookAhead = lookAheadSpeed * dt;

		//X
		if( targetBounds.max.x > cameraAABB.max.x)
		{
			float deltaX = targetBounds.max.x - cameraAABB.max.x;			
			cameraAABB.center += Vector3.right * deltaX;

			if(xCurrentLookAheadAmount < xLookAheadAmount)
			{
				xCurrentLookAheadAmount += deltaLookAhead;
				xCurrentLookAheadAmount = Mathf.Clamp(xCurrentLookAheadAmount , - xLookAheadAmount , xLookAheadAmount);
			}


		}
		else if( targetBounds.min.x < cameraAABB.min.x)
		{

			float deltaX = cameraAABB.min.x - targetBounds.min.x;
			cameraAABB.center -= Vector3.right * deltaX;

			//Look Ahead
			if(xCurrentLookAheadAmount > - xLookAheadAmount)
			{
				xCurrentLookAheadAmount -= deltaLookAhead;
				xCurrentLookAheadAmount = Mathf.Clamp(xCurrentLookAheadAmount , - xLookAheadAmount , xLookAheadAmount);
				
			}
			
						
		}

		//Y
		if(targetBounds.max.y > cameraAABB.max.y)
		{
			float deltaY = targetBounds.max.y - cameraAABB.max.y;
			cameraAABB.center += Vector3.up * deltaY;

			

			//Look Ahead
			if(yCurrentLookAheadAmount < yLookAheadAmount)
			{
				yCurrentLookAheadAmount += deltaLookAhead;
				yCurrentLookAheadAmount = Mathf.Clamp(yCurrentLookAheadAmount , - yLookAheadAmount , yLookAheadAmount);
				
			}

			
		}
		else if(targetBounds.min.y < cameraAABB.min.y)
		{

			float deltaY = cameraAABB.min.y - targetBounds.min.y;
			cameraAABB.center -= Vector3.up * deltaY;			

			//Look Ahead
			if(yCurrentLookAheadAmount > - yLookAheadAmount)
			{
				yCurrentLookAheadAmount -= deltaLookAhead;
				yCurrentLookAheadAmount = Mathf.Clamp(yCurrentLookAheadAmount , - yLookAheadAmount , yLookAheadAmount);
			}
		}		

		targetCameraPosition.x = cameraAABB.center.x + xCurrentLookAheadAmount;
		targetCameraPosition.y = cameraAABB.center.y + yCurrentLookAheadAmount;
	}	
	

	void UpdatePosition( float dt )
	{
		Vector3 targetPos = Vector3.zero;
		if( mode == CameraPositionMode.Bounds )
		{
			targetPos = Vector3.SmoothDamp( transform.position , targetCameraPosition , ref smoothDampVelocity , smoothTargetTime ); 
			
		}
		else
		{
			targetPos = Vector3.SmoothDamp( transform.position , target.position + transform.TransformVector( offset ) , ref smoothDampVelocity , smoothTargetTime );
		}

		transform.position = targetPos;
	}

	void UpdateRotation( float dt )
	{
		Quaternion deltaRotation = Quaternion.FromToRotation( transform.up , target.up );
		transform.rotation *= Quaternion.Slerp ( Quaternion.identity , deltaRotation , rotationSlerpSpeed * dt );
		
	}

	
}

}
