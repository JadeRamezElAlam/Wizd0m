using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Linq;

[System.Serializable]
public enum Turn
{
    Player,
    Enemy
}

public class GameManager : MonoBehaviour
{
    #region Variables

    Board m_board;

    PlayerManager m_player;

    List<EnemyManager> m_enemies;

    Turn m_currentTurn = Turn.Player;
    public Turn CurrentTurn { get { return m_currentTurn; } }

    bool m_hasLevelStarted = false;
    public bool HasLevelStarted { get { return m_hasLevelStarted; } set { m_hasLevelStarted = value; } }

    bool m_isGamePlaying = false;
    public bool IsGamePlaying { get { return m_isGamePlaying; } set { m_isGamePlaying = value; } }

    bool m_isGameOver = false;
    public bool IsGameOver { get { return m_isGameOver; } set { m_isGameOver = value; } }

    bool m_hasLevelFinished = false;
    public bool HasLevelFinished { get { return m_hasLevelFinished; } set { m_hasLevelFinished = value; } }

    public float delay = 1f;

    public UnityEvent setupEvent;
    public UnityEvent startLevelEvent;
    public UnityEvent playLevelEvent;
    public UnityEvent endLevelEvent;
    public UnityEvent loseLevelEvent;

    #endregion


    void Awake()
    {
        // Populates the Board, Player and Enemy components to the scene when loaded
        m_board = Object.FindObjectOfType<Board>().GetComponent<Board>();
        m_player = Object.FindObjectOfType<PlayerManager>().GetComponent<PlayerManager>();
        m_enemies = (Object.FindObjectsOfType<EnemyManager>() as EnemyManager[]).ToList();
    }

    void Start()
    {
        // Starts the main game loop when the player and board are present
        if (m_player != null && m_board != null)
        {
            StartCoroutine("RunGameLoop");
        }
        else
        {
            Debug.LogWarning("GAMEMANAGER Error: no player or board found!");
        }
    }

    // Runs the main game loop, separated into different coroutine
    IEnumerator RunGameLoop()
    {
        yield return StartCoroutine("StartLevelRoutine");
        yield return StartCoroutine("PlayLevelRoutine");
        yield return StartCoroutine("EndLevelRoutine");
    }

    #region Coroutines

    // Initial stage after the level is loaded
    IEnumerator StartLevelRoutine()
    {
        Debug.Log("SETUP LEVEL");
        if (setupEvent != null)
        {
            setupEvent.Invoke();
        }

        Debug.Log("START LEVEL");
        m_player.playerInput.InputEnabled = false;

        while (!m_hasLevelStarted)
        {
            yield return null;
        }

        // Triggers events when player presses the StartButton
        if (startLevelEvent != null)
        {
            startLevelEvent.Invoke();
        }
    }

    IEnumerator PlayLevelRoutine()
    {
        Debug.Log("PLAY LEVEL");
        m_isGamePlaying = true;
        yield return new WaitForSeconds(delay);
        m_player.playerInput.InputEnabled = true;

        // Triggers any events as we start playing the level
        if (playLevelEvent != null)
        {
            playLevelEvent.Invoke();
        }

        while (!m_isGameOver)
        {

            yield return null;

            m_isGameOver = IsWinner();

        }
    }

    public void LoseLevel()
    {
        StartCoroutine(LoseLevelRoutine());
    }

    //Lose level condition
    IEnumerator LoseLevelRoutine()
    {
        m_isGameOver = true;
        yield return new WaitForSeconds(1.5f);

        if (loseLevelEvent != null)
        {
            loseLevelEvent.Invoke();
        }
        yield return new WaitForSeconds(2f);

        Debug.Log("LOSE! =============================");

        RestartLevel();
    }

    IEnumerator EndLevelRoutine()
    {
        Debug.Log("END LEVEL");
        m_player.playerInput.InputEnabled = false;

        if (endLevelEvent != null)
        {
            endLevelEvent.Invoke();
        }

        // Shows end screen
        while (!m_hasLevelFinished)
        {
            yield return null;
        }
        RestartLevel();
    }

    #endregion

    // Restarts current level
    void RestartLevel()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    // Attaches to StartButton, triggers PlayLevelRoutine
    public void PlayLevel()
    {
        m_hasLevelStarted = true;
    }

    // Checks if player reache the goal node
    bool IsWinner()
    {
        if (m_board.PlayerNode != null)
        {
            return (m_board.PlayerNode == m_board.GoalNode);
        }
        return false;
    }

    #region Turn-Based functions

    // Switches to player's turn
    void PlayPlayerTurn()
    {
        m_currentTurn = Turn.Player;
        m_player.IsTurnComplete = false;

        // Allows player to move
    }

    // Switches to enemy's turn
    void PlayEnemyTurn()
    {
        m_currentTurn = Turn.Enemy;

        foreach (EnemyManager enemy in m_enemies)
        {
            if (enemy != null && !enemy.IsDead)
            {
                enemy.IsTurnComplete = false;

                enemy.PlayTurn();
            }
        }
    }

    bool IsEnemyTurnComplete()
    {
        foreach (EnemyManager enemy in m_enemies)
        {
            if (enemy.IsDead)
            {
                continue;
            }

            if (!enemy.IsTurnComplete)
            {
                return false;
            }
        }

        return true;
    }

    bool AreEnemiesAllDead()
    {
        foreach (EnemyManager enemy in m_enemies)
        {
            if (!enemy.IsDead)
            {
                return false;
            }
        }
        return true;
    }

    public void UpdateTurn()
    {
        if (m_currentTurn == Turn.Player && m_player != null)
        {
            if (m_player.IsTurnComplete && !AreEnemiesAllDead())
            {
                PlayEnemyTurn();
            }

        }
        else if (m_currentTurn == Turn.Enemy)
        {
            if (IsEnemyTurnComplete())
            {
				PlayPlayerTurn(); 
            }
        }
    }
    #endregion
}
