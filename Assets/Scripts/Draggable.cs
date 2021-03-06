﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    
    public GameObject TrapToCreate = null;
    public int cost;

    private Camera _camera = null;
    private GameObject _trapIndicator;
    
	public void Update () {
	    if (RoundHandler.roundHandler)
	    {
            if (RoundHandler.roundHandler.IsFiring)
            {
                GetComponent<CanvasGroup>().alpha = 0.5f;
            }
            else
            {
                GetComponent<CanvasGroup>().alpha = 1;
            }
	    }
	}

    public void OnBeginDrag(PointerEventData eventData) {
        // Create an indicator of where the trap will land
        _trapIndicator = CreateOnPosition(eventData.position);
        _trapIndicator.GetComponent<Trap>().Deactivate();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RoundHandler.roundHandler.IsFiring)
        {
            return;
        }
        
        // Have trap indicator follow under draggable
        if (_trapIndicator != null)
        {
            Vector3 trapPosition = _trapIndicator.transform.position;
            Vector3 ownPosition = _camera.ScreenToWorldPoint(eventData.position);
            _trapIndicator.transform.position = new Vector3(ownPosition.x, trapPosition.y, trapPosition.z);
        }

        if (canPlace())
        {
            _trapIndicator.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, .5f);
        }
        else
        {
            _trapIndicator.GetComponent<SpriteRenderer>().color = new Color(1f, 0.3f, 0.3f, .5f);
        }
    }

    public bool canPlace()
    {
        var trapLayer = LayerMask.GetMask("Trap");
        return !_trapIndicator.GetComponent<BoxCollider2D>().IsTouchingLayers(trapLayer);
    }

    public void OnEndDrag(PointerEventData eventData) {
        Debug.Log("OnEndDrag");

        GetComponent<CanvasGroup>().blocksRaycasts = true;

        if (canPlace() && RoundHandler.gold > cost)
        {
            CreateOnPosition(eventData.position);

            // Update current gold
            RoundHandler.gold = RoundHandler.gold - cost;
        }
        Destroy(_trapIndicator);

    }

    public GameObject CreateOnPosition(Vector2 position)
    {
        Debug.Log("Creating object");
        if (_camera == null)
        {
            _camera = Camera.main;
        }
        Vector3 point = _camera.ScreenToWorldPoint(position);
        point = new Vector3(point.x, point.y, 0);
        return Instantiate(TrapToCreate, point, Quaternion.identity);
    }
}
