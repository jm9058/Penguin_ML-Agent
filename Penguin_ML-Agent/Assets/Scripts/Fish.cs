using UnityEngine;

public class Fish : MonoBehaviour
{
    [Tooltip("The swim speed")]
    public float fishSpeed;

    private float randomizedSpeed = 0f;
    private float nextActionTime = -1f;
    private Vector3 targetPosition;

    /// <summary>
    /// 타임스텝마다 호출
    /// </summary>
    private void FixedUpdate()
    {
        if (fishSpeed > 0f)
        {
            Swim();
        }
    }

    /// <summary>
    /// 임의의 위치 사이에서 수영
    /// </summary>
    private void Swim()
    {
        // If, 다음 행동을 할 시간이라면 새로운 속도와 목적지를 선택하세요.
        // Else, 그렇지 않으면 목적지를 향해 수영하십시오.
        if (Time.fixedTime >= nextActionTime)
        {
            // 속도 무작위화
            randomizedSpeed = fishSpeed * UnityEngine.Random.Range(.5f, 1.5f);

            // 임의의 대상 선택
            targetPosition = PenguinArea.ChooseRandomPosition(transform.parent.position, 100f, 260f, 2f, 13f);

            // 대상을 향해 회전
            transform.rotation = Quaternion.LookRotation(targetPosition - transform.position, Vector3.up);

            // 거기에 도착하는 시간을 계산
            float timeToGetThere = Vector3.Distance(transform.position, targetPosition) / randomizedSpeed;
            nextActionTime = Time.fixedTime + timeToGetThere;
        }
        else
        {
            // 물고기가 목표물을 지나 헤엄치지 않도록 하십시오.
            Vector3 moveVector = randomizedSpeed * transform.forward * Time.fixedDeltaTime;
            if (moveVector.magnitude <= Vector3.Distance(transform.position, targetPosition))
            {
                transform.position += moveVector;
            }
            else
            {
                transform.position = targetPosition;
                nextActionTime = Time.fixedTime;
            }
        }
    }
}
