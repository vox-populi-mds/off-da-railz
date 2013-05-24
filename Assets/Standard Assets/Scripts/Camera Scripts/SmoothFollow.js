/*
This camera smoothes out rotation around the y-axis and height.
Horizontal Distance to the target is always fixed.

There are many different ways to smooth the rotation but doing it this way gives you a lot of control over how the camera behaves.

For every of those smoothed values we calculate the wanted value and the current value.
Then we smooth it using the Lerp function.
Then we apply the smoothed values to the transform's position.
*/

// The target we are following
var target : Transform;
// The distance in the x-z plane to the target
private var distance = 10.0;
private var hitDistance = 10.0f;
// the height we want the camera to be above the target
var height = 5.0;
// How much we 
var heightDamping = 2.0;
var rotationDamping = 3.0;

var CollisionDetected : boolean;
var CollisionCoolDown : float = 0.0f;
var minDistance = 10.0f;
var maxDistance = 40.0f;

// Place the script in the Camera-Control group in the component menu
@script AddComponentMenu("Camera-Control/Smooth Follow")

function SetDistance(_distance : float)
{
	if(CollisionDetected == false)
	{
		distance = _distance;
	}
}

function GetDistance() : float
{
	return(distance);
}

function LateUpdate ()
{
	// Early out if we don't have a target
	if (!target)
		return;
		
	var Layermask : int = 1 << 9;
	Layermask = ~Layermask;
	var hit : RaycastHit;
		
	if(CollisionDetected)
	{
		CollisionCoolDown += Time.deltaTime;
		if(CollisionCoolDown > 2.0f)
		{
			if( Physics.Linecast(transform.position, target.position, hit, Layermask) == false)
		    {
				CollisionCoolDown = 0.0f;
				CollisionDetected = false;		
			}		
		}
	}
	
	// Calculate the current rotation angles
	var wantedRotationAngle = target.eulerAngles.y;
	var wantedHeight = target.position.y + height;
		
	var currentRotationAngle = transform.eulerAngles.y;
	var currentHeight = transform.position.y;
	
	// Damp the rotation around the y-axis
	currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

	// Damp the height
	currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);

	// Convert the angle into a rotation
	var currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
	
	//if(CollisionDetected == false)
	//{		
		//Set the position of the camera on the x-z plane to:
		//distance meters behind the target
		transform.position = target.position;
		transform.position -= currentRotation * Vector3.forward * distance;     	
		
		// Set the height of the camera
		transform.position.y = currentHeight;		
	//}
	
	var TargetPos : Vector3 = target.position;
	TargetPos.y += 10;
	
	if( Physics.Linecast(TargetPos, transform.position, hit, Layermask))
    {
    	CollisionDetected = true;   
    	
    	var Direction : Vector3;
    	Direction = target.position - transform.position;
    	Direction.Normalize();
    	Direction *= 5.0f;   	       	
    	transform.position = hit.point + Direction; 
    	//transform.position = hit.point;
    	//Debug.DrawLine(transform.position, target.position, Color.red);	  	    	
    }  
	
	// Always look at the target
	transform.LookAt (target);
}
