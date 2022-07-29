using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideBarManager : MonoBehaviour
{
    bool open = false;
    public Animator animator;

    public void ToggleSideBar()
    {
        if (open)
        {
            open = false;
            animator.SetTrigger("Close");
        }
        else
        {
            open = true;
            animator.SetTrigger("Open");
        }
    }
    
}
