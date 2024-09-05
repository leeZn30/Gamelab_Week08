using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpSlash : MonoBehaviour
{
    public float targetHeight = 5f;  // 목표 높이
    public float duration = 1f;      // 길어지고 알파가 변하는 데 걸리는 시간

    private SpriteRenderer objectRenderer;
    private Vector3 initialScale;

    void Start()
    {
        // 초기 스케일과 렌더러 컴포넌트 저장
        initialScale = transform.localScale;
        objectRenderer = GetComponent<SpriteRenderer>();

        // 코루틴 시작
        StartCoroutine(ScaleAndFadeCoroutine());
    }

    private IEnumerator ScaleAndFadeCoroutine()
    {
        float elapsedTime = 0f;
        Color initialColor = objectRenderer.color;
        Color targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 1f);  // 알파 값이 1인 목표 색상

        while (elapsedTime < duration)
        {
            // 높이를 점진적으로 변경
            float newHeight = Mathf.Lerp(initialScale.y, targetHeight, elapsedTime / duration);
            transform.localScale = new Vector3(initialScale.x, newHeight, initialScale.z);

            if (elapsedTime < duration * 0.8f)
            {
                // 알파 값을 점진적으로 변경
                Color newColor = Color.Lerp(initialColor, targetColor, elapsedTime / duration);
                objectRenderer.color = newColor;
            }
            else
            {
                targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);

                // 알파 값을 점진적으로 변경
                Color newColor = Color.Lerp(initialColor, targetColor, elapsedTime / duration);
                objectRenderer.color = newColor;
            }


            // 시간 경과
            elapsedTime += Time.deltaTime;
            yield return null;  // 다음 프레임까지 대기
        }

        // 애니메이션이 끝난 후 최종 상태로 설정
        transform.localScale = new Vector3(initialScale.x, targetHeight, initialScale.z);
        objectRenderer.color = targetColor;
    }
}
