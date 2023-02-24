using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direct { Forward, Back, Right, Left, None}
public class Player : MonoBehaviour
{
    public LayerMask layerBrick;
    public float speed = 5;

    public Transform playerBrickPrefab;
    public Transform brickHolder;

    public Transform playerSkin;

    private Vector3 mouseDown, mouseUp;
    private bool isMoving;
    private bool isControl;
    private Vector3 moveNextPoint;
    private List<Transform> playerBricksList = new List<Transform>();


    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.IsState(GameState.Gameplay) && !isMoving)
        {
            if (Input.GetMouseButtonDown(0) && !isControl)
            {
                isControl = true;
                mouseDown = Input.mousePosition;
            }
            if (Input.GetMouseButtonUp(0) && isControl)
            {
                isControl = false;
                mouseUp = Input.mousePosition;
                Direct direct = GetDirect(mouseDown, mouseUp);
                if (direct != Direct.None)
                {
                    moveNextPoint = GetNextPoint(GetDirect(mouseDown, mouseUp));
                    isMoving = true;
                }
            }
        }
        else if (isMoving)
        {
            if (Vector3.Distance(transform.position, moveNextPoint) < 0.1f)
            {
                isMoving = false;
            }
            transform.position = Vector3.MoveTowards(transform.position, moveNextPoint, Time.deltaTime * speed);
        }
    }
    public void OnInit()
    {
        isMoving = false;
        isControl = false;
        ClearBrick();
        playerSkin.localPosition = Vector3.zero;
    }
    private Direct GetDirect(Vector3 mouseDown, Vector3 mouseUp)
    {
        Direct direct = Direct.None;

        float deltaX = mouseUp.x - mouseDown.x;
        float deltaY = mouseUp.y - mouseDown.y;

        if (Vector3.Distance(mouseDown, mouseUp) < 100)
        {
            direct = Direct.None;
        }
        else
        {
            if (Mathf.Abs(deltaY) > Mathf.Abs(deltaX))
            {
                // tren
                if (deltaY > 0)
                {
                    direct = Direct.Forward;
                }
                // duoi
                else
                {
                    direct = Direct.Back;
                }
            }
            else
            {
                // phai
                if (deltaX > 0)
                {
                    direct = Direct.Right;
                }
                // trai
                else
                {
                    direct = Direct.Left;
                }
            }
        }
        return direct;
    }
    private Vector3 GetNextPoint(Direct direct)
    {
        RaycastHit hit;
        Vector3 nextPoint = transform.position;
        Vector3 dir = Vector3.zero;

        switch (direct)
        {
            case Direct.Forward:
                dir = Vector3.forward;
                break;
            case Direct.Back:
                dir = Vector3.back;
                break;
            case Direct.Left:
                dir = Vector3.left;
                break;
            case Direct.Right:
                dir = Vector3.right;
                break;
        }

        for (int i = 1; i < 100; i++)
        {
            if (Physics.Raycast(transform.position + dir * i + Vector3.up * 2, Vector3.down, out hit, 10f, layerBrick))
            {
                nextPoint = hit.collider.transform.position;
            }
            else
            {
                break;
            }
        }
        return nextPoint;
    }
    public void AddBrick()
    {
        int index = playerBricksList.Count;
        Transform playerBrick = Instantiate(playerBrickPrefab, brickHolder);
        playerBrick.localPosition = Vector3.down + index * 0.25f * Vector3.up;

        playerBricksList.Add(playerBrick);

        playerSkin.localPosition = playerSkin.localPosition + Vector3.up * 0.25f;
    }
    public void RemoveBrick()
    {
        int index = playerBricksList.Count - 1;
        if (index >= 0)
        {
            Transform playerBrick = playerBricksList[index];
            playerBricksList.RemoveAt(index);
            Destroy(playerBrick.gameObject);

            playerSkin.localPosition = playerSkin.localPosition - Vector3.up * 0.25f;
        }
    }
    public void ClearBrick()
    {
        for (int i = 0; i < playerBricksList.Count; i++)
        {
            Destroy(playerBricksList[i].gameObject);
        }
        playerBricksList.Clear();
    }
}
