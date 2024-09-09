using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : MonoBehaviour
{
    public float speed = 5f;          // 슬래시의 이동 속도
    public float maxLength = 5f;     // 슬래시의 최대 길이
    public float duration = 1f;       // 슬래시의 총 지속 시간
    public float flickerInterval = 0.1f; // 반짝이는 간격

    void Start()
    {
        StartCoroutine(SlashRoutine(GameObject.FindWithTag("Boss").transform, 10f));
    }

    public IEnumerator SlashRoutine(Transform playerTransform, float distance)
    {
        // 슬래시의 초기 설정
        Vector3 startPosition = transform.position;
        Vector3 direction;
        if (playerTransform.localScale.x == -1)
            direction = Vector3.right;
        else
            direction = Vector3.left;
        Vector3 targetPosition = startPosition + direction * distance;

        float halfDuration = duration / 2f;
        float elapsedTime = 0f;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        while (elapsedTime < duration)
        {
            // 슬래시의 길이를 증가
            float currentLength;
            if (elapsedTime < halfDuration)
            {
                currentLength = Mathf.Lerp(0.1f, maxLength, elapsedTime / halfDuration);
            }
            else
            {
                currentLength = Mathf.Lerp(maxLength, 0.1f, (elapsedTime - halfDuration) / halfDuration);
            }
            transform.localScale = new Vector3(transform.localScale.x, currentLength, transform.localScale.z);

            // 슬래시를 x축 방향으로 이동
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

            // 반짝거리는 효과
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, Mathf.PingPong(elapsedTime / flickerInterval, 1f));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 슬래시 종료 시 오브젝트 제거 또는 비활성화
        Destroy(gameObject);
    }
}
