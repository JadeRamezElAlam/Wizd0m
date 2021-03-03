using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(EnemyMover))]
[RequireComponent(typeof(EnemySensor))]
[RequireComponent(typeof(EnemyAttack))]
public class EnemyManager : TurnManager
{
    #region Variables
    EnemyMover m_enemyMover;
    EnemySensor m_enemySensor;
    EnemyAttack m_enemyAttack;

    Board m_board;
    
    bool m_isDead = false;
    public bool IsDead { get { return m_isDead; }}

    // Coroutine to invoke upon enemy death
    public UnityEvent deathEvent;
    
    #endregion

    // Setups Enemy component
    protected override void Awake()
    {
        base.Awake();

        m_board = Object.FindObjectOfType<Board>().GetComponent<Board>();
        m_enemyMover = GetComponent<EnemyMover>();
        m_enemySensor = GetComponent<EnemySensor>();
        m_enemyAttack = GetComponent<EnemyAttack>();

    }


    #region Enemy's Turn

    // Plays the Enemy's turn routine
    public void PlayTurn()
    {
        // If enemy is dead disables enemy behaviour completely
        if (m_isDead)
        {
            FinishTurn();
            return;
        }

        StartCoroutine(PlayTurnRoutine());
    }

    IEnumerator PlayTurnRoutine()
    {
        if (m_gameManager != null && !m_gameManager.IsGameOver)
        {
            // Detects player
            m_enemySensor.UpdateSensor(m_enemyMover.CurrentNode);

            yield return new WaitForSeconds(0f);

            if (m_enemySensor.FoundPlayer)
            {
				// Notify the GameManager to lose the level
				m_gameManager.LoseLevel();

                // Detects player's position
                Vector3 playerPosition = new Vector3(m_board.PlayerNode.Coordinate.x, 0f,
                                                     m_board.PlayerNode.Coordinate.y);
                // Moves to the Player's position
                m_enemyMover.Move(playerPosition, 0f);

                // Waits for the enemy iTween animation to finish
                while (m_enemyMover.isMoving)
                {
                    yield return null;
                }

                // Attacks/Kills the player   
                m_enemyAttack.Attack();


            }
            else
            {
                // Movement
                m_enemyMover.MoveOneTurn();
            }
        }
    }
    #endregion

    // Invokes the death event if enemy is not dead
    public void Die()
    {
        if (m_isDead)
        {
            return;
        }

        m_isDead = true;

        if (deathEvent != null)
        {
            deathEvent.Invoke();
        }
    }
}
