using System;
using System.Collections.Generic;
using UnityEngine;

//Come back to this one day if you want to make an intense level blackboard

namespace Behaviour_Tree.Blackboard
{
    public enum BlackboardScope
    {
        Entity,     // Per-game object (enemy, player)
        Global      // Game-wide
    }

    /// <summary>
    /// Central manager for all blackboards in the game.
    /// Supports entity-specific, and global blackboards.
    /// </summary>
    public class BlackboardManager : MonoBehaviour
    {
        #region Singleton Pattern
        private static BlackboardManager _instance;
        public static BlackboardManager Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = FindObjectOfType<BlackboardManager>();
                    if (!_instance)
                    {
                        GameObject go = new GameObject("BlackboardManager");
                        _instance = go.AddComponent<BlackboardManager>();
                    }
                }
                return _instance;
            }
        }
        #endregion

        // Entity-level blackboards (by GameObject instance ID)
        private Dictionary<int, Blackboard> _entityBlackboards = new();
    
        // Global blackboard (shared by everything)
        private Blackboard _globalBlackboard = new();
    
        // Optional: Cache for fast GameObject->Blackboard lookups
        private Dictionary<GameObject, int> _gameObjectIdCache = new();

        // Events for observation
        public event Action<BlackboardScope, string, string, object> OnBlackboardValueChanged;
        public event Action<GameObject, Blackboard> OnEntityBlackboardRegistered;

        #region Public API - Entity Blackboards

        /// <summary>
        /// Register a GameObject with its blackboard
        /// </summary>
        public void RegisterEntity(GameObject entity, Blackboard blackboard)
        {
            int id = entity.GetInstanceID();
            _entityBlackboards[id] = blackboard;
            _gameObjectIdCache[entity] = id;
        
            OnEntityBlackboardRegistered?.Invoke(entity, blackboard);
        
            Debug.Log($"Registered entity: {entity.name} (ID: {id})");
        }

        /// <summary>
        /// Get an entity's blackboard
        /// </summary>
        public Blackboard GetEntityBlackboard(GameObject entity)
        {
            if (_gameObjectIdCache.TryGetValue(entity, out int id))
            {
                if (_entityBlackboards.TryGetValue(id, out Blackboard bb))
                    return bb;
            }
            return null;
        }

        /// <summary>
        /// Check if an entity has a blackboard
        /// </summary>
        public bool HasEntityBlackboard(GameObject entity)
        {
            return _gameObjectIdCache.ContainsKey(entity);
        }

        /// <summary>
        /// Remove an entity's blackboard (when destroyed)
        /// </summary>
        public void UnregisterEntity(GameObject entity)
        {
            if (_gameObjectIdCache.TryGetValue(entity, out int id))
            {
                _entityBlackboards.Remove(id);
                _gameObjectIdCache.Remove(entity);
            }
        }

        #endregion
        

        #region Public API - Global Blackboard

        /// <summary>
        /// Get the global blackboard (shared by all)
        /// </summary>
        public Blackboard GetGlobalBlackboard()
        {
            return _globalBlackboard;
        }

        #endregion

        #region Unified Access Methods

        /// <summary>
        /// Set a value in any blackboard scope
        /// </summary>
        public void SetValue<T>(BlackboardScope scope, string identifier, string key, T value)
        {
            Blackboard targetBb = GetBlackboardByScope(scope, identifier);
            if (targetBb == null) return;

            BlackboardKey bbKey = targetBb.GetOrRegisterKey(key);
            targetBb.SetValue(bbKey, value);
        
            // Notify listeners
            OnBlackboardValueChanged?.Invoke(scope, identifier, key, value);
        }

        /// <summary>
        /// Get a value from any blackboard scope
        /// </summary>
        public bool TryGetValue<T>(BlackboardScope scope, string identifier, string key, out T value)
        {
            Blackboard targetBb = GetBlackboardByScope(scope, identifier);
            if (targetBb != null)
            {
                BlackboardKey bbKey = targetBb.GetOrRegisterKey(key);
                return targetBb.TryGetValue(bbKey, out value);
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Get the appropriate blackboard based on scope
        /// </summary>
        private Blackboard GetBlackboardByScope(BlackboardScope scope, string identifier)
        {
            return scope switch
            {
                BlackboardScope.Entity => GetEntityBlackboardByIdentifier(identifier),
                BlackboardScope.Global => _globalBlackboard,
                _ => null
            };
        }

        private Blackboard GetEntityBlackboardByIdentifier(string identifier)
        {
            // Identifier can be GameObject name or instance ID
            if (int.TryParse(identifier, out int id))
            {
                if (_entityBlackboards.TryGetValue(id, out Blackboard bb))
                    return bb;
            }
        
            // Fallback: search by GameObject name (slower)
            foreach (var kvp in _entityBlackboards)
            {
                // You'd need a way to map ID back to GameObject
                // Consider adding a reverse dictionary if needed
            }
        
            return null;
        }

        #endregion

        #region Bulk Operations & Queries

        /// <summary>
        /// Find all entities with a specific blackboard value
        /// </summary>
        public List<GameObject> FindEntitiesWithValue<T>(string key, T value)
        {
            List<GameObject> result = new();
        
            foreach (var kvp in _entityBlackboards)
            {
                int entityId = kvp.Key;
                Blackboard bb = kvp.Value;
            
                BlackboardKey bbKey = bb.GetOrRegisterKey(key);
                if (bb.TryGetValue(bbKey, out T storedValue) && storedValue.Equals(value))
                {
                    // Find GameObject from ID
                    foreach (var cache in _gameObjectIdCache)
                    {
                        if (cache.Value == entityId)
                        {
                            result.Add(cache.Key);
                            break;
                        }
                    }
                }
            }
        
            return result;
        }

        /// <summary>
        /// Broadcast a value to multiple entities
        /// </summary>
        public void BroadcastToEntities(List<GameObject> entities, string key, object value)
        {
            foreach (GameObject entity in entities)
            {
                Blackboard bb = GetEntityBlackboard(entity);
                if (bb != null)
                {
                    BlackboardKey bbKey = bb.GetOrRegisterKey(key);
                    bb.SetValue(bbKey, value);
                }
            }
        }

        /// <summary>
        /// Copy values from one blackboard to another
        /// </summary>
        public void CopyBlackboardValues(Blackboard source, Blackboard destination, params string[] keys)
        {
            foreach (string key in keys)
            {
                BlackboardKey bbKey = source.GetOrRegisterKey(key);
            
                // This is tricky because we don't know the type
                // You might need reflection or a different approach
            }
        }

        #endregion

        #region Debug & Inspection

        /// <summary>
        /// Draw all blackboard contents in the console
        /// </summary>
        public void DebugAllBlackboards()
        {
            Debug.Log("=== GLOBAL BLACKBOARD ===");
            _globalBlackboard.Debug();

            Debug.Log($"=== ENTITY BLACKBOARDS ({_entityBlackboards.Count}) ===");
            foreach (var kvp in _entityBlackboards)
            {
                Debug.Log($"Entity ID: {kvp.Key}");
                kvp.Value.Debug();
            }
        }

        /// <summary>
        /// Get statistics about blackboard usage
        /// </summary>
        public void GetStatistics(out int entityCount, out int totalEntries)
        {
            entityCount = _entityBlackboards.Count;
        
            totalEntries = 0;
            foreach (var bb in _entityBlackboards.Values)
            {
                // Count entries - you'd need to expose this from Blackboard class
            }
        }

        #endregion

        #region MonoBehaviour Lifecycle

        void Awake()
        {
            if (_instance && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
        
            _instance = this;
            DontDestroyOnLoad(gameObject);
        
            Debug.Log("BlackboardManager initialized");
        }

        void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        #endregion
    }
}