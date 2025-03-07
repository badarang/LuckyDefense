using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PoolManager : Singleton<PoolManager>
{
    [SerializeField] private GameObject displayTextPrefab;
    
    private Queue<GameObject> _displayTextPool = new Queue<GameObject>();
    private Queue<GameObject> _enemyPool = new Queue<GameObject>();
    
    public GameObject GetDisplayText()
    {
        if (_displayTextPool.Count == 0)
        {
            GameObject displayText = Instantiate(displayTextPrefab);
            displayText.SetActive(false);
            _displayTextPool.Enqueue(displayText);
        }

        GameObject displayTextFromPool = _displayTextPool.Dequeue();
        displayTextFromPool.SetActive(true);
        return displayTextFromPool;
    }

    public void ReturnDisplayText(GameObject displayText)
    {
        displayText.SetActive(false);
        _displayTextPool.Enqueue(displayText);
    }

    public GameObject GetEnemy(GameObject enemyPrefab, Vector3 position)
    {
        if (_enemyPool.Count == 0)
        {
            GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
            enemy.SetActive(false);
            _enemyPool.Enqueue(enemy);
        }

        GameObject enemyFromPool = _enemyPool.Dequeue();
        enemyFromPool.transform.position = position;
        enemyFromPool.SetActive(true);
        return enemyFromPool;
    }

    public void ReturnEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
        _enemyPool.Enqueue(enemy);
    }
}
