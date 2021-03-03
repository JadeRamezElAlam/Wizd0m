using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerMover))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerDeath))]
public class PlayerManager : TurnManager
{
    #region Varialbes#

	public PlayerMover playerMover;
    public PlayerInput playerInput;

    Board m_board;

    public UnityEvent deathEvent;

    #endregion

    protected override void Awake()
    {
        base.Awake();

		playerMover = GetComponent<PlayerMover>();
        playerInput = GetComponent<PlayerInput>();

        m_board = Object.FindObjectOfType<Board>().GetComponent<Board>();

		playerInput.InputEnabled = true;
    }

    #region Mouvement Behaviour

    void Update()
    {
        if (playerMover.isMoving || m_gameManager.CurrentTurn != Turn.Player)
        {
            return;
        }

		playerInput.GetKeyInput();

		if (playerInput.V == 0)
        {
            if (playerInput.H < 0)
            {
                playerMover.MoveLeft();
            }
            else if (playerInput.H > 0)
            {
                playerMover.MoveRight();
            }
        }
        else if (playerInput.H == 0)
        {
            if (playerInput.V < 0)
            {
                playerMover.MoveBackward();
            }
            else if (playerInput.V > 0)
            {
                playerMover.MoveForward();
            }
        }
    }

    public void Die()
    {
        if (deathEvent != null)
        {
            deathEvent.Invoke();
        }
    }

    void CaptureEnemies()
    {
        if (m_board != null)
        {
            // Finds all enemies on the PlayerNode
            List<EnemyManager> enemies = m_board.FindEnemiesAt(m_board.PlayerNode);


            if (enemies.Count != 0)
            {
                foreach (EnemyManager enemy in enemies)
                {
                    if (enemy != null)
                    {
                        enemy.Die();
                    }
                }
            }
        }
    }

    public override void FinishTurn()
    {
        CaptureEnemies();

        // Tells the GameManager the PlayerTurn is complete
        base.FinishTurn();
    }
    #endregion

}
