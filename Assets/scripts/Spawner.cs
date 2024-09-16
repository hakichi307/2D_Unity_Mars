using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSpawner : MonoBehaviour
{
    [SerializeField] private GameObject groundPrefab; // Prefab của đối tượng Ground
    [SerializeField] private int initialGroundCount = 10; // Số lượng ground ban đầu được sinh
    [SerializeField] private float groundWidth = 10f; // Chiều rộng của mỗi đoạn ground
    [SerializeField] private float minYOffset = -2f; // Giá trị tối thiểu cho độ lệch Y
    [SerializeField] private float maxYOffset = 2f; // Giá trị tối đa cho độ lệch Y

    private List<GameObject> groundSegments = new List<GameObject>(); // Danh sách chứa các đoạn ground
    private Vector3 lastEndPosition; // Vị trí kết thúc của đoạn ground cuối cùng

    void Start()
    {
        // Khởi tạo vị trí bắt đầu
        lastEndPosition = transform.position;

        // Sinh các đoạn ground ban đầu
        for (int i = 0; i < initialGroundCount; i++)
        {
            SpawnGroundSegment();
        }
    }

    void SpawnGroundSegment()
    {
        // Tính toán vị trí mới cho đoạn ground
        float randomYOffset = Random.Range(minYOffset, maxYOffset);
        Vector3 spawnPosition = new Vector3(lastEndPosition.x + groundWidth, lastEndPosition.y + randomYOffset, lastEndPosition.z);

        // Tạo một đoạn ground mới tại vị trí tính toán
        GameObject newGround = Instantiate(groundPrefab, spawnPosition, Quaternion.identity);

        // Thêm đoạn ground mới vào danh sách
        groundSegments.Add(newGround);

        // Cập nhật vị trí kết thúc cho đoạn ground tiếp theo
        lastEndPosition = newGround.transform.position;
    }

    // Hàm để sinh thêm ground khi cần thiết
    void Update()
    {
        // Kiểm tra nếu cần sinh thêm ground dựa trên điều kiện nào đó (như nhân vật gần đến đoạn cuối cùng)
        // Nếu cần sinh thêm, gọi SpawnGroundSegment();
    }
}
