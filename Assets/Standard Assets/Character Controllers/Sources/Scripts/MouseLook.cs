using UnityEngine;
using System.Collections;

/// MouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character:
/// - Create a capsule.
/// - Add the MouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSInputController script to the capsule
///   -> A CharacterMotor and a CharacterController component will be automatically added.

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
/// - Add a MouseLook script to the camera.
///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour {

	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;

	public float minimumX = -360F;
	public float maximumX = 360F;

	public float minimumY = -60F;
	public float maximumY = 60F;

	float rotationY = 0F;

	public bool MouseLookOn = true;

	Quaternion OriginalRot;
	float startX, startY, startZ;

	public int deadzoneLeft = 100;
	public int deadzoneRight = 100;
	public int deadzoneTop = 100;
	public int deadzoneBottom = 100;

	void Update ()
	{
		if (MouseLookOn)
		{
			if (axes == RotationAxes.MouseXAndY)
			{
				float rotationX =
					((Input.mousePosition.x - (Screen.width / 2)) / (Screen.width / 2)) * sensitivityX;

				if (rotationX > (maximumX+180))
					rotationX -= 360;

				rotationX = Mathf.Clamp (rotationX, minimumX, maximumX);
				/*
				if (rotationX < 0)
				{
					rotationX = Mathf.Min(0, deadzoneLeft+rotationX);
				}
				else if (rotationX > 0)
				{
					rotationX = Mathf.Max(0, deadzoneRight-rotationX);
				}
				*/
				rotationY = ((Input.mousePosition.y - (Screen.height / 2)) / (Screen.height / 2)) * sensitivityY;
				rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);

				/*
				if (rotationY < 0)
				{
					rotationY = Mathf.Min(startY, deadzoneBottom+rotationY);
				}
				else if (rotationY > 0)
				{
					rotationY = Mathf.Max(startY, rotationY-deadzoneTop);
				}
				*/
				//transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
				transform.localRotation = 
					Quaternion.RotateTowards(
						transform.localRotation, 
						Quaternion.Euler(-rotationY+startX, rotationX+startY, startZ), 
						sensitivityX*Time.deltaTime);
			}
			else if (axes == RotationAxes.MouseX)
			{
				transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
			}
			else
			{
				rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
				rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
				
				transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
			}
		}
		else
		{
			transform.localRotation = Quaternion.RotateTowards(transform.localRotation, OriginalRot, sensitivityX*Time.deltaTime);
		}
	}
	
	void Start ()
	{
		// Make the rigid body not change rotation
		if (rigidbody)
			rigidbody.freezeRotation = true;
	}

	public void SetOriginalRot(Quaternion rot)
	{
		OriginalRot = rot;
		
		startX = OriginalRot.eulerAngles.x;
		startY = OriginalRot.eulerAngles.y;
		startZ = OriginalRot.eulerAngles.z;
	}
}