using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementType
{
    Stationary,
    Patrol,
    Spinner
}

public class EnemyMover : Mover
{
    #region Variables

    // Local direction to move (defaults to local positive z)
    public Vector3 directionToMove = new Vector3(0f, 0f, Board.spacing);

    // Movement mode
    public MovementType movementType = MovementType.Stationary;

    // Wait time for stationary enemies
    public float standTime = 1f;

    #endregion


    protected override void Awake()
    {
        base.Awake();

        // EnemyMovers always faces the direction they are moving
        faceDestination = true;
    }

    protected override void Start()
    {
        base.Start();
    }

    #region Enemy Behaviour

    // Complete one turn of movement
    public void MoveOneTurn()
    {
        switch (movementType)
        {
            case MovementType.Patrol:
                Patrol();
                break;
            case MovementType.Stationary:
				Stand();
                break;
            case MovementType.Spinner:
                Spin();
                break;
        }
    }

    void Patrol()
    {
        StartCoroutine(PatrolRoutine());
    }

    IEnumerator PatrolRoutine()
    {
        // Stores are starting transform position
        Vector3 startPos = new Vector3(m_currentNode.Coordinate.x, 0f, m_currentNode.Coordinate.y);

        // Moes one step forward
        Vector3 newDest = startPos + transform.TransformVector(directionToMove);

        // Moves two steps forward
        Vector3 nextDest = startPos + transform.TransformVector(directionToMove * 2f);

        // Moves to the Enemy new destination
        Move(newDest, 0f);

        // PAuse until we complete the movement
        while (isMoving)
        {
			yield return null; 
        }

        // Checks if we have reached a deadend
        if (m_board != null)
        {
            // Our destination Node
            Node newDestNode = m_board.FindNodeAt(newDest);

            // The node two spaces away
            Node nextDestNode = m_board.FindNodeAt(nextDest);

            // Checks the node two spaces away does exists OR is not connected to the enemy's destination board
            if (nextDestNode == null || !newDestNode.LinkedNodes.Contains(nextDestNode))
            {
                // Turns to face the enemy's original Node and sets that as our new destination
                destination = startPos;
                FaceDestination();

                yield return new WaitForSeconds(rotateTime);
            }
        }

		base.finishMovementEvent.Invoke();
    }

    void Stand()
    {
        StartCoroutine(StandRoutine());
    }

    IEnumerator StandRoutine()
    {
        yield return new WaitForSeconds(standTime);
        base.finishMovementEvent.Invoke();
    }

    void Spin()
    {
        StartCoroutine(SpinRoutine());
    }

    IEnumerator SpinRoutine()
    {
        Vector3 localForward = new Vector3(0f, 0f, Board.spacing);
        destination = transform.TransformVector(localForward * -1f) + transform.position;
        FaceDestination();

        yield return new WaitForSeconds(rotateTime);

		base.finishMovementEvent.Invoke();
    }

    #endregion
}
