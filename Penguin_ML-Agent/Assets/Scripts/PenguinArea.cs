using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using TMPro;

public class PenguinArea : MonoBehaviour
{
    [Tooltip("The agent inside the area")]
    public PenguinAgent penguinAgent;

    [Tooltip("The baby penguin inside the area")]
    public GameObject penguinBaby;

    [Tooltip("The TextMeshPro text that shows the cumulative reward of the agent")]
    public TextMeshPro cumulativeRewardText;

    [Tooltip("Prefab of a live fish")]
    public Fish fishPrefab;

    private List<GameObject> fishList;

    /// <summary>
    /// 물고기 및 펭귄 배치를 포함한 영역 재설정
    /// </summary>
    public void ResetArea()
    {
        RemoveAllFish();
        PlacePenguin();
        PlaceBaby();
        SpawnFish(4, .5f);
    }

    /// <summary>
    /// 특정 생선을 먹을 때 해당 부위에서 제거
    /// </summary>
    /// <param name="fishObject">제거할 물고기</param>
    public void RemoveSpecificFish(GameObject fishObject)
    {
        fishList.Remove(fishObject);
        Destroy(fishObject);
    }

    /// <summary>
    /// 남은 물고기 수
    /// </summary>
    public int FishRemaining
    {
        get { return fishList.Count; }
    }

    /// <summary>
    /// 부분 도넛 모양 내에서 X-Z 평면의 임의 위치 선택
    /// </summary>
    /// <param name="center">도넛의 중심부</param>
    /// <param name="minAngle">쐐기의 최소 각도</param>
    /// <param name="maxAngle">쐐기의 최대 각도</param>
    /// <param name="minRadius">중심으로부터의 최소 거리</param>
    /// <param name="maxRadius">중심으로부터의 최대 거리</param>
    /// <returns>지정된 영역 내에 있는 위치</returns>
    public static Vector3 ChooseRandomPosition(Vector3 center, float minAngle, float maxAngle, float minRadius, float maxRadius)
    {
        float radius = minRadius;
        float angle = minAngle;

        if (maxRadius > minRadius)
        {
            // 임의의 반지름 선택
            radius = UnityEngine.Random.Range(minRadius, maxRadius);
        }

        if (maxAngle > minAngle)
        {
            // 임의의 각도 선택
            angle = UnityEngine.Random.Range(minAngle, maxAngle);
        }

        // 중심 위치 + 전방 벡터가 Y축을 중심으로 "각도"만큼 회전하고 "반경"으로 곱합니다.
        return center + Quaternion.Euler(0f, angle, 0f) * Vector3.forward * radius;
    }

    /// <summary>
    /// 해당 지역에서 모든 물고기 제거
    /// </summary>
    private void RemoveAllFish()
    {
        if (fishList != null)
        {
            for (int i = 0; i < fishList.Count; i++)
            {
                if (fishList[i] != null)
                {
                    Destroy(fishList[i]);
                }
            }
        }

        fishList = new List<GameObject>();
    }

    /// <summary>
    /// 해당 지역에 펭귄 배치
    /// </summary>
    private void PlacePenguin()
    {
        Rigidbody rigidbody = penguinAgent.GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        penguinAgent.transform.position = ChooseRandomPosition(transform.position, 0f, 360f, 0f, 9f) + Vector3.up * .5f;
        penguinAgent.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
    }

    /// <summary>
    /// 해당 지역에 아기를 배치
    /// </summary>
    private void PlaceBaby()
    {
        Rigidbody rigidbody = penguinBaby.GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        penguinBaby.transform.position = ChooseRandomPosition(transform.position, -45f, 45f, 4f, 9f) + Vector3.up * .5f;
        penguinBaby.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }

    /// <summary>
    /// 해당 지역에 몇 마리의 물고기를 생성하고 수영 속도를 설정합니다.
    /// </summary>
    /// <param name="count">The number to spawn</param>
    /// <param name="fishSpeed">The swim speed</param>
    private void SpawnFish(int count, float fishSpeed)
    {
        for (int i = 0; i < count; i++)
        {
            // 산란 및 물고기 배치
            GameObject fishObject = Instantiate<GameObject>(fishPrefab.gameObject);
            fishObject.transform.position = ChooseRandomPosition(transform.position, 100f, 260f, 2f, 13f) + Vector3.up * .5f;
            fishObject.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);

            // 물고기의 부모를 이 영역의 변형으로 설정합니다.
            fishObject.transform.SetParent(transform);

            // 물고기를 추적
            fishList.Add(fishObject);

            // 물고기 속도 설정
            fishObject.GetComponent<Fish>().fishSpeed = fishSpeed;
        }
    }

    /// <summary>
    /// 게임 시작시 호출
    /// </summary>
    private void Start()
    {
        ResetArea();
    }

    /// <summary>
    /// 모든 프레임 호출
    /// </summary>
    private void Update()
    {
        // 누적 보상 텍스트 업데이트
        cumulativeRewardText.text = penguinAgent.GetCumulativeReward().ToString("0.00");
    }

}
