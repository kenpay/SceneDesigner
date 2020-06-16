using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class ShowToolbox : MonoBehaviour
{
    public List<GameObject> buttons;
    public GameObject panel;
    public aRTapToPlace aRTapToPlace;
    GameObject dragging;
    Vector3 lastPoint;
    Quaternion objectRotation;

    void SetButtonsState(bool state)
    {
        for (int index = 0; index <= 2; index++)
            buttons[index].SetActive(state);
    }

    public void Show()
    {
        SetButtonsState(false);
        panel.SetActive(true);
    }

    private void Hide()
    {
        SetButtonsState(true);
        panel.SetActive(false);
    }

    public void SetObjectTOInstantiate(GameObject selectedPrefab)
    {
        aRTapToPlace.gameObjectToInstantiate = selectedPrefab;
    }

    private bool isFurnitureObject(Transform obj)
    {
        Transform contentFurniture = panel.transform.GetChild(0).GetChild(0);
        for (int index = 0; index < contentFurniture.childCount; index++)
            if (contentFurniture.GetChild(index) == obj)
                return true;
        return false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastPoint = Input.mousePosition;
            if (!RectTransformUtility.RectangleContainsScreenPoint(panel.transform as RectTransform, lastPoint, Camera.main))
                Hide();
            Ray ray = Camera.main.ScreenPointToRay(lastPoint);
            if (Physics.Raycast(ray, out RaycastHit hit) && isFurnitureObject(hit.transform))
            {
                SetObjectTOInstantiate(dragging = hit.transform.gameObject);
                dragging.transform.parent.parent.parent.GetComponent<ScrollRect>().enabled = false;
                objectRotation = hit.transform.localRotation;
            }
        }
        else if(dragging != null)
        {
            if (Input.GetMouseButtonUp(0))
            {
                dragging.transform.localRotation = objectRotation;
                dragging.transform.parent.parent.parent.GetComponent<ScrollRect>().enabled = true;
                dragging = null;
            }
            else
            {
                dragging.transform.Rotate(Vector3.up, lastPoint.x - Input.mousePosition.x, Space.World);
                dragging.transform.Rotate(Vector3.right, lastPoint.y - Input.mousePosition.y, Space.World);
                lastPoint = Input.mousePosition;
            }
        }
    }
}
