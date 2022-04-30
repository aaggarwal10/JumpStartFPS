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
        currentPosition += (targetPosition - startPosition).normalized * maxBulletSpeed;
        shotBullet.transform.position = currentPosition;
      }
    }
  }
}
