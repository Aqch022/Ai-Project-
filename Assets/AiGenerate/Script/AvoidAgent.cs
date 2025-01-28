using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AvoidAgent : Agent
{
    [SerializeField] private Transform targetTransform; // 目標地点
    [SerializeField] private Transform[] obstacles; // 障害物
    [SerializeField] private float moveSpeed = 5f; // 移動速度

    private float rayDistance;
    // 初期化
    public override void Initialize()
    {
        rayDistance = 5.0f;
    }
    // エピソード開始時のリセット処理
    public override void OnEpisodeBegin()
    {
        // エージェントとターゲットの位置をリセット
       //transform.localPosition = new Vector3(Random.Range(-4f, 4f), 0, Random.Range(-4f, 4f));
        targetTransform.localPosition = new Vector3(Random.Range(-4f, 4f), 0, Random.Range(-4f, 4f));

        // 障害物の位置をランダム化
        foreach (var obstacle in obstacles)
        {
            obstacle.localPosition = new Vector3(Random.Range(-4f, 4f), 0, Random.Range(-4f, 4f));
        }
    }

    // 観測データの収集
    public override void CollectObservations(VectorSensor sensor)
    {
        // ターゲットの相対位置
        sensor.AddObservation(targetTransform.localPosition - transform.localPosition);

        // 障害物の相対位置
        foreach (var obstacle in obstacles)
        {
            sensor.AddObservation(obstacle.localPosition - transform.localPosition);
        }

        // エージェントの速度
        sensor.AddObservation(GetComponent<Rigidbody>().velocity);
    }

    // 行動実行
    public override void OnActionReceived(ActionBuffers actions)
    {
        // ContinuousActionsのサイズが2であることを前提
        if (actions.ContinuousActions.Length < 2)
        {
            Debug.LogError("ContinuousActions array size is less than expected.");
            return;
        }
        // 行動を取得
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        // エージェントの移動
        Vector3 move = new Vector3(moveX, 0, moveZ) * moveSpeed * Time.deltaTime;
        transform.localPosition += move;

        // 距離に応じて報酬を与える
        float distanceToTarget = Vector3.Distance(transform.localPosition, targetTransform.localPosition);
        AddReward(-0.01f); // 時間経過でペナルティ

        if (distanceToTarget < 1.5f)
        {
            AddReward(30.0f);
            EndEpisode();
        }

        // 障害物との衝突ペナルティ
        foreach (var obstacle in obstacles)
        {
            if (Vector3.Distance(transform.localPosition, obstacle.localPosition) < 1.0f)
            {
                AddReward(-50.0f);
                EndEpisode();
            }
        }
    }
    public void Update()
    {
        Vector3 rayPosition = transform.position + new Vector3(0, 0, 0);
        Ray ray1 = new Ray(rayPosition, Vector3.right);
        Ray ray2 = new Ray(rayPosition, Vector3.left);

        Debug.DrawRay(rayPosition, Vector3.right * rayDistance, Color.red);
        Debug.DrawRay(rayPosition, Vector3.left * rayDistance, Color.red);
        RaycastHit hit;
        if (Physics.Raycast(ray1, out hit, rayDistance))
        {
            if (hit.collider.tag == "block")
            {
                AddReward(30.0f);
            }
            if (hit.collider.tag == "wall")
            {
                AddReward(-1.0f);
            }
        }
        if (Physics.Raycast(ray2, out hit, rayDistance))
        {
            if (hit.collider.tag == "block")
            {
                AddReward(30.0f);
            }
            if (hit.collider.tag == "wall")
            {
                AddReward(-1.0f);
            }
        }
    }

    // ヒューマン操作用（デバッグ用）
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;
        // ContinuousActionsのサイズが2であることを前提
        if (actionsOut.ContinuousActions.Length < 2)
        {
            Debug.LogError("ContinuousActions array size is less than expected.");
            return;
        }
        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
    }
}
