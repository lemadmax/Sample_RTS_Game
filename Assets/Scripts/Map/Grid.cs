using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public int mapW;
    public int mapL;
    public int unitW;
    public bool drawLine;

    public static int w = 200;
    public static int l = 200;
    int[,] map = new int[w, l];

    Dictionary<int, List<Vector2Int>> obstacles = new Dictionary<int, List<Vector2Int>>();
    Dictionary<int, List<GameObject>> objs = new Dictionary<int, List<GameObject>>();

    LineRenderer line;
    

    void Start()
    {
        if(drawLine)
        {
            line = GetComponent<LineRenderer>();
            line.positionCount = (w + l) * 2;
            line.useWorldSpace = true;
            line.startColor = new Color(0.8f, 0.9f, 0.8f, 0.8f);
            line.endColor = new Color(0.8f, 0.9f, 0.8f, 0.8f);
            line.startWidth = 0.4f;
            line.endWidth = 0.4f;
            CreatePoints();
        }
    }

    public void SetGridPos(List<Vector3> points, int id)
    {
        List<Vector2Int> newObstacle = new List<Vector2Int>();
        List<GameObject> newObj = new List<GameObject>();
        foreach (Vector3 point in points)
        {
            int x = (int)(point.x + 500.0f) / unitW;
            int y = (int)(point.z + 500.0f) / unitW;
            newObstacle.Add(new Vector2Int(x, y));
            newObj.Add(CreateQuad(x, y));
            map[x, y] = id;
        }
        if(objs.ContainsKey(id))
        {
            foreach(GameObject ob in objs[id])
            {
                if(!newObj.Contains(ob))
                {
                    Destroy(ob);
                }
            }
            objs[id] = newObj;
        }
        else
        {
            objs.Add(id, newObj);
        }
        if(obstacles.ContainsKey(id))
        {
            foreach (Vector2Int point in obstacles[id])
            {
                if(!newObstacle.Contains(point))
                {
                    map[point.x, point.y] = 0;
                }
            }
            obstacles[id] = newObstacle;
        }
        else
        {
            obstacles.Add(id, newObstacle);
        }
    }

    public void RemoveObstacle(int id)
    {
        if (obstacles.ContainsKey(id))
        {
            List<Vector2Int> obstacle = obstacles[id];
            List<GameObject> meshes = objs[id];
            foreach (Vector2Int point in obstacle)
            {
                map[point.x, point.y] = 0;
            }
            foreach (GameObject mesh in meshes)
            {
                Destroy(mesh);
            }
        }
        obstacles.Remove(id);
        objs.Remove(id);
    }

    void CreatePoints()
    {
        int index = 0;
        for(int i = 0; i < w; i++)
        {
            if(i % 2 == 0)
            {
                line.SetPosition(index++, new Vector3(i * unitW - 500.0f, 0.2f, -500.0f));
                line.SetPosition(index++, new Vector3(i * unitW - 500.0f, 0.2f, 500.0f));
            }
            else
            {
                line.SetPosition(index++, new Vector3(i * unitW - 500.0f, 0.2f, 500.0f));
                line.SetPosition(index++, new Vector3(i * unitW - 500.0f, 0.2f, -500.0f));
            }
        }
        for (int i = 0; i < l; i++)
        {
            if(i % 2 == 0)
            {
                line.SetPosition(index++, new Vector3(-500.0f, 0.2f, i * unitW - 500.0f));
                line.SetPosition(index++, new Vector3(500.0f, 0.2f, i * unitW - 500.0f));
            }
            else
            {
                line.SetPosition(index++, new Vector3(500.0f, 0.2f, i * unitW - 500.0f));
                line.SetPosition(index++, new Vector3(-500.0f, 0.2f, i * unitW - 500.0f));
            }
        }
        //Debug.Log("line position count: " + index);
    }
    public GameObject CreateQuad(int w, int l)
    {
        if (!drawLine) return null;
        string name = "Quad" + w + "x" + l;
        GameObject obj = GameObject.Find(name);
        if (obj != null) return obj;
        obj = new GameObject("Quad" + w + "x" + l);
        MeshFilter mf = obj.AddComponent<MeshFilter>();
        MeshRenderer mr = obj.AddComponent<MeshRenderer>();
        float x = w * unitW - 500.0f;
        float y = l * unitW - 500.0f;
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(x, 0.2f, y);
        vertices[1] = new Vector3(x, 0.2f, y + 5.0f);
        vertices[2] = new Vector3(x + 5.0f, 0.2f, y + 5.0f);
        vertices[3] = new Vector3(x + 5.0f, 0.2f, y);

        int[] tris = { 0, 1, 2, 0, 2, 3 };
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.RecalculateBounds();

        mf.sharedMesh = mesh;
        Shader shader = Shader.Find("Diffuse");
        mr.sharedMaterial = new Material(shader);
        mr.sharedMaterial.color = Color.red;
        
        return obj;
    }
    public bool IsLocEmpty(float x, float y)
    {
        int xx = (int)(x + 500.0f) / unitW;
        int yy = (int)(y + 500.0f) / unitW;
        return map[xx, yy] == 0;
    }
    public int GetMapValue(float x, float y)
    {
        int xx = (int)(x + 500.0f) / unitW;
        int yy = (int)(y + 500.0f) / unitW;
        return map[xx, yy];
    }

    int[] dx = { -1, 0, 1, 1, 1, 0, -1, -1 };
    int[] dy = { -1, -1, -1, 0, 1, 1, 1, 0 };
    public Vector3 GetNearestLoc(float x, float y)
    {
        Vector3 res = new Vector3();
        HashSet<Vector2Int>  visited = new HashSet<Vector2Int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Vector2Int start = new Vector2Int((int)(x + 500.0f) / unitW, (int)(y + 500.0f) / unitW);
        if (map[start.x, start.y] == 0) return new Vector3(start.x * unitW - 500.0f + 2.5f, 0, start.y * unitW - 500.0f + 2.5f);
        //Debug.Log(map[start.x, start.y]);
        queue.Enqueue(start);
        visited.Add(start);
        while(queue.Count > 0)
        {
            Vector2Int cur = queue.Dequeue();
            for(int i = 0; i < 8; i++)
            {
                Vector2Int next = new Vector2Int(cur.x + dx[i], cur.y + dy[i]);
                if (visited.Contains(next) || next.x < 0 || next.x >= 200 || next.y < 0 || next.y >= 200) continue;
                if (map[next.x, next.y] == 0) return new Vector3(next.x * unitW - 500.0f + 2.5f, 0, next.y * unitW - 500.0f + 2.5f);
                queue.Enqueue(next);
                visited.Add(next);
            }
        }
        return res;
    }
    public Vector3 GetNearestAdjLoc(float x, float y, Vector3 origin)
    {
        Vector3 res = new Vector3(2000f, 0f, 2000f);
        float minDis = (res - origin).magnitude;
        int xx = (int)(x + 500.0f) / unitW;
        int yy = (int)(y + 500.0f) / unitW;
        for (int i = 0; i < 8; i++)
        {
            int nx = xx + dx[i];
            int ny = yy + dy[i];
            if (nx < 0 || nx >= 200 || ny < 0 || ny >= 200) continue;
            Vector3 nLoc = new Vector3(nx * unitW - 500.0f + 2.5f, 0.0f, ny * unitW - 500.0f + 2.5f);
            float nDis = (nLoc - origin).magnitude;
            if(nDis < minDis)
            {
                res = nLoc;
                minDis = nDis;
            }
        }
        if (res.x == 2000f) return new Vector3(-1f, -1f, -1f);
        else return res;
    }
}
