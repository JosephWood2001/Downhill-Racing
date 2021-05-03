using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartInput
{

    private float horizontalInput = 0f;
    public float HorizontalInput
    {
        get
        {
            return horizontalInput;
        }
        set
        {
            horizontalInput = Mathf.Clamp(value, -1f, 1f);
        }
    }

    private float verticalInput = 0f;
    public float VerticalInput
    {
        get
        {
            return verticalInput;
        }
        set
        {
            verticalInput = Mathf.Clamp(value, -1f, 1f);
        }
    }

    private float airHorizontalInput = 0f;
    public float AirHorizontalInput
    {
        get
        {
            return airHorizontalInput;
        }
        set
        {
            airHorizontalInput = Mathf.Clamp(value, -1f, 1f);
        }
    }

    private float airVerticalInput = 0f;
    public float AirVerticalInput
    {
        get
        {
            return airVerticalInput;
        }
        set
        {
            airVerticalInput = Mathf.Clamp(value, -1f, 1f);
        }
    }

    private float airRollInput = 0f;
    public float AirRollInput
    {
        get
        {
            return airRollInput;
        }
        set
        {
            airRollInput = Mathf.Clamp(value, -1f, 1f);
        }
    }

    private float breakingInput = 0f;
    public float BreakingInput
    {
        get
        {
            return breakingInput;
        }
        set
        {
            breakingInput = Mathf.Clamp(value, 0f, 1f);
        }
    }

    private bool reset = false; // READING THIS WHEN TRUE WILL MAKE IT FALSE
    public bool Reset
    {
        get
        {
            if (reset)
            {
                reset = false;
                return true;
            }
            else
            {
                return false;
            }
        }
        set
        {
            reset = value;
        }
    }


}
