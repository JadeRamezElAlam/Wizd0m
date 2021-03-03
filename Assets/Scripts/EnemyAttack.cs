using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{

   // Reference to PlayerManager component
    PlayerManager m_player;

    void Awake()
    {
        m_player = Object.FindObjectOfType<PlayerManager>().GetComponent<PlayerManager>();
    }

    // Kills the player
    public void Attack()
    {
        if (m_player != null)
        {
            m_player.Die();
        }
    }
}
