using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] GameObject boss2;
    [SerializeField] TextMeshProUGUI bossInfo;

    public void OnBoss1Cleared()
    {
        // CinemachineTargetGroup targetGroup = FindObjectOfType<CinemachineTargetGroup>();

        // // 현재 타겟 리스트를 가져옴
        // CinemachineTargetGroup.Target[] targets = targetGroup.m_Targets;

        // // 새로운 타겟 리스트를 기존보다 한 개 더 큰 배열로 확장
        // CinemachineTargetGroup.Target[] newTargets = new CinemachineTargetGroup.Target[targets.Length];

        // // 기존 타겟을 새 배열로 복사
        // for (int i = 0; i < targets.Length; i++)
        // {
        //     newTargets[i] = targets[i];
        // }

        // // 새로운 타겟을 배열에 추가
        // newTargets[1] = new CinemachineTargetGroup.Target
        // {
        //     target = boss2.transform,
        //     weight = 1,
        //     radius = 2
        // };

        // // 새 타겟 배열을 타겟 그룹에 할당
        // targetGroup.m_Targets = newTargets;

        // boss2.SetActive(true);

        SceneManager.LoadScene("02_Game_Boss2");
    }

    public void OnBoss2Cleared()
    {
        SceneManager.LoadScene("03_Game_Boss3");
    }

    public void showBossInfo(string text)
    {
        bossInfo.SetText(text);
        bossInfo.transform.parent.gameObject.SetActive(true);
    }
    public void hideBossInfo()
    {
        bossInfo.transform.parent.gameObject.SetActive(false);
    }
}
