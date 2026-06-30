using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Habillage
{
    public class CharacterMovement : MonoBehaviour
    {
        public float moveSpeed = 2f;           // Speed at which the character moves
        public float changeDirectionTime = 2f; // Time interval to change direction
        public float idleTime = 1f;            // Time to stay idle before changing direction

        private float timer = 0f;
        private int distance = 1;
        private enum State { Walking, Idle }
        private State currentState = State.Idle;
        private bool TurningLeft;

        public Vector2 clamp = new Vector2(-4, 4);

        void Start()
        {

        }

        private void OnEnable()
        {
            ChangeState(State.Idle);
        }

        void Update()
        {
            timer += Time.deltaTime;

            if (currentState == State.Walking)
            {
                // Move the character
                transform.Translate(Vector2.left * distance * moveSpeed * Time.deltaTime);

                // Change state after the specified time
                if (timer >= changeDirectionTime)
                {
                    ChangeState(State.Idle);
                }
            }
            else if (currentState == State.Idle)
            {
                // Stay idle for a specific time
                if (timer >= idleTime)
                {
                    idleTime = Random.Range(0, 3);
                    ChangeState(State.Walking);
                }
            }
        }

        private void LateUpdate()
        {
            var _pos = transform.localPosition;
            _pos.x = Mathf.Clamp(_pos.x, clamp.x, clamp.y);

            transform.localPosition = _pos;
        }

        void ChangeState(State newState)
        {
            currentState = newState;
            timer = 0f;

            if (currentState == State.Walking)
            {
                distance = Random.Range(0, 3);
                int turn = Random.Range(0, 2);
                // Randomly choose a new turn: -1 (turn) or 1 (not turn)
                if (turn > 0)
                {
                    LeanTween.rotateY(gameObject, TurningLeft ? 0 : 180, 0.25f).setEaseInOutCubic();
                    TurningLeft = !TurningLeft;
                }
            }
        }
    }
}