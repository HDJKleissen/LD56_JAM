using Shapes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SelectionCircle : MonoBehaviour
{
    [SerializeField] private Disc _baseRing;
    [SerializeField] private Disc _timeRing;
    [SerializeField] private float _growSpeed;
    [SerializeField] private float _maxTime;
    [SerializeField] private float _shrinkDuration;
    [SerializeField] private LayerMask _groundMask;

    Tween shrinkTween;
    float radius;
    float time;

    List<NPCMouseMovement> selectedMice = new List<NPCMouseMovement>();

    // Start is called before the first frame update
    void Start()
    {
        _baseRing.enabled = false;
        _timeRing.enabled = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(_baseRing.transform.position, radius);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (selectedMice.Count > 0)
            {
                ClearMouseList();
            }
        }

        if (Input.GetMouseButton(0) && (shrinkTween == null || !shrinkTween.IsPlaying()))
        {
            if (!_baseRing.enabled)
            {
                _baseRing.enabled = true;
                _timeRing.enabled = true;
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hitData, float.PositiveInfinity, _groundMask))
            {
                UpdateRings(hitData);
                Collider[] hitColliders = Physics.OverlapSphere(_baseRing.transform.position, radius);
                ClearMouseList();
                foreach (Collider hitCollider in hitColliders)
                {
                    NPCMouseMovement mouse = hitCollider.GetComponentInChildren<NPCMouseMovement>();
                    if (mouse)
                    {
                        SelectMouse(mouse);
                    }
                }
            }

            if (time > _maxTime)
            {
                EndSelection();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            EndSelection();
        }

        if (Input.GetMouseButtonUp(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Vector3 clickLocation;

            if (Physics.Raycast(ray, out RaycastHit hitData, float.PositiveInfinity, _groundMask))
            {
                clickLocation = hitData.point;
                foreach (NPCMouseMovement mouse in selectedMice)
                {
                    mouse.SetDestination(clickLocation);
                }
            }
        }
    }

    private void SelectMouse(NPCMouseMovement mouse)
    {
        mouse.SetSelected(true);
        selectedMice.Add(mouse);
    }

    private void ClearMouseList()
    {
        foreach (NPCMouseMovement mouse in selectedMice)
        {
            mouse.SetSelected(false);
        }
        selectedMice.Clear();
    }

    private void EndSelection()
    {
        shrinkTween = DOTween.To(
            () => _baseRing.Radius,
            val =>
            {
                _baseRing.Radius = val;
                _timeRing.Radius = val;
            },
            0,
            _shrinkDuration).OnComplete(() =>
            {
                _baseRing.enabled = false;
                _timeRing.enabled = false;
                time = 0;
                radius = 0;
            });
    }

    private void UpdateRings(RaycastHit hit)
    {
        Vector3 position = hit.point;

        time += Time.deltaTime;

        _baseRing.transform.position = position + new Vector3(0, 0.01f, 0);
        _timeRing.transform.position = position + new Vector3(0, 0.02f, 0);

        radius += _growSpeed * Time.deltaTime;
        _baseRing.Radius = radius;
        _timeRing.Radius = radius;

        _timeRing.AngRadiansEnd = (time / _maxTime) * 2 * Mathf.PI;



        transform.up = hit.normal;
    }
}