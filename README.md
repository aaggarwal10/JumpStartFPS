# MasseyHacks VIII JumpStart - Game Development
This is a repository designed to introduce students to the Unity 3D Platform. The following is a lesson plan made for MasseyHack's JumpStart Program:

## Introduction
Unity is a great platform used for building a variety of applications. From Game to Medical Applications, Unity is a multiplatform tool that has use even in professional standards. I will be bringing you through Unity primarily through a Game Development viewpoint, but do not think that Unity can only be used in this field further down the line.

## Description of Layout
When you start up Unity, you will see a default layout something like this:

![Layout Image](https://github.com/aaggarwal10/7WC-GameDev/blob/main/Images/layout.png?raw=true)

The Unity Layout is separated into many sections with different purposes. For this tutorial we will look at the basic sections:
* **Project:** Assets and Models are stored (Much like a file manager)
* **Console:** Output from run time is displayed here (including warnings/errors)
* **Scene:** The 3D world containing all objects and the environment in the game
* **Game:** The Environment visible through cameras (What the player will see when they run the game)
* **Hierarchy:** Stores references and structure of all objects in the scene
* **Inspector:** Panel that allows developers to inspect and alter the properties of objects in the scene and project

Any Unity developer will find themselves interacting with these sections for any Unity project. Now to get familiar with these sections the best way is through practice. 

## Game Dev. Workshop ~ Part 1: Introduction to Unity
To make a moving character we first will create a capsule like stand-in. The way to create basic objects is through the *Hierarchy* section.

![Hierarchy Image](https://github.com/aaggarwal10/7WC-GameDev/blob/main/Images/hierarchy.png?raw=true)

By right clicking in the Hierarchy, we can see many things that we can add like Empty Objects, 3D Objects, Lights, Cameras, etc. As we want to create a capsule, we will hover over 3D Object, and select a capsule now. We have a capsule in the scene. However, we might want to navigate around the scene to better see the cube. 

![Controls](https://github.com/aaggarwal10/7WC-GameDev/blob/main/Images/controls.png?raw=true)

At the top left, there is a task bar with different settings that will allow you to do different actions in the scene:
* **Hand Tool:** Used to pan (rotate on right click) in scene to move viewpoint (shortcut: middle mouse button)
* **Move Tool:** Allows the selection of objects and the changing of an objects position in the scene (Most Used Probably)
* **Rotate Tool:** Rotation of object in scene
* **Scale Tool:**  Scaling of object (Shrink, Expand)
* **Rect Tool:** 2D changes mainly in UI.

Furthermore, one can go forward and backward in the scene by using the scroll wheel.

Now, let us take a look at the inspector. Double click on the capsule in the scene. It will bring you to a closer view of the capsule in the scene (in case you lost it). On the right side, you should also see the details of the capsule in the inspector. You can try using the scale / rotate tool or you can try changing the transform properties of the object.

![Box Collider](https://github.com/aaggarwal10/7WC-GameDev/blob/main/Images/box.png?raw=true)
You should also see the collider component on the capsule. A capsule automatically has a collider to deal with physics. Before we start making our cube move, let us start with the "groundwork." We will first make a plane to act as ground so that we can give our cube gravity so that it does not just float through the air. This is simple in Unity, by making a plane below the cube in the hierarchy, and then adding a rigid body component to the cube while turning on gravity. Congratulations, you have successfully created gravity in Unity. 

![Update Project](https://github.com/aaggarwal10/7WC-GameDev/blob/main/Images/material.png?raw=true)
We can know add colour to our monotonic world by creating materials in our project folder. Specifically, right clicking in a the project section will allow you to *Create > Material*. Then, you can change the colour properties in the inspector and drag these materials onto objects into the scene to give those objects that specific colour property.

Now, we need to make our capsule work like in a FPS. As we are creating an FPS, we want the camera to follow the player. This means parenting the camera to the player, as the player parent is what is going to move. Parenting is done via the Unity Scene Hierarchy, where you drag an object to be “under” another one. In order to do this, we need to use **Scripting**. We will start creating a C# script (Unity's scripting language), by making a new script in the project. Again, *Right Click > Create > C# Script*. Name the script and open it in Visual Studios or another editor of your choice.

From this script, you will see the two basic functions of Unity scripting. Start is called on the frame when a script is enabled just before any of the Update methods are called the first time. Update, on the other hand, is called once each frame. There are other built in runtime functions, but for now we will focus on these two.

We will start by getting the "look-around" feel of the FPS. Specifically looking at how the mouse changes where the character is looking:

```C#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client.Scripts.PlayerScripts
{
  public class PlayerCamera : MonoBehaviour
  {
    public float sensX, sensY; // sensitivity of mouse, can be adjusted in most FPS games
    public Transform orientation; // a component of our player that allows us to look at the y-rotation specifically
    private float xRotation, yRotation;

    private void Start()
    {
      // Set the cursor mode and visibility as an FPS would have
      Cursor.lockState = CursorLockMode.Locked;
      Cursor.visible = false;
    }

    private void Update()
    { // Get rotation from mouse cursor input
      float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
      float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;
      yRotation += mouseX;

      xRotation -= mouseY;
      xRotation = Mathf.Clamp(xRotation, -90f, 90f);

      transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
      orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
  }
}
```

Now, we will move onto to trying to make this a first player controller. To do this we need to look at the **Camera** in the scene. The Camera provides the viewport for the player which means that what the camera sees is what the player sees in the world when they play the game. Thus, to do this we will attach the Camera to the cube in the hierarchy. This is called parenting an object. Simply put the Capsule Player becomes the *parent* of the Camera while the Camera becomes its *child*. This means that the transforms of the Cube will affect the Camera transform as well. We will now add a separate PlayerMovement script that goes into detail on the motion of the capsule as shown below:

```C#
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client.Scripts.PlayerScripts
{
  public class PlayerMovement : MonoBehaviour
  {
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;

    public float jumpForce, jumpCooldown;
    public float airMultiplier;
    bool readyToJump = true;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    private bool grounded;
    public Transform orientation;



    private float horizontalInput, verticalInput;
    private Vector3 moveDirection;
    private Rigidbody rb;

    private void Start()
    {
      rb = GetComponent<Rigidbody>();
      rb.freezeRotation = true;
    }

    private void Update()
    {
      // Ground Check
      grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
      if (grounded)
        rb.drag = groundDrag;
      else
        rb.drag = 0;

      MyInput();
      SpeedControl();
    }

    private void MyInput()
    {
      horizontalInput = Input.GetAxisRaw("Horizontal");
      verticalInput = Input.GetAxisRaw("Vertical");

      if (Input.GetKey(jumpKey) && readyToJump && grounded)
      {
        readyToJump = false;
        Jump();
        Invoke(nameof(ResetJump), jumpCooldown);
      }
    }

    private void ResetJump()
    {
      readyToJump = true;
    }

    private void Jump()
    {
      rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
      rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void MovePlayer()
    {
      moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
      if (grounded)
        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
      else
        rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void FixedUpdate()
    {
      MovePlayer();
    }

    private void SpeedControl()
    {
      Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
      if (flatVel.magnitude > moveSpeed)
      {
        Vector3 limitedVel = flatVel.normalized * moveSpeed;
        rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
      }
    }
  }
}
```

With that we have completed our FPS Motion Scripts, and now the FPS character has a complete and proper motion set including jumping. Now, lets move onto the next part of getting a gun, and a target.

## Part 2: Collisions & Events
In this tutorial, we will use the following two assets from the asset store:
* [Sci-Fi Gun](https://assetstore.unity.com/packages/3d/props/guns/sci-fi-gun-162872#content)
* [Training Dummy](https://assetstore.unity.com/packages/3d/props/lowpoly-training-dummy-202311/)

Try attaching this to your capsule player similar to how you did with the camera. Position it to be offset from the center somewhat like what you would see in a traditional FPS. Now, add a sphere for the bullet that starts off in the gun. Now, we can start with coding the actual firing mechanic. 

```C#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client.Scripts.InteractionScripts
{
  public class ShootingInteractions : MonoBehaviour
  {
    [Header("Gun Settings")]
    public float maxBulletSpeed, shootingCoolDown;
    public GameObject bullet;
    private bool readyToShoot = true;

    private Vector3 startPosition, targetPosition, currentPosition;
    private GameObject shotBullet = null;

    [Header("Target Description")]

    public LayerMask whatIsEnemy;
    public TargetInteractions targetInteractions;

    private void Update()
    {
      MyInput();
      ProcessBulletPosition();
    }

    private void MyInput()
    {
      if (Input.GetMouseButtonDown(0) && readyToShoot)
      {
        readyToShoot = false;
        Shoot();
        Invoke(nameof(ResetShot), shootingCoolDown);
      }
    }

    private void ResetShot()
    {
      targetInteractions.onTargetHit.Invoke(false);
      Destroy(shotBullet);
      shotBullet = null;
      readyToShoot = true;
    }

    private void Shoot()
    {
      shotBullet = Instantiate(bullet, bullet.transform.position, bullet.transform.rotation);
      RaycastHit hit;
      int layerMask = 1 << 8;
      layerMask = ~layerMask;
      startPosition = shotBullet.transform.position;
      currentPosition = startPosition;
      if (Physics.Raycast(transform.position, transform.forward, out hit, 100f, layerMask))
      {
        Debug.Log(hit.transform.name);
        targetPosition = hit.point;
        if (hit.transform.tag == "Target")
        {
          targetInteractions.onTargetHit.Invoke(true);
        }
      }
      else
      {
        targetPosition = startPosition + transform.forward * 100f;
      }
    }

    private void ProcessBulletPosition()
    {
      if ((targetPosition - startPosition).magnitude > (currentPosition - startPosition).magnitude)
      {
        currentPosition += (targetPosition - startPosition) / maxBulletSpeed;
        shotBullet.transform.position = currentPosition;
      }
    }
  }
}

```
![Trigger Component](https://github.com/aaggarwal10/7WC-GameDev/blob/main/Images/trigger.png?raw=true)

Now, our movement of the character should be complete. To add the wall breaking component, we need to look at the collisions between colliders. If you recall with the rigid body before we used Unity's inbuilt engine to work out the collisions for us. However, in some cases, we do not want to actually have collision physics, but instead only want to check if two objects collide. We can do this through a OnTrigger Script. Simply, what an OnTrigger event does is it checks when a Trigger object collides with a Non-Trigger Object. Using an on Trigger Exit we can simply look for collision through the script:
```C#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDestroy : MonoBehaviour
{
    public Animator animator;
    private void OnTriggerExit(Collider coll)
    {
        if (coll.tag == "Wall" && animator.GetCurrentAnimatorStateInfo(0).IsName("Punch"))
        {
            Destroy(coll.gameObject);
            Debug.Log(coll.name);
        }
    }
}
```
Note: To put this script on the humanoid character, we want to put it on the hand collider object, which will be in the rigged bone hierarchy. With all of this complete we will have our Wall Breaking Project Complete. 

For the second part of the Lesson, I did not go through the specific Unity editor parts like rigging and animating your humanoid controller. It might differ depending on the model you use. For our purposes, I have gone through this in the Workshop so if you are having difficulty with it, I highly recommend going through the Workshop Video for the in editor portions. 

## Resources
* **[Unity Documentation](https://docs.unity3d.com/Manual/index.html)**: Unity has a great amount of documentation that should be referenced when looking for syntax or help on specific problems.
* **[Unity Asset Store](https://assetstore.unity.com/)**: Unity has an amazing asset store with tons of free assets that can be used to get free models and animations for your game.