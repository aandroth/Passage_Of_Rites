using UnityEngine;
using BehaviorTree;

public class TaskPatrol : Node
{
    public Transform _transform;
    public Transform[] _waypoints;

    private int _currWaypointIndex = 0;

    public float _waitTime = 0.25f;
    private float _waitCounter = 0f;
    private bool  _waiting = false;


    public TaskPatrol(Transform transform, Transform[] waypoints)
    {
        _transform = transform;
        _waypoints = waypoints;
    }

    public override NodeState Evaluate()
    {
        if (_waiting)
        {
            _waitCounter += Time.deltaTime;
            if(_waitCounter >= _waitTime)
            {
                _waiting = false;
            }
        }
        else
        {
            Transform wp = _waypoints[_currWaypointIndex];
            if (Vector3.Distance(_transform.position, wp.position) < 0.01f)
            {
                _transform.position = wp.position;
                _waitCounter = 0f;
                _waiting = true;

                _currWaypointIndex = (_currWaypointIndex+1) %_waypoints.Length;
            }
            else
            {
                _transform.position = Vector3.MoveTowards(_transform.position, wp.position, GuardBT.speed * Time.deltaTime);
            }
        }

        state = NodeState.RUNNING;
        return state;
    }
}
