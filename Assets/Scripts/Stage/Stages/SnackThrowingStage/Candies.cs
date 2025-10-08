using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candies : MonoBehaviour
{
    [SerializeField] private GameObject _purpleCandy;
    [SerializeField] private GameObject _redCandy;
    [SerializeField] private GameObject _yellowCandy;
    private int _sortingOrderIndex = 6;

    private Stack<GameObject> _candiesStack = new Stack<GameObject>();

    private void PushCandyToStack(GameObject candy)
    {
        GameObject candyInstance = Instantiate(candy);
        candyInstance.GetComponent<SpriteRenderer>().sortingOrder = _sortingOrderIndex++;
        candyInstance.transform.SetParent(transform);
        candyInstance.transform.localPosition = Vector3.zero;
        _candiesStack.Push(candyInstance);
    }

    public IEnumerator ThrowCandy(Collider2D student)
    {
        float duration = 0.5f; // 이동 시간

        if (_candiesStack.Count > 0)
        {
            GameObject candy = _candiesStack.Pop();
            candy.transform.SetParent(null);

            Vector3 startPosition = candy.transform.position;
            float elapsedTime = 0;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;

                candy.transform.position = Vector3.Slerp(startPosition, student.transform.position, t);

                yield return null; // 다음 프레임까지 대기
            }
            candy.transform.SetParent(student.transform);
            candy.transform.localPosition = new Vector3(0, 5f, 0);

            student.GetComponent<SnackDetector>().GetCandy();

            yield return new WaitForSeconds(1f);

            candy.SetActive(false);
        }
        else
        {
            Debug.LogWarning("No candies left to throw!");
        }
    }

    // ============ Lifecycle methods ============ //
    void Start()
    {
        List<GameObject> candiesList = new List<GameObject> { _purpleCandy, _redCandy, _yellowCandy };

        // 리스트를 랜덤하게 섞음
        for (int i = 0; i < candiesList.Count; i++)
        {
            int randomIndex = Random.Range(0, candiesList.Count);
            GameObject temp = candiesList[i];
            candiesList[i] = candiesList[randomIndex];
            candiesList[randomIndex] = temp;
        }

        // 섞인 순서대로 스택에 Push
        foreach (GameObject candy in candiesList)
        {
            PushCandyToStack(candy);
        }
    }

    void Update()
    {
        
    }
}
