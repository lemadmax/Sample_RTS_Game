using UnityEngine;
/*********************************************************
 *
 * ----------------- CameraController --------------------
 * This class controlls the movement of the main camera.
 *
 *********************************************************/
public class CameraController : MonoBehaviour
{
    public bool cameraLock;         // Lock camera from moving for easy testing.
    public float movementSpeed;
    public float zoomSpeed;
    public float zoomAmount;
    public Camera inGameCamera;
    public GameObject miniMapViewArea;

    Transform cameraTransform;
    Transform followTransform;
    Transform miniMapViewAreaRect;

    Vector3 newPosition;
    Vector3 newZoom;
    int     screenWidth;
    int     screenHeight;

    Vector3 zoomVec;
    Vector3 minZoom = new Vector3(0, 10, -10);
    Vector3 maxZoom = new Vector3(0, 80, -80);

    void Start()
    {
        if(GameObject.Find("PlayerInitSetting").GetComponent<PlayerInitSetting>().side == 1)
        {
            transform.position = new Vector3(455f, 0.0f, -455f);
        }
        else
        {
            transform.position = new Vector3(-455f, 0.0f, 455f);
        }

        cameraTransform = inGameCamera.transform;
        newPosition     = transform.position;
        newZoom         = cameraTransform.localPosition;
        screenWidth     = Screen.width;
        screenHeight    = Screen.height;
        miniMapViewAreaRect = miniMapViewArea.GetComponent<Transform>();

        zoomVec.Set(0, zoomAmount, -zoomAmount);
    }

    void Update()
    {
        if(followTransform != null)
        {
            transform.position = followTransform.position;
        }
        else
        {
            HandleMouseInput();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            followTransform = null;
        }
    }

    void HandleMouseInput()
    {
        if(Input.mousePosition.x < 1)
        {
            newPosition -= transform.right * movementSpeed * Time.deltaTime;
        }
        if(Input.mousePosition.x > screenWidth - 2)
        {
            newPosition += transform.right * movementSpeed * Time.deltaTime;
        }
        if(Input.mousePosition.y < 1)
        {
            newPosition -= transform.forward * movementSpeed * Time.deltaTime;
        }
        if(Input.mousePosition.y > screenHeight - 2)
        {
            newPosition += transform.forward * movementSpeed * Time.deltaTime;
        }
        if(Input.mouseScrollDelta.y > 0)
        {
            newZoom -= zoomVec * zoomSpeed * Time.deltaTime;
            if (newZoom.y < minZoom.y) newZoom = minZoom;
        }
        if(Input.mouseScrollDelta.y < 0)
        {
            newZoom += zoomVec * zoomSpeed * Time.deltaTime;
            if (newZoom.y > maxZoom.y) newZoom = maxZoom;
        }
        if (newPosition.x >= -500 && newPosition.x <= 500 && newPosition.z >= -500 && newPosition.z <= 500 && !cameraLock) {
			MoveMiniMapViewArea(newPosition);
            transform.position = newPosition;
        }
        cameraTransform.localPosition = newZoom;
        miniMapViewAreaRect.localScale = new Vector3(newZoom.y * 0.5f, newZoom.y * 0.75f);
    }
    public void LookAtHero(GameObject hero)
    {
        float xOffset = cameraTransform.position.y * Mathf.Tan(Mathf.PI * (90f - cameraTransform.rotation.eulerAngles.x) / 180f) - 2.0f;
        transform.position = new Vector3(hero.transform.position.x - xOffset, 0, hero.transform.position.z);
        newPosition = transform.position;
    }
    public void LookAtBase(GameObject playerBase)
    {
        float xOffset = cameraTransform.position.y * Mathf.Tan(Mathf.PI * (90f - cameraTransform.rotation.eulerAngles.x) / 180f) - 2.0f;
        transform.position = new Vector3(playerBase.transform.position.x - xOffset, 0, playerBase.transform.position.z);
        newPosition = transform.position;
    }
    public void LookAtGivenPoint(Vector3 mapPos)
    {
        transform.position = mapPos;
        newPosition = transform.position;
    }
    public void MoveMiniMapViewArea(Vector3 mapPos)
    {
        mapPos.y = 100.0f;
        miniMapViewAreaRect.position = mapPos;
    }
}
