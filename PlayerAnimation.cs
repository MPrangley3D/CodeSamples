using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[NOTES]This is all pretty simple, I just grab a handle to the player animator controller, and the special effects animator controller
//[NOTES]then I set parameters detailed in the animation controllers
//[NOTES]the important element here is handling this in an animation controller class, rather than keeping it all tangled up in the Player controller script
public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;
    private Animator effects;

    void Start()
    {
        anim = transform.GetChild(0).GetComponent<Animator>();
        effects = transform.GetChild(1).GetComponent<Animator>();
    }

    public void Move(float move)
    {
        anim.SetFloat("Move", Mathf.Abs(move));
    }

    public void Jump(bool jump)
    {
        anim.SetBool("Jumping", jump);
    }

    public void Attack()
    {
        anim.SetTrigger("Attack");
        effects.SetTrigger("SwordAnim");
    }
}