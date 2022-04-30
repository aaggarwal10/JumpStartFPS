using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
namespace Client.Scripts.InteractionScripts
{
  public class TargetInteractions : MonoBehaviour
  {
    public UnityEvent<bool> onTargetHit;
    public Animator animator;

    public TMP_Text scoreText;
    public Transform blocks;

    private bool isHit;
    private int score = 0;
    private void Start()
    {
      onTargetHit.AddListener(HandleOnTargetHitEvent);
    }

    private void HandleOnTargetHitEvent(bool isDead)
    {
      if (isDead)
      {
        score += 1;
        scoreText.text = "Score: " + score;
        isHit = true;
      }
      else
      {
        if (isHit)
        {
          int numChildren = blocks.childCount;
          int randomChild = UnityEngine.Random.Range(0, numChildren);
          Transform block = blocks.GetChild(randomChild);
          transform.position = new Vector3(block.position.x, block.position.y + (float)0.5 * block.localScale.y, block.position.z);
          isHit = false;
        } 
      }
      animator.SetBool("Die", isDead);
    }
  }
}
