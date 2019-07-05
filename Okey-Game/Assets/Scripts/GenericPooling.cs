using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericPooling<T> : MonoBehaviour where T : Component
{
    public static event Action PoolInitialized;

    [SerializeField] private T _prefab;
    [SerializeField] private Transform _prefabContainerTransform;

    public static GenericPooling<T> Instance { get; private set; }
    private Queue<T> objects = new Queue<T>();

    private void Awake()
    {
        Instance = this;

        //do any initialization before here
        if(PoolInitialized != null)
        {
            Debug.Log("Pool Initialized");
            PoolInitialized.Invoke();
        }
    }    

    // use this to get object from pool
    public T Get()
    {
        if(objects.Count == 0)
        {
            AddObjects(1);
        }

        return objects.Dequeue();
    }

    //returning objects to pool so we can re-use them
    public void ReturnToPool(T objectToReturn)
    {
        objectToReturn.gameObject.SetActive(false);
        objects.Enqueue(objectToReturn);
    }

    //instantiating objects and adding them to pool
    private void AddObjects(int count)
    {
        for(int i = 0; i < count; i++)
        {
            var newObject = Instantiate(_prefab, _prefabContainerTransform);
            newObject.gameObject.SetActive(false);
            objects.Enqueue(newObject);
        }
    }
}
