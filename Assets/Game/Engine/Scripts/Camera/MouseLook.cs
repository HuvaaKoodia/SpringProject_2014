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

	public BoxCollider DeadZone;

	public float currentX, currentY;
	public bool clampX, clampY;

	int layer;

	void Update ()
	{
		//VERSIO 4, KAIKKI MUUT VERSIOT KOMMENTTEIHIN JA TÄÄLLÄ YLHÄÄLLÄ NÄIN
		//VERSION 4 SAA POIS PÄÄLTÄ JOS PAKOTTAA isInDeadzonen TRUEKSI
		Ray mouseRay = camera.ScreenPointToRay(Input.mousePosition);
		bool isInDeadzone = Physics.Raycast(mouseRay, 1, layer);

		if (isInDeadzone)
		{
			Debug.DrawRay(mouseRay.origin, mouseRay.direction, Color.green);
		}
		else
		{
			Debug.DrawRay(mouseRay.origin, mouseRay.direction, Color.red);
		}

		if (MouseLookOn && !isInDeadzone)
		{
			if (axes == RotationAxes.MouseXAndY)
			{


				float rotationX =
					((Input.mousePosition.x - (Screen.width / 2)) / (Screen.width / 2)) * sensitivityX;

				if (rotationX > (maximumX+180))
					rotationX -= 360;

				rotationX = Mathf.Clamp (rotationX, minimumX, maximumX);

				rotationY = ((Input.mousePosition.y - (Screen.height / 2)) / (Screen.height / 2)) * sensitivityY;
				rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);

				/*
				bool insideDeadZone = Subs.insideArea(rotationX, rotationY, 
				                                      -deadzoneLeft, -deadzoneBottom, 
				                                      deadzoneLeft+deadzoneRight, deadzoneBottom+deadzoneTop);

				//VERSIO 1, KAIKKI MUU PAITSI TÄMÄ IF KOMMENTTEIHIN
				if (insideDeadZone)
				{
					rotationX = 0;
					rotationY = 0;
				}
				else //VERSIO 2, OTA TÄMÄ ELSE VERSION 1 MUKAAN
				{
					clampX = false;
					clampY = false;

					if (rotationX < -deadzoneLeft || rotationX > deadzoneRight)
						clampX = true;

					if (rotationY < -deadzoneBottom || rotationY > deadzoneTop)
						clampY = true;


					if (rotationX < -deadzoneLeft)
						rotationX += deadzoneLeft;
					else if (rotationX > deadzoneRight)
						rotationX -= deadzoneRight;
					
					if (!clampX && clampY)
					{
					if (rotationY < -deadzoneBottom)
						rotationY += deadzoneBottom;
					else if (rotationY > deadzoneTop)
						rotationY -= deadzoneTop;
					}
				}

				//VERSIO 3, TÄSTÄ ALASPÄIN KOMMENTIT POIS JA YLÖS KAIKKI KOMMENTTEIHIN
				/*
				if (rotationX < -deadzoneLeft)
					rotationX += deadzoneLeft;
				else if (rotationX > deadzoneRight)
					rotationX -= deadzoneRight;
				else
					rotationX = 0;

				if (rotationY < -deadzoneBottom)
					rotationY += deadzoneBottom;
				else if (rotationY > deadzoneTop)
					rotationY -= deadzoneTop;
				else
					rotationY = 0;
				*/

				currentX = rotationX;
				currentY = rotationY;

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

		layer = 1 << LayerMask.NameToLayer("MouseLookDeadzone");
	}

	public void SetOriginalRot(Quaternion rot)
	{
		OriginalRot = rot;
		
		startX = OriginalRot.eulerAngles.x;
		startY = OriginalRot.eulerAngles.y;
		startZ = OriginalRot.eulerAngles.z;
	}
}