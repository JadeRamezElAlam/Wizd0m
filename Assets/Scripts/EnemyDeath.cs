using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    #region Variables
    public Vector3 offscreenOffset = new Vector3(0f, 10f, 0f);

    Board m_board;

    public float deathDelay = 0f;

    public float offscreenDelay = 1f;

    public float iTweenDelay = 0f;
    public iTween.EaseType easeType = iTween.EaseType.easeInOutQuint;

    public float moveTime = 0.5f;

    #endregion

    void Awake()
    {
        m_board = Object.FindObjectOfType<Board>().GetComponent<Board>();
    }

    // Moves the enemy to a target position, uses iTween to animate the movement and moves the enemy gameObject
    public void MoveOffBoard(Vector3 target)
    {
        iTween.MoveTo(gameObject, iTween.Hash(
            "x", target.x,
            "y", target.y,
            "z", target.z,
            "delay", iTweenDelay,
            "easetype", easeType,
            "time", moveTime
        ));
    }

	// Starts the enemy death coroutine
	public void Die()
    {
        StartCoroutine(DieRoutine());
    }

    IEnumerator DieRoutine()
    {
        // Waiits for a short delay before the enemy death
        yield return new WaitForSeconds(deathDelay);

        // Moves the enemy piece above the camera and off the board
        Vector3 offscreenPos = transform.position + offscreenOffset;

        MoveOffBoard(offscreenPos);

        // waits for the animation to finish and adds an extra delay
        yield return new WaitForSeconds(moveTime + offscreenDelay);
        
        if (m_board.capturePositions.Count != 0 
            && m_board.CurrentCapturePosition < m_board.capturePositions.Count)
        {
            // Selects the corresponding capture position
            Vector3 capturePos = m_board.capturePositions[m_board.CurrentCapturePosition].position;

            // Moves the enemy directly over the capture position
            transform.position = capturePos + offscreenOffset;

            // Drops the enemy down onto the capture position
            MoveOffBoard(capturePos);

            // Waits for the animation to finish
            yield return new WaitForSeconds(moveTime);

            // Incremenet the current index and verify the index is valid
            m_board.CurrentCapturePosition++;
            m_board.CurrentCapturePosition = 
                Mathf.Clamp(m_board.CurrentCapturePosition, 0, m_board.capturePositions.Count - 1);
        }
    }
}
