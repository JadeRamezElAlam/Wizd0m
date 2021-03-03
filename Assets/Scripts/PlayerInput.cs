using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{

    #region Variables

    // Stores horizontal input
    float m_h;
    public float H { get { return m_h; } }

    // Stores the vertical input 
    float m_v;
    public float V { get { return m_v; } }

    // Global flag for enabling and disabling user input
    bool m_inputEnabled = false;
    public bool InputEnabled { get { return m_inputEnabled; } set { m_inputEnabled = value; } }

    #endregion

    // Gets keyboard input
    public void GetKeyInput()
    {
        if (m_inputEnabled)
        {
            m_h = Input.GetAxisRaw("Horizontal");
            m_v = Input.GetAxisRaw("Vertical");
        }

        else
        {
            m_h = 0f;
            m_v = 0f;
        }
    }

}
