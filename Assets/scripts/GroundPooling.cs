using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPooling : MonoBehaviour
{
    [SerializeField] private GameObject groundPrefab; // Prefab của đối tượng Ground
    [SerializeField] private int poolSize = 10; // Số lượng đối tượng ground trong pool
    [SerializeField] private float groundWidth = 20f; // Chiều rộng của mỗi đoạn ground
    [SerializeField] private float minYOffset = -2f; // Giá trị tối thiểu cho độ lệch Y
    [SerializeField] private float maxYOffset = 3f; // Giá trị tối đa cho độ lệch Y
    [SerializeField] private Transform player; // Vị trí của Player
    [SerializeField] private Camera mainCamera; // Camera chính để xác định khung nhìn

    private List<GameObject> groundPool; // Danh sách chứa các đoạn ground trong pool
    private Vector3 lastEndPosition; // Vị trí kết thúc của đoạn ground cuối cùng
    private float lastXPosition; // Vị trí X cuối cùng của nền xa nhất phía trước

    void Start()
    {
        // Khởi tạo pool nền
        groundPool = new List<GameObject>();

        // Vị trí bắt đầu cho nền đầu tiên
        lastEndPosition = transform.position;

        // Tạo các đối tượng nền ban đầu và thêm vào pool
        for (int i = 0; i < poolSize; i++)
        {
            SpawnGroundSegment();
        }

        // Cập nhật vị trí X cuối cùng dựa trên số nền ban đầu
        lastXPosition = (poolSize - 1) * groundWidth;
    }

    void Update()
    {
        // Lấy vị trí rìa bên trái của camera
        float cameraLeftEdge = mainCamera.transform.position.x - mainCamera.orthographicSize * mainCamera.aspect;

        // Kiểm tra từng nền trong pool
        foreach (var ground in groundPool)
        {
            // Nếu nền đã vượt qua rìa bên trái của camera
            if (ground.transform.position.x + groundWidth < cameraLeftEdge)
            {
                // Dịch chuyển nền ra phía trước
                RepositionGround(ground);
            }
        }
    }

    // Hàm sinh một đoạn ground mới
    void SpawnGroundSegment()
    {
        // Tính toán vị trí mới với độ lệch Y ngẫu nhiên
        float randomYOffset = Random.Range(minYOffset, maxYOffset);
        Vector3 spawnPosition = new Vector3(lastEndPosition.x + groundWidth, lastEndPosition.y + randomYOffset, lastEndPosition.z);

        // Tạo một đoạn ground mới tại vị trí spawn
        GameObject newGround = Instantiate(groundPrefab, spawnPosition, Quaternion.identity);

        // Thêm đoạn ground vào danh sách pool
        groundPool.Add(newGround);

        // Cập nhật vị trí kết thúc cho đoạn ground tiếp theo
        lastEndPosition = newGround.transform.position;
    }
    // Hàm di chuyển một nền về phía trước khi Player đi qua
    void RepositionGround(GameObject ground)
    {
        float randomXOffset = Random.Range(20f, 30f);
        // Tính toán vị trí X mới dựa trên vị trí kết thúc của nền cuối cùng (lastEndPosition)
        lastEndPosition.x += randomXOffset;
        // Lấy giá trị Y hiện tại của nền cuối cùng
        float currentYOffset = lastEndPosition.y;
        // Tạo một giá trị Y mới đảm bảo chênh lệch ít nhất 1 đơn vị so với nền trước đó
        float newYOffset;
        // Kiểm tra để đảm bảo sự chênh lệch ít nhất 1 đơn vị và trong khoảng minYOffset - maxYOffset
        do
        {
            newYOffset = Random.Range(minYOffset, maxYOffset);
        } while (Mathf.Abs(newYOffset - currentYOffset) < 1.5f);  // Đảm bảo sự chênh lệch ít nhất là 1
        // Đặt vị trí mới cho nền, với sự chênh lệch Y rõ ràng
        ground.transform.position = new Vector3(lastEndPosition.x, newYOffset, ground.transform.position.z);
        // Cập nhật lastEndPosition để theo dõi nền cuối cùng đã được di chuyển
        lastEndPosition = ground.transform.position;
    }
}
