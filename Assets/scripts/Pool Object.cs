using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour
{
    public GameObject[] availableObjects;
    public List<GameObject> objects = new List<GameObject>();
    private float screenWidthInPoints;
    public float objectsMinDistance = 10.0f;
    public float objectsMaxDistance = 20.0f;

    public float objectsMinY = 1f;
    public float objectsMaxY = 3f;

    private Queue<GameObject> objectPool = new Queue<GameObject>();  // Pool để lưu trữ đối tượng đã tái sử dụng

    void Start()
    {
        float height = 2.0f * Camera.main.orthographicSize;
        screenWidthInPoints = height * Camera.main.aspect;

        // Tạo một số đối tượng ban đầu để thêm vào pool
        for (int i = 0; i < availableObjects.Length * 3; i++) // Số lượng ban đầu của object pool
        {
            GameObject obj = Instantiate(availableObjects[Random.Range(0, availableObjects.Length)]);
            obj.SetActive(false);  // Vô hiệu hóa đối tượng ngay từ đầu
            objectPool.Enqueue(obj);
        }

        StartCoroutine(GeneratorCheck());
    }

    // Lấy một đối tượng từ pool nếu có, nếu không tạo đối tượng mới
    GameObject GetPooledObject()
    {
        if (objectPool.Count > 0)
        {
            GameObject obj = objectPool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            int randomIndex = Random.Range(0, availableObjects.Length);
            return Instantiate(availableObjects[randomIndex]);
        }
    }

    // Trả đối tượng về pool (vô hiệu hóa đối tượng thay vì hủy nó)
    void ReturnObjectToPool(GameObject obj)
    {
        obj.SetActive(false);  // Vô hiệu hóa đối tượng
        objectPool.Enqueue(obj);  // Đưa đối tượng vào pool để tái sử dụng
    }

    void AddObject(float lastObjectX)
    {
        GameObject obj = GetPooledObject();

        // Xác định vị trí mới cho đối tượng
        float objectPositionX = lastObjectX + Random.Range(objectsMinDistance, objectsMaxDistance);
        float randomY = Random.Range(objectsMinY, objectsMaxY);
        obj.transform.position = new Vector3(objectPositionX, randomY, 0);

        // Thêm đối tượng vào danh sách đang hoạt động
        objects.Add(obj);
    }

    void GenerateObjectsIfRequired()
    {
        float playerX = transform.position.x;
        float removeObjectsX = playerX - screenWidthInPoints;
        float addObjectX = playerX + screenWidthInPoints;
        float farthestObjectX = 0;

        // Xử lý các đối tượng hiện tại và loại bỏ các đối tượng quá xa
        List<GameObject> objectsToRemove = new List<GameObject>();
        foreach (var obj in objects)
        {
            if (obj == null) continue; // Bỏ qua đối tượng null

            float objX = obj.transform.position.x;
            farthestObjectX = Mathf.Max(farthestObjectX, objX);

            // Loại bỏ các đối tượng ra khỏi màn hình
            if (objX < removeObjectsX)
            {
                objectsToRemove.Add(obj);
            }
        }

        // Xóa các đối tượng quá xa (trả về pool thay vì hủy chúng)
        foreach (var obj in objectsToRemove)
        {
            objects.Remove(obj);
            ReturnObjectToPool(obj);  // Đưa đối tượng về pool
        }

        // Đảm bảo có đủ đối tượng để sinh ra tiếp
        if (farthestObjectX < addObjectX || objects.Count == 0)
        {
            AddObject(farthestObjectX);  // Thêm đối tượng tiếp theo
        }
    }

    private IEnumerator GeneratorCheck()
    {
        while (true)
        {
            GenerateObjectsIfRequired();
            yield return new WaitForSeconds(0.25f);  // Kiểm tra mỗi 0.25 giây
        }
    }
}
