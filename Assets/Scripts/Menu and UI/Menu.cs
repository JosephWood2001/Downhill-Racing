using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public GameObject content;

    [HideInInspector]
    public MenusManager parentMenu;

    private bool isEnabled = false;
    protected float deactivateTime = 0;

    public bool hasLocation = false;
    public string tagName;

    public bool IsEnabled{
        get
        {
            return isEnabled;
        }
        set
        {
            if (isEnabled != value)
            {
                isEnabled = value;
                if (value)
                {
                    Activate();
                }
                else
                {
                    Deactivate();
                }
            }
            
        }
    }

    public virtual void Activate() {
        content.SetActive(true);
    }

    public virtual void Deactivate() {
        content.SetActive(false);
    }
}
