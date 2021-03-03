using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySensor : MonoBehaviour
{
    #region Variables

    public Vector3 directionToSearch = new Vector3(0f, 0f, 2f);

    Node m_nodeToSearch;

    Board m_board;

    bool m_foundPlayer = false;

    public bool FoundPlayer { get { return m_foundPlayer; }}

    #endregion

    void Awake()
    {
        m_board = Object.FindObjectOfType<Board>().GetComponent<Board>();
    }

    #region Sensor Behaviour

    // Checks if the Player has moved into the sensor
    public void UpdateSensor(Node enemyNode)
    {
        // Converts the local directionToSearch into a 3d position
        Vector3 worldSpacePositionToSearch = transform.TransformVector(directionToSearch) 
                                                      + transform.position;
        if (m_board != null)
        {
            // Finds the node at the world space position to search
            m_nodeToSearch = m_board.FindNodeAt(worldSpacePositionToSearch);

            // If the enemy's Node is not connected to the Node to search, we cannot detect the Player
            if (!enemyNode.LinkedNodes.Contains(m_nodeToSearch))
            {
                m_foundPlayer = false;
                return;
            }

            // If the node to search is the PlayerNode, then we have found the Player
            if (m_nodeToSearch == m_board.PlayerNode)
            {
                m_foundPlayer = true;
            }
        }
    }

    #endregion

}
