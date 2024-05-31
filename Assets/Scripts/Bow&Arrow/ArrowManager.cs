using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ArrowManager : MonoBehaviour
{
    public static ArrowManager Instance { get; private set; }

    public GameObject arrowPrefab;
    public int maxArrows = 20;
    public int maxVisibleArrows = 10;

    private int visibleArrowNum;

    private Queue<GameObject> arrowPool = new Queue<GameObject>();
    private Queue<GameObject> visibleArrowPool = new Queue<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        visibleArrowNum = 0;
        Debug.Log("visibleArrowNum 초기화됨: " + visibleArrowNum);

        // 화살 풀 생성 및 초기화
        for (int i = 0; i < maxArrows; i++)
        {
            GameObject arrow = Instantiate(arrowPrefab);
            arrow.SetActive(false);
            arrowPool.Enqueue(arrow);
        }
    }

    private void Update()
    {
        if (visibleArrowNum >= maxVisibleArrows)
        {
            HideArrow();
        }
    }

    public GameObject GetArrowFromPool()
    {
        if (arrowPool.Count > 0)
        {
            GameObject arrow = arrowPool.Dequeue();
            arrow.SetActive(true);
            return arrow;
        }
        else
        {
            Debug.LogWarning("No arrows left in the pool!");
            return null;
        }
    }

    public void AddVisibleArrow(GameObject arrow)
    {
        visibleArrowPool.Enqueue(arrow);
        visibleArrowNum += 1;
        Debug.Log("OK! count: " + visibleArrowNum);
    }

    private void HideArrow()
    {
        GameObject arrow = visibleArrowPool.Dequeue();
        arrow.SetActive(false);
        arrowPool.Enqueue(arrow);
        visibleArrowNum -= 1;
    }
}
