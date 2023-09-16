using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LiteFsm.Example
{
    public sealed partial class Bot_SeparateStates : MonoBehaviour
    {
        [SerializeField] Food _food;
        [SerializeField] Home _home;

        [SerializeField] GameObject _foodAtHand;

        enum State { FindFood, GoHome, RunAway }

        State _backState;

        LiteFsm<State> _fsm;

        Vector3 _runAwayDirection;

        private void Awake()
        {
            _fsm = new LiteFsm<State>()
                .Add(State.FindFood, new FindFoodState())
                .Add(State.GoHome, new GoHomeState())
                .Add(State.RunAway, new RunAwayState());
        }

        private void OnEnable()
        {
            _fsm.Entry(State.FindFood);
        }

        private void OnDisable()
        {
            _fsm.Exit();
        }

        private void Update()
        {
            _fsm.Update(Time.deltaTime);
        }

        private bool IsNearMouse()
        {
            return Vector3.Distance(transform.position, GetMousePosition()) < 4f;
        }

        private Vector3 GetMousePosition()
        {
            var camera = Camera.main;
            var plane = new Plane(Vector3.up, Vector3.zero);
            var ray = camera.ScreenPointToRay(Input.mousePosition);
            plane.Raycast(ray, out var enter);
            return ray.GetPoint(enter);
        }

        private void LookAt(Vector3 destination)
        {
            transform.LookAt(destination);
        }

        private void MoveTo(Vector3 destination)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * 10f);
        }
    }


    public sealed partial class Bot_SeparateStates : MonoBehaviour
    {
        class StateBase : LiteFsm<State>.State
        {
            public Bot_SeparateStates bot { private set; get; }
            public Transform transform { private set; get; }

            protected void LookAt(Vector3 destination)
            {
                bot.LookAt(destination);
            }

            protected void MoveTo(Vector3 destination)
            {
                bot.MoveTo(destination);
            }

            protected bool IsNearMouse()
            {
                return bot.IsNearMouse();
            }
        }

        sealed class FindFoodState : StateBase
        {
            protected override void Update(float deltaTime)
            {
                if (IsNearMouse())
                {
                    bot._backState = attachedFsm.currentStateKey;
                    SetState(State.RunAway);
                }
                else if (Vector3.Distance(bot._food.transform.position, transform.position) < 1f)
                {
                    bot._foodAtHand.SetActive(true);
                    bot._food.Respawn();
                    SetState(State.GoHome);
                }
                else
                {
                    MoveTo(bot._food.transform.position);
                    LookAt(bot._food.transform.position);
                }
            }
        }

        sealed class GoHomeState : StateBase
        {
            protected override void Update(float deltaTime)
            {
                if (IsNearMouse())
                {
                    bot._backState = attachedFsm.currentStateKey;
                    SetState(State.RunAway);
                }
                else if (Vector3.Distance(transform.position, bot._home.transform.position) < 1f)
                {
                    bot._foodAtHand.SetActive(false);
                    SetState(State.FindFood);
                }
                else
                {
                    MoveTo(bot._home.transform.position);
                    LookAt(bot._home.transform.position);
                }
            }
        }

        sealed class RunAwayState : StateBase
        {
            protected override void Enter()
            {
                bot._runAwayDirection = (transform.position - bot.GetMousePosition()).normalized;
            }

            protected override void Update(float deltaTime)
            {
                MoveTo(transform.position + bot._runAwayDirection);
                LookAt(transform.position + bot._runAwayDirection);

                if (base.time > 2f && !IsNearMouse())
                {
                    SetState(bot._backState);
                }
            }
        }
    }
}