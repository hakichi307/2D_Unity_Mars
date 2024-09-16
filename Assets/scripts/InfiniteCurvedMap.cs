using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteCurvedMap : MonoBehaviour
{
    public GameObject groundPrefab;  // Prefab của đoạn đất
    public Transform player;  // Transform của Player
    public int initialSegmentCount = 10;  // Số đoạn map ban đầu
    public float segmentWidth = 5f;  // Chiều rộng của mỗi đoạn map
    public float amplitude = 2f;  // Biên độ của sóng (độ cao của đoạn cong)
    public float frequency = 1f;  // Tần số của sóng (tần suất uốn cong)
    public float distanceBeforeSpawn = 10f;  // Khoảng cách trước khi sinh đoạn mới

    private List<GameObject> groundSegments = new List<GameObject>();  // Danh sách chứa các đoạn địa hình
    private float lastXPosition;  // Vị trí x cuối cùng của đoạn map sinh ra
    private float initialOffset;  // Giá trị dùng để tiếp tục đường cong

    void Start()
    {
        // Sinh các đoạn map ban đầu
        for (int i = 0; i < initialSegmentCount; i++)
        {
            SpawnGroundSegment();
        }
    }
    void Update()
    {
        // Nếu Player sắp đến đoạn cuối, sinh thêm đoạn mới
        if (player.position.x > lastXPosition - distanceBeforeSpawn)
        {
            SpawnGroundSegment();
        }
    }
    void SpawnGroundSegment()
    {
        // Tính toán vị trí X mới
        lastXPosition += segmentWidth;
        // Tính toán vị trí Y dựa trên hàm sin để tạo địa hình cong
        float yOffset = Mathf.Sin(initialOffset + lastXPosition * frequency) * amplitude;
        // Tạo vị trí mới cho đoạn map
        Vector3 spawnPosition = new Vector3(lastXPosition, yOffset, 0);
        // Sinh đoạn map tại vị trí mới
        GameObject newGround = Instantiate(groundPrefab, spawnPosition, Quaternion.identity);
        // Thêm đoạn map vào danh sách
        groundSegments.Add(newGround);
        // Tăng giá trị offset để tiếp tục đường cong
        initialOffset += frequency;
    }
}
