using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PoolManager : Singleton<PoolManager>
{
    [SerializeField] private GameObject displayTextPrefab;
    
    private Dictionary<GameObject, Queue<GameObject>> enemyPools = new Dictionary<GameObject, Queue<GameObject>>();
    private Queue<GameObject> displayTextPool = new Queue<GameObject>();
    private Queue<GameObject> enemyPool = new Queue<GameObject>();
    
    public GameObject GetDisplayText()
    {
        if (displayTextPool.Count == 0)
        {
            GameObject displayText = Instantiate(displayTextPrefab);
            displayText.SetActive(false);
            displayTextPool.Enqueue(displayText);
        }

        GameObject displayTextFromPool = displayTextPool.Dequeue();
        displayTextFromPool.SetActive(true);
        return displayTextFromPool;
    }

    public void ReturnDisplayText(GameObject displayText)
    {
        displayText.SetActive(false);
        displayTextPool.Enqueue(displayText);
    }

    public GameObject GetEnemy(GameObject enemyPrefab, Vector3 position)
    {
        if (!enemyPools.ContainsKey(enemyPrefab))
        {
            enemyPools[enemyPrefab] = new Queue<GameObject>();
        }

        Queue<GameObject> pool = enemyPools[enemyPrefab];
        
        if (pool.Count == 0)
        {
            GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
            enemy.SetActive(false);
            pool.Enqueue(enemy);
        }
        
        GameObject enemyFromPool = pool.Dequeue();
        enemyFromPool.transform.position = position;
        enemyFromPool.SetActive(true);

        return enemyFromPool;
    }

    public void ReturnEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
        enemyPool.Enqueue(enemy);
    }
}
