using Behaviour_Tree.Blackboard;
using Behaviour_Tree.Nodes;
using UnityEngine;

public class SearchArea : NodeLeaf
{
    private Vector3? _searchCenter;
    private Vector3[] _searchPoints = new Vector3[3];
    private int _currentSearchPointIndex = -1;
    private bool _isLookingLeft = false;
    private float _lookTimer = 0f;
    
    public SearchArea(Blackboard blackboard, GameObject enemy) : base(blackboard, enemy) { }

    public override NodeReturnType Execute()
    {
        // 1. If this is the first call, set up the search
        if (_currentSearchPointIndex == -1)
        {
            SetupSearchPoints();
            _currentSearchPointIndex = 0;
            return NodeReturnType.Running;
        }
        
        // 2. Get the current search point we're investigating
        Vector3 currentTarget = _searchPoints[_currentSearchPointIndex];
        
        // 3. Move toward the current search point if not there yet
        float distance = Vector3.Distance(_gameObject.transform.position, currentTarget);
        
        if (distance > 5f)
        {
            // Move toward search point
            _gameObject.transform.position = Vector3.MoveTowards(
                _gameObject.transform.position,
                currentTarget,
                Time.deltaTime * 1.5f
            );
            
            _gameObject.transform.LookAt(currentTarget);
            return NodeReturnType.Running;
        }
        
        // 4. We've reached the search point, now look around
        if (!_isLookingLeft)
        {
            // Look left
            _lookTimer += Time.deltaTime;
            if (_lookTimer < 1f)
            {
                _gameObject.transform.Rotate(0, -45f * Random.Range(1f, 3f) * Time.deltaTime, 0);
            }
            else
            {
                _isLookingLeft = true;
                _lookTimer = 0f;
            }

            return NodeReturnType.Running;
        }

        // Look right
        _lookTimer += Time.deltaTime;
        if (_lookTimer < 1f)
        {
            _gameObject.transform.Rotate(0, 45f * Random.Range(1f, 3f) * Time.deltaTime, 0);
        }
        else
        {
            // Finished looking at this point
            _isLookingLeft = false;
            _lookTimer = 0f;
            _currentSearchPointIndex++;
                
            // Check if we've searched all points
            if (_currentSearchPointIndex < _searchPoints.Length) return NodeReturnType.Running;
            
            ResetSearch();
            return NodeReturnType.Success;
        }

        return NodeReturnType.Running;
    }
    
    private void SetupSearchPoints()
    {
        _searchCenter = _gameObject.transform.position;
        
        // Generate 3 random search points around the center
        for (int i = 0; i < _searchPoints.Length; i++)
        {
            // Random point within 3-5 units of center
            float angle = Random.Range(0f, 360f);
            float distance = Random.Range(3f, 5f);
            
            Vector3 randomDirection = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            _searchPoints[i] = _searchCenter.Value + randomDirection * distance;
            
            // Optional: Raycast to ensure point is reachable/not inside walls
            // (Add if your environment has obstacles)
        }
    }
    
    private void ResetSearch()
    {
        _currentSearchPointIndex = -1;
        _isLookingLeft = false;
        _lookTimer = 0f;
        _searchCenter = null;
    }
    
    // Optional: Visualize search points in editor
    private void OnDrawGizmos()
    {
        if (!_searchCenter.HasValue || _currentSearchPointIndex < 0) return;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_searchCenter.Value, 1f);
            
        for (int i = 0; i < _searchPoints.Length; i++)
        {
            Gizmos.color = i == _currentSearchPointIndex ? Color.red : Color.green;
            Gizmos.DrawSphere(_searchPoints[i], 0.3f);
            Gizmos.DrawLine(_searchCenter.Value, _searchPoints[i]);
        }
    }
}