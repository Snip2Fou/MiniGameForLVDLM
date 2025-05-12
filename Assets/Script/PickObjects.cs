using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickObjects : MonoBehaviour
{
    private GameObject hoveredObj;
    private Camera targetCamera;

    [Header("Reference")]
    [SerializeField] private GameObject pickedObjCanvas;
    [SerializeField] private InputActionReference pickAction;
    [SerializeField] private PlayerInventory playerInventory;

    [Header("Game Settings")]
    [SerializeField] private int score;
    [SerializeField] private float timer = 60f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timerText;

    private void Start()
    {
        targetCamera = Camera.main;
        StartCoroutine(MiniGameTimer());
    }

    private void OnEnable()
    {
        pickAction.action.Enable();
        pickAction.action.performed += PickObject;
    }

    private void OnDisable()
    {
        pickAction.action.performed -= PickObject;
        pickAction.action.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, 5f))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.red);
            if (hit.collider.CompareTag("Pickeable"))
            {
                pickAction.action.Enable();
                if (hoveredObj != hit.collider.gameObject)
                {
                    InitNewHoveredObj(hit.collider.gameObject);
                }
                else
                {
                    UpdateHoveredObj();
                }
            }
            else if (hoveredObj != null)
            {
                pickAction.action.Disable();
                if (hoveredObj.TryGetComponent<Outline>(out Outline existing))
                {
                    Destroy(existing);
                }
                hoveredObj = null;
                pickedObjCanvas.transform.parent = null;
                pickedObjCanvas.SetActive(false);
            }
        }
    }

    private void InitNewHoveredObj(GameObject _newObj)
    {
        if (hoveredObj != null && hoveredObj.TryGetComponent<Outline>(out Outline existing))
        {
            Destroy(existing);
        }
        hoveredObj = _newObj;
        if(!hoveredObj.TryGetComponent<Outline>(out Outline outline))
        {
            outline = hoveredObj.AddComponent<Outline>();
        }
        outline.OutlineWidth = 5f;
        pickedObjCanvas.transform.parent = hoveredObj.transform;
        pickedObjCanvas.transform.position = hoveredObj.transform.position;
        pickedObjCanvas.transform.localPosition = new Vector3(0, 0.5f, 0);
        pickedObjCanvas.SetActive(true);
        UpdateHoveredObj();
    }

    private void UpdateHoveredObj()
    {
        pickedObjCanvas.transform.LookAt(pickedObjCanvas.transform.position + targetCamera.transform.rotation * Vector3.forward,
                         targetCamera.transform.rotation * Vector3.up);
    }

    private void PickObject(InputAction.CallbackContext context)
    {
        if (hoveredObj != null)
        {
            pickedObjCanvas.transform.parent = null;
            pickedObjCanvas.SetActive(false);
            if(hoveredObj.TryGetComponent<Item>(out Item item))
            {
                score += item.point;
                playerInventory.AddItem(item);
            }
            DestroyImmediate(hoveredObj);
            hoveredObj = null;
            scoreText.text = "Score : " + score;
        }
    }

    private IEnumerator MiniGameTimer()
    {
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            timerText.text = Mathf.FloorToInt(timer / 60).ToString() + ":" + Mathf.FloorToInt(timer % 60).ToString("00");
            yield return null;
        }
        timerText.text = "0:00";
    }
}
