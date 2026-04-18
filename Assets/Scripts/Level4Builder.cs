using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Level4Builder : MonoBehaviour
{
    [Header("Tilemaps")]
    [SerializeField] private Tilemap platformsTilemap;
    [SerializeField] private Tilemap hazardsTilemap;
    [SerializeField] private Tilemap climbingTilemap;
    [SerializeField] private Tilemap waterTilemap;
    [SerializeField] private Tilemap backgroundTilemap;

    [Header("Scene Objects")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform exitPortal;
    [SerializeField] private Transform generatedRoot;

    [Header("Tiles")]
    [SerializeField] private TileBase groundTile;
    [SerializeField] private TileBase spikeTile;
    [SerializeField] private TileBase ladderTile;
    [SerializeField] private TileBase waterTile;
    [SerializeField] private TileBase backgroundTile;

    [Header("Prefabs")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject fallingPlatformPrefab;

    [Header("Build")]
    [SerializeField] private Vector3Int origin = new Vector3Int(0, 0, 0);
    [SerializeField] private bool buildOnStart = true;
    [SerializeField] private bool clearBeforeBuild = true;

    private readonly List<GameObject> spawnedObjects = new();

    private void Start()
    {
        if (buildOnStart)
        {
            BuildLevel();
        }
    }

    [ContextMenu("Build Level 4")]
    public void BuildLevel()
    {
        if (!ValidateSetup()) return;

        NormalizeTransforms();

        if (clearBeforeBuild)
        {
            ClearLevel();
        }

        FillBackground(0, 0, 78, 42);
        BuildPracticalLevel();

        Debug.Log("Level 4 practical build xong.");
    }

    [ContextMenu("Clear Level 4")]
    public void ClearLevel()
    {
        if (platformsTilemap != null) platformsTilemap.ClearAllTiles();
        if (hazardsTilemap != null) hazardsTilemap.ClearAllTiles();
        if (climbingTilemap != null) climbingTilemap.ClearAllTiles();
        if (waterTilemap != null) waterTilemap.ClearAllTiles();
        if (backgroundTilemap != null) backgroundTilemap.ClearAllTiles();

        if (generatedRoot != null)
        {
            for (int i = generatedRoot.childCount - 1; i >= 0; i--)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    DestroyImmediate(generatedRoot.GetChild(i).gameObject);
                else
                    Destroy(generatedRoot.GetChild(i).gameObject);
#else
                Destroy(generatedRoot.GetChild(i).gameObject);
#endif
            }
        }

        for (int i = spawnedObjects.Count - 1; i >= 0; i--)
        {
            if (spawnedObjects[i] == null) continue;
#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(spawnedObjects[i]);
            else
                Destroy(spawnedObjects[i]);
#else
            Destroy(spawnedObjects[i]);
#endif
        }

        spawnedObjects.Clear();
    }

    private bool ValidateSetup()
    {
        bool ok = true;

        if (platformsTilemap == null) { Debug.LogError("Thiếu Platforms Tilemap"); ok = false; }
        if (hazardsTilemap == null) { Debug.LogError("Thiếu Hazards Tilemap"); ok = false; }
        if (climbingTilemap == null) { Debug.LogError("Thiếu Climbing Tilemap"); ok = false; }
        if (backgroundTilemap == null) { Debug.LogError("Thiếu Background Tilemap"); ok = false; }

        if (groundTile == null) { Debug.LogError("Thiếu Ground Tile"); ok = false; }
        if (spikeTile == null) { Debug.LogError("Thiếu Spike Tile"); ok = false; }
        if (ladderTile == null) { Debug.LogError("Thiếu Ladder Tile"); ok = false; }
        if (backgroundTile == null) { Debug.LogError("Thiếu Background Tile"); ok = false; }

        if (player == null) { Debug.LogError("Thiếu Player"); ok = false; }
        if (exitPortal == null) { Debug.LogError("Thiếu Exit Portal"); ok = false; }
        if (generatedRoot == null) { Debug.LogError("Thiếu Generated Root"); ok = false; }

        return ok;
    }

    private void NormalizeTransforms()
    {
        ResetTransform(platformsTilemap != null ? platformsTilemap.transform : null);
        ResetTransform(hazardsTilemap != null ? hazardsTilemap.transform : null);
        ResetTransform(climbingTilemap != null ? climbingTilemap.transform : null);
        ResetTransform(waterTilemap != null ? waterTilemap.transform : null);
        ResetTransform(backgroundTilemap != null ? backgroundTilemap.transform : null);
        ResetTransform(generatedRoot);
    }

    private void ResetTransform(Transform t)
    {
        if (t == null) return;
        t.position = Vector3.zero;
        t.rotation = Quaternion.identity;
        t.localScale = Vector3.one;
    }

    private void BuildPracticalLevel()
    {
        BuildFrame();

        // 1. START: nhảy làm quen
        BuildStartTraining();

        // 2. LADDER TEST: leo rồi đổi hướng
        BuildLadderTest();

        // 3. SPIKE GAP: nhảy qua hố có spike
        BuildSpikeGapTest();

        // 4. ENEMY CORRIDOR: xử lý enemy trong đoạn rộng
        BuildEnemyCorridor();

        // 5. WATER + FALLING PLATFORM: phản ứng liên tục
        BuildWaterReactionZone();

        // 6. VERTICAL CLIMB: leo theo từng tầng có chủ đích
        BuildVerticalTower();

        // 7. FINAL APPROACH: nhịp cuối đến portal
        BuildFinalApproach();
    }

    private void BuildFrame()
    {
        GroundRect(0, 0, 78, 1);
        GroundRect(0, 41, 78, 1);
        GroundRect(0, 0, 1, 42);
        GroundRect(77, 0, 1, 42);
    }

    private void BuildStartTraining()
    {
        SetPlayerPosition(4.5f, 4.1f);

        GroundRect(2, 2, 8, 1);
        GroundRect(12, 4, 4, 1);
        GroundRect(18, 6, 4, 1);

        SpawnCoin(5.0f, 4.5f);
        SpawnCoin(14.0f, 6.5f);
        SpawnCoin(20.0f, 8.5f);
    }

    private void BuildLadderTest()
    {
        // Mục đích: từ bệ thấp leo ladder, lên xong phải đi sang phải ngay
        GroundRect(24, 6, 5, 1);
        Ladder(27, 7, 7);
        GroundRect(24, 14, 8, 1);

        // spike đặt ở bên trái bệ trên để ép người chơi leo lên rồi đi phải, không quay đầu vô thức
        Spikes(24, 14, 2);

        SpawnCoin(30.0f, 16.5f);
    }

    private void BuildSpikeGapTest()
    {
        // Mục đích: nhảy qua hố spike, điểm đáp rõ ràng
        GroundRect(34, 14, 4, 1);
        Spikes(38, 14, 4);
        GroundRect(42, 14, 4, 1);

        // Sau khi đáp thành công, leo tiếp
        Ladder(44, 15, 6);
        GroundRect(41, 21, 8, 1);

        SpawnCoin(43.5f, 16.5f);
        SpawnCoin(47.0f, 23.5f);
    }

    private void BuildEnemyCorridor()
    {
        // Mục đích: hành lang đủ rộng để né enemy, không gài spike vô nghĩa
        GroundRect(12, 21, 16, 1);

        SpawnEnemy(20.0f, 22.2f, 13.0f, 27.0f, 3.0f);
        SpawnCoin(14.0f, 23.5f);
        SpawnCoin(26.0f, 23.5f);

        // cuối hành lang là nhảy ngắn lên bệ tiếp
        GroundRect(30, 23, 4, 1);
        SpawnCoin(31.5f, 25.5f);
    }

    private void BuildWaterReactionZone()
    {
        // Mục đích: từ bệ trái sang phải qua nước bằng các bước nhảy ngắn + 1 falling platform
        GroundRect(36, 23, 5, 1);

        Water(41, 1, 10);

        DrawPlatform(42, 26, 2);
        DrawPlatform(46, 28, 2);
        DrawPlatform(50, 30, 2);

        SpawnFallingPlatform(54.0f, 28.2f, 2.0f);
        GroundRect(57, 26, 5, 1);

        // spike ở mép vào bệ cuối để buộc đáp sâu vào trong bệ
        Spikes(57, 26, 1);

        SpawnCoin(43.0f, 28.5f);
        SpawnCoin(47.0f, 30.5f);
        SpawnCoin(51.0f, 32.5f);
        SpawnCoin(58.5f, 28.5f);
    }

    private void BuildVerticalTower()
    {
        // Mục đích: leo dọc nhiều tầng, mỗi tầng có một quyết định rõ
        GroundRect(60, 26, 5, 1);

        Ladder(63, 27, 6);
        GroundRect(57, 33, 6, 1);

        // tầng 2: sang trái bằng nhảy ngắn
        GroundRect(50, 35, 4, 1);

        // tầng 3: quay lại phải và leo tiếp
        GroundRect(58, 37, 5, 1);
        Ladder(61, 38, 3);

        GroundRect(56, 40, 9, 1);

        // spike chỉ dùng tại điểm rơi sai nhịp
        Spikes(54, 35, 2);

        SpawnEnemy(58.5f, 34.2f, 57.5f, 62.0f, 2.8f);

        SpawnCoin(59.0f, 35.5f);
        SpawnCoin(51.5f, 37.5f);
        SpawnCoin(62.0f, 42.0f);
    }

    private void BuildFinalApproach()
    {
        // từ đỉnh tháp sang portal bằng nhịp cuối
        GroundRect(46, 40, 6, 1);
        DrawPlatform(42, 37, 2);
        DrawPlatform(38, 35, 2);
        GroundRect(32, 33, 5, 1);

        // spike ở dưới quỹ đạo rơi sai, không đặt chặn ngay mặt đi
        Spikes(40, 35, 2);

        SpawnCoin(43.0f, 39.5f);
        SpawnCoin(39.0f, 37.5f);
        SpawnCoin(34.5f, 35.5f);

        SetPortalPosition(34.5f, 35.5f);
    }

    private void FillBackground(int startX, int startY, int width, int height)
    {
        if (backgroundTilemap == null || backgroundTile == null) return;

        for (int x = startX; x < startX + width; x++)
        {
            for (int y = startY; y < startY + height; y++)
            {
                backgroundTilemap.SetTile(Cell(x, y), backgroundTile);
            }
        }
    }

    private void GroundRect(int startX, int startY, int width, int height)
    {
        for (int x = startX; x < startX + width; x++)
        {
            for (int y = startY; y < startY + height; y++)
            {
                platformsTilemap.SetTile(Cell(x, y), groundTile);
            }
        }
    }

    private void DrawPlatform(int startX, int y, int width)
    {
        GroundRect(startX, y, width, 1);
    }

    private void Column(int x, int startY, int height)
    {
        GroundRect(x, startY, 1, height);
    }

    private void Spikes(int startX, int y, int width)
    {
        for (int x = startX; x < startX + width; x++)
        {
            hazardsTilemap.SetTile(Cell(x, y), spikeTile);
        }
    }

    private void Ladder(int x, int startY, int height)
    {
        for (int y = startY; y < startY + height; y++)
        {
            climbingTilemap.SetTile(Cell(x, y), ladderTile);
        }
    }

    private void Water(int startX, int y, int width)
    {
        if (waterTilemap == null || waterTile == null) return;

        for (int x = startX; x < startX + width; x++)
        {
            waterTilemap.SetTile(Cell(x, y), waterTile);
        }
    }

    private Vector3Int Cell(int x, int y)
    {
        return new Vector3Int(origin.x + x, origin.y + y, origin.z);
    }

    private void SetPlayerPosition(float x, float y)
    {
        if (player == null) return;
        player.position = new Vector3(origin.x + x, origin.y + y, 0f);
    }

    private void SetPortalPosition(float x, float y)
    {
        if (exitPortal == null) return;
        exitPortal.position = new Vector3(origin.x + x, origin.y + y, 0f);
    }

    private void SpawnCoin(float x, float y)
    {
        if (coinPrefab == null || generatedRoot == null) return;

        GameObject coin = Instantiate(
            coinPrefab,
            new Vector3(origin.x + x, origin.y + y, 0f),
            Quaternion.identity,
            generatedRoot
        );

        coin.name = "GeneratedCoin";
        spawnedObjects.Add(coin);
    }

    private void SpawnEnemy(float x, float y, float leftX, float rightX, float speed)
    {
        if (enemyPrefab == null || generatedRoot == null) return;

        GameObject enemy = Instantiate(
            enemyPrefab,
            new Vector3(origin.x + x, origin.y + y, 0f),
            Quaternion.identity,
            generatedRoot
        );

        enemy.name = "GeneratedEnemy";

        EnemyMovement oldMovement = enemy.GetComponent<EnemyMovement>();
        if (oldMovement != null)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) DestroyImmediate(oldMovement);
            else Destroy(oldMovement);
#else
            Destroy(oldMovement);
#endif
        }

        RuntimePatrolEnemy patrol = enemy.GetComponent<RuntimePatrolEnemy>();
        if (patrol == null)
        {
            patrol = enemy.AddComponent<RuntimePatrolEnemy>();
        }

        patrol.Configure(
            new Vector2(origin.x + leftX, origin.y + y),
            new Vector2(origin.x + rightX, origin.y + y),
            speed,
            0.2f
        );

        spawnedObjects.Add(enemy);
    }

    private void SpawnFallingPlatform(float x, float y, float widthScale)
    {
        if (fallingPlatformPrefab == null || generatedRoot == null) return;

        GameObject platform = Instantiate(
            fallingPlatformPrefab,
            new Vector3(origin.x + x, origin.y + y, 0f),
            Quaternion.identity,
            generatedRoot
        );

        platform.name = "GeneratedFallingPlatform";
        platform.transform.localScale = new Vector3(widthScale, 1f, 1f);

        if (platform.GetComponent<RuntimeFallingPlatform>() == null &&
            platform.GetComponent<FallingPlatform>() == null)
        {
            platform.AddComponent<RuntimeFallingPlatform>();
        }

        spawnedObjects.Add(platform);
    }
}

public class RuntimePatrolEnemy : MonoBehaviour
{
    private Vector2 leftPoint;
    private Vector2 rightPoint;
    private float moveSpeed;
    private float waitTimeAtPoint;

    private Rigidbody2D rb;
    private Vector3 startScale;
    private bool movingRight = true;
    private float waitCounter = 0f;
    private bool configured = false;

    public void Configure(Vector2 left, Vector2 right, float speed, float waitTime)
    {
        leftPoint = left;
        rightPoint = right;
        moveSpeed = speed;
        waitTimeAtPoint = waitTime;
        configured = true;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startScale = transform.localScale;
    }

    private void Update()
    {
        if (!configured || rb == null) return;

        if (waitCounter > 0f)
        {
            waitCounter -= Time.deltaTime;
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        Vector2 target = movingRight ? rightPoint : leftPoint;
        float dir = Mathf.Sign(target.x - transform.position.x);

        rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);

        if (Mathf.Abs(transform.position.x - target.x) < 0.08f)
        {
            movingRight = !movingRight;
            waitCounter = waitTimeAtPoint;
        }

        if (Mathf.Abs(dir) > 0.01f)
        {
            transform.localScale = new Vector3(
                Mathf.Sign(dir) * Mathf.Abs(startScale.x),
                startScale.y,
                startScale.z
            );
        }
    }
}

public class RuntimeFallingPlatform : MonoBehaviour
{
    [SerializeField] private float fallDelay = 0.35f;
    [SerializeField] private float destroyDelay = 2f;

    private Rigidbody2D rb;
    private bool triggered = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 1f;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (triggered) return;
        if (!collision.collider.CompareTag("Player")) return;

        triggered = true;
        StartCoroutine(FallRoutine());
    }

    private IEnumerator FallRoutine()
    {
        yield return new WaitForSeconds(fallDelay);

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }

        Destroy(gameObject, destroyDelay);
    }
}