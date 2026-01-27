using System;
using System.Collections.Generic;
using System.Reflection;

namespace Behaviour_Tree.Blackboard
{
    // Thanks git-amend
// https://www.youtube.com/watch?v=HNGJ8KOqdYQ
// https://github.com/adammyhre/Unity-Behaviour-Trees/tree/master

    /// <summary>
    /// IEquatable : Defines a generalized method that a value type or class implements to create a type-specific method
    /// for determining equality of instances.
    /// This is a way for us to have a generic type for the dictionary so that we can have any type for the key value pair
    /// </summary>
    [Serializable]
    public readonly struct BlackboardKey : IEquatable<BlackboardKey> 
    {
        private readonly string _name;
        private readonly int _hashedKey;

        public BlackboardKey(string name) {
            this._name = name;
            _hashedKey = name.ComputeFNV1aHash();
        }
        
        public bool Equals(BlackboardKey other) => _hashedKey == other._hashedKey;
        
        public override bool Equals(object obj) => obj is BlackboardKey other && Equals(other);
        public override int GetHashCode() => _hashedKey;
        public override string ToString() => _name;
        
        public static bool operator ==(BlackboardKey lhs, BlackboardKey rhs) => lhs._hashedKey == rhs._hashedKey;
        public static bool operator !=(BlackboardKey lhs, BlackboardKey rhs) => !(lhs == rhs);
    }

    /// <summary>
    /// Wrapper class for all Values that go into the blackboard dictionary.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class BlackboardEntry<T> 
    {
        public BlackboardKey Key { get; }
        public T Value { get; }
        public Type ValueType { get; }

        public BlackboardEntry(BlackboardKey key, T value) 
        {
            Key = key;
            Value = value;
            ValueType = typeof(T);
        }
        
        public override bool Equals(object obj) => obj is BlackboardEntry<T> other && other.Key == Key;
        public override int GetHashCode() => Key.GetHashCode();
    }
    
    /// <summary>
    /// Blackboards are key value pairs of information commonly accessible throughout the tree.
    /// </summary>
    [Serializable]
    public class Blackboard 
    {
        private Dictionary<string, BlackboardKey> _keyRegistry = new();
        private Dictionary<BlackboardKey, object> _entries = new();
        
        public List<Action> PassedActions { get; } = new();

        public void AddAction(Action action) 
        {
            PassedActions.Add(action);
        }
        
        public void ClearActions() => PassedActions.Clear();

        public void Debug() 
        {
            foreach (KeyValuePair<BlackboardKey, object> entry in _entries) 
            {
                Type entryType = entry.Value.GetType();

                if (entryType.IsGenericType && entryType.GetGenericTypeDefinition() == typeof(BlackboardEntry<>)) 
                {
                    PropertyInfo valueProperty = entryType.GetProperty("Value");
                    if (valueProperty == null) continue;
                    object value = valueProperty.GetValue(entry.Value);
                    UnityEngine.Debug.Log($"Key: {entry.Key}, Value: {value}");
                }
            }
        }

        /// <summary>
        /// Try and get a value out of the Dictionary from a key
        /// </summary>
        public bool TryGetValue<T>(BlackboardKey key, out T value) 
        {
            if (_entries.TryGetValue(key, out object entry) && entry is BlackboardEntry<T> castedEntry) 
            {
                value = castedEntry.Value;
                return true;
            }
            
            value = default;
            return false;
        }
        
        public void SetValue<T>(BlackboardKey key, T value) 
        {
            _entries[key] = new BlackboardEntry<T>(key, value);
        }

        /// <summary>
        /// Checks if there are any keys in the blackboard with the given key name, if not it creates a key with it
        /// The key has NO VALUE associated to it
        /// </summary>
        /// <param name="keyName"></param>
        public BlackboardKey GetOrRegisterKey(string keyName)
        {
            if (!_keyRegistry.TryGetValue(keyName, out BlackboardKey key)) 
            {
                key = new BlackboardKey(keyName);
                _keyRegistry[keyName] = key;
            }
            
            return key;
        }
        
        public bool ContainsKey(BlackboardKey key) => _entries.ContainsKey(key);
        
        public void Remove(BlackboardKey key) => _entries.Remove(key);
    }
}