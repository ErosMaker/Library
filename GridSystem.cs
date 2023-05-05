using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ErosUtils;

public class GridSystem : MonoBehaviour
{
    private bool[,] gridData;

    public GameObject instance;
    GameObject _object;
    public Material src;
    private GameObject popupMessage;
    private TextMesh mesh;
    private Vector3 cameraPosition;
    private Camera cam;
    private GameObject[,] gridObjects;
    public Material source;

    private List<Grid<GameObject>> layers = new List<Grid<GameObject>>();

    private int activeLayer = 0;

    int gridWidth = 30;
    int gridHeight = 30;
    float origin = .1f;
    float cellSize = 1f;
    float yAxisAngle;
    Vector2 camPos;
    float camRadius = 16f;
    float rotSpeed = 3f;
    float scrollSpeed = 400;
    float animAngleSpeed = 250f;
    float animMoveSpeed = 4f;
    float inactiviteTimer;

    bool cutsceneIntro = true;
    bool changeCutsceneValues = false;
    //bool isAreaCreated = false;
    bool brushEnabled = false;

    private void Start()
    {
        cam = Camera.main;
        _object = new GameObject("CamAnglePointer");
        layers.Add(new Grid<GameObject>(gridWidth, gridHeight, cellSize, new Color(0, .7f, 1, 1), new Vector3(0, origin, 0), true, source));
        gridData = new bool[gridWidth, gridHeight];
        gridObjects = new GameObject[gridWidth, gridHeight];
        //instance = new GameObject("Block", typeof(MeshRenderer), typeof(MeshFilter));
        instance.transform.position = new Vector3(-10000, 0, -10000);
        instance.layer = 1;
        yAxisAngle = cam.transform.eulerAngles.y;
        camPos = new Vector2(cam.transform.position.x, cam.transform.position.z);

        //instance.GetComponent<MeshFilter>().mesh = ErosCubeMesh.GetMesh();

        camPos = new Vector2(0 + cellSize * .5f, 0+ cellSize * .5f);
        yAxisAngle = -Mathf.PI / 1.3f;
        animAngleSpeed = .75f;
        animMoveSpeed = .75f;
        inactiviteTimer = 20f;
    }

    private void Update()
    {
        if(inactiviteTimer >= 0)
        {
            inactiviteTimer -= Time.deltaTime * 1;
        }
        else
        {
            yAxisAngle += Time.deltaTime * .1f;
            cam.transform.eulerAngles = new Vector3(cam.transform.eulerAngles.x + Time.deltaTime * .4f, yAxisAngle, cam.transform.eulerAngles.z + Time.deltaTime * .2f);
        }

        if(cutsceneIntro && !changeCutsceneValues)
        {
            animMoveSpeed += Time.deltaTime * .8f;
            camRadius -= 2 * Time.deltaTime;

            if(animMoveSpeed >= 4f)
            {
                changeCutsceneValues = true;
                animMoveSpeed = 4f;
            }
        }
        if (!Input.GetKey(KeyCode.LeftAlt)
            && Input.GetMouseButtonDown(0))
        {
            inactiviteTimer = 20f;
            //Physics.IgnoreLayerCollision()
            Vector3 point = ErosInput.GetMouseXZCoords(origin);
            int x, z;

            layers[activeLayer].GetXZ(point, out x, out z);

            if (x < gridWidth && z < gridHeight && x >= 0 && z >= 0)
            {

                if (!gridData[x, z])
                {
                    float r = 1f / gridWidth;
                    float b = 1f / gridHeight;

                    Material source = new Material(src);
                    source.color = new Color(r * x + .2f, 1f * (1f / ((x + z + .3f)/5f)) + .2f, b * z + .2f);
                    Debug.Log(source.color.ToString());
                    layers[activeLayer][x, z] = Instantiate(instance, new Vector3(x, origin + .4f, z) * cellSize + new Vector3(cellSize * .5f, .1f, cellSize * .5f), Quaternion.identity);
                    //layers[activeLayer][x, z].GetComponent<MeshRenderer>().material = source;
                    gridData[x, z] = true;
                }

                else
                {
                    GameObject _popupMessage = Instantiate(new GameObject("Popup_Message", typeof(ErosFloatingText)), new Vector3(x, 0, z) * cellSize + new Vector3(cellSize * .5f, .1f, cellSize * .5f), Quaternion.identity);
                    _popupMessage.transform.LookAt(cam.transform.position);
                    _popupMessage.GetComponent<ErosFloatingText>().text = "This position is occuped";
                }
            }
            //ErosText.CreateTextMesh("x", null, 40, TextAnchor.MiddleCenter, TextAlignment.Center, Color.cyan, new Vector3(x, 0, z+1)*cellSize+ new Vector3(cellSize*.5f, 0, -cellSize*.5f));
        }


        //Brush mode implementation

        /*if (!Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButton(0) && brushEnabled)
        {
            //Physics.IgnoreLayerCollision()
            Vector3 point = ErosInput.GetMouseXYZCoords();
            int x, z;
            layers[activeLayer].GetXZ(point, out x, out z);

            if (x < gridWidth && z < gridHeight && x >= 0 && z >= 0)
            {

                if (!gridData[x, z])
                {
                    float r = 1f / gridWidth;
                    float b = 1f / gridHeight;

                    Material source = new Material(src);
                    source.color = new Color(r * x, 1f * (1f / (x + z + .1f)), b * z);
                    Debug.Log(source.color.ToString());
                    gridObjects[x, z] = Instantiate(instance, new Vector3(x, origin + .5f, z) * cellSize + new Vector3(cellSize * .5f, .1f, cellSize * .5f), Quaternion.identity);
                    gridObjects[x, z].GetComponent<MeshRenderer>().material = source;
                    gridData[x, z] = true;
                }
            }
            //ErosText.CreateTextMesh("x", null, 40, TextAnchor.MiddleCenter, TextAlignment.Center, Color.cyan, new Vector3(x, 0, z+1)*cellSize+ new Vector3(cellSize*.5f, 0, -cellSize*.5f));
        }*/

        if (!Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButtonDown(1))
        {
            inactiviteTimer = 20f;
            Vector3 point = ErosInput.GetMouseXYZCoords();
            int x, z;
            layers[activeLayer].GetXZ(point, out x, out z);

            if (x < gridWidth && z < gridHeight && x >= 0 && z >= 0)
            {

                if (gridData[x, z])
                {
                    Destroy(layers[activeLayer][x, z]);
                    gridData[x, z] = false;
                }

                else
                {
                    GameObject _popupMessage = Instantiate(new GameObject("Popup_Message", typeof(ErosFloatingText)), new Vector3(x, 0, z) * cellSize + new Vector3(cellSize * .5f, .1f, cellSize * .5f), Quaternion.identity);
                    _popupMessage.transform.LookAt(cam.transform.position);
                    _popupMessage.GetComponent<ErosFloatingText>().text = "This position is free";
                }
            }
        }

        if(Input.GetMouseButtonDown(2) || (Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButtonDown(0)))
        {
            inactiviteTimer = 20f;
            Vector3 point = ErosInput.GetMouseXZCoords(origin);
            int x, z;
            layers[activeLayer].GetXZ(point, out x, out z);

            if (x < gridWidth && z < gridHeight && x >= 0 && z >= 0)
            {
                camPos = new Vector2(x + cellSize * .5f, z + cellSize * .5f);
                animMoveSpeed = 4f;
                animAngleSpeed = 0.0f;
            }
        }

        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButton(1))
        {
            inactiviteTimer = 20f;
            Vector3 camAngle = cam.transform.eulerAngles;
            yAxisAngle -= Input.GetAxis("Mouse X") * rotSpeed * Time.deltaTime;
        }

        if(Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            inactiviteTimer = 20f;
            float scrollAxis = Input.GetAxis("Mouse ScrollWheel");
            camRadius -= scrollAxis * scrollSpeed * Time.deltaTime;
            
            if(camRadius > 20f)
            {
                camRadius = 20f;
            }

            else if(camRadius < 1f)
            {
                camRadius = 1f;
            }
        }

        //Lerp movement of main camera to follow current layers[activeLayer] unit in mouse cursor
        if(!cam.transform.position.Equals(camPos))
            CameraSnapCheck();

        if(Input.GetKeyDown(KeyCode.B))
        {
            inactiviteTimer = 20f;
            BrushToogle();
        }

        if(Input.GetKeyDown(KeyCode.G))
        {
            inactiviteTimer = 20f;
            layers[activeLayer].ToggleGizmosEnabled();
        }
    }

    private void CameraSnapCheck()
    {
        cam.transform.position = new Vector3(Mathf.Lerp(cam.transform.position.x, camPos.x + camRadius * Mathf.Cos(yAxisAngle), Time.deltaTime*animMoveSpeed),
                                             3,
                                             Mathf.Lerp(cam.transform.position.z, camPos.y + camRadius * Mathf.Sin(yAxisAngle), Time.deltaTime*animMoveSpeed));

        _object.transform.position = cam.transform.position;

        _object.transform.LookAt(new Vector3(camPos.x, origin, camPos.y));

        StartCoroutine(SlowAngleAnimSpeed());

        if(animAngleSpeed > 5f && inactiviteTimer >= .01f)
        {
            cam.transform.eulerAngles = new Vector3(_object.transform.eulerAngles.x,
            Mathf.Lerp(cam.transform.eulerAngles.y, _object.transform.eulerAngles.y, Time.deltaTime * animAngleSpeed),
                                                    _object.transform.eulerAngles.z);
        }
    }

    IEnumerator SlowAngleAnimSpeed()
    {
        yield return new WaitForSeconds(.4f);

        animAngleSpeed = 250f;
    }

    public Vector3 GetCameraTransform()
    {
        return cam.transform.position;
    }

    private void BrushToogle()
    {
        brushEnabled = !brushEnabled;
        if (brushEnabled)
        {
            GameObject _popupMessage = Instantiate(new GameObject("Popup_Message", typeof(ErosFloatingText)), ErosInput.GetMouseXZCoords(origin), Quaternion.identity);
            _popupMessage.GetComponent<ErosFloatingText>().text = "Brush enabled";
            _popupMessage.layer = 5;
            return;
        }

        else
        {
            GameObject _popupMessage = Instantiate(new GameObject("Popup_Message", typeof(ErosFloatingText)), ErosInput.GetMouseXZCoords(origin), Quaternion.identity);
            _popupMessage.GetComponent<ErosFloatingText>().text = "Brush disabled";
            _popupMessage.layer = 5;
            return;
        }
    }
}
