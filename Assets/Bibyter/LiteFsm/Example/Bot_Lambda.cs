using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LiteFsm.Example
{
    public sealed class Bot_Lambda : MonoBehaviour
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
                .AddActions(State.FindFood,
                    update: () =>
                    {
                        if (IsNearMouse())
                        {
                            _backState = _fsm.currentStateKey;
                            _fsm.SetState(State.RunAway);
                        }
                        else if (Vector3.Distance(_food.transform.position, transform.position) < 1f)
                        {
                            _foodAtHand.SetActive(true);
                            _food.Respawn();
                            _fsm.SetState(State.GoHome);
                        }
                        else
                        {
                            MoveTo(_food.transform.position);
                            LookAt(_food.transform.position);
                        }
                    })
                .AddActions(State.GoHome, 
                    update: () =>
                    {
                        if (IsNearMouse())
                        {
                            _backState = _fsm.currentStateKey;
                            _fsm.SetState(State.RunAway);
                        }
                        else if (Vector3.Distance(transform.position, _home.transform.position) < 1f)
                        {
                            _foodAtHand.SetActive(false);
                            _fsm.SetState(State.FindFood);
                        }
                        else
                        {
                            MoveTo(_home.transform.position);
                            LookAt(_home.transform.position);
                        }
                    })
                .AddActions(State.RunAway, 
                    enter: () =>
                    {
                        _runAwayDirection = (transform.position - GetMousePosition()).normalized;
                    },
                    update: () =>
                    {
                        MoveTo(transform.position + _runAwayDirection);
                        LookAt(transform.position + _runAwayDirection);

                        if (_fsm.stateTime > 2f && !IsNearMouse())
                        {
                            _fsm.SetState(_backState);
                        }
                    });
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

}