using UnityEngine;
using Stage;   // 위 네임스페이스

[RequireComponent(typeof(Collider2D))]
public class Lv3NumberButton : MonoBehaviour
{
    [SerializeField] int number = 4;   // 이 오브젝트가 담당하는 숫자
    [SerializeField] PlayTheSongLv3Stage stage;

    void Reset()
    {
        stage = FindObjectOfType<PlayTheSongLv3Stage>();
    }

    void OnMouseDown()
    {
        if (stage != null)
            stage.OnNumberButtonPressed(number);
    }
}
