using Shapes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SelectionCircle : MonoBehaviour
{
    [SerializeField] private Disc _baseRing;
    [SerializeField] private float _growSpeed;
    [SerializeField] private float _maxRadius;
    [SerializeField] private float _shrinkDuration;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private PlayerMovement _player;
    Tween shrinkTween;
    float radius;

    List<NPCMouseController> selectedMice = new List<NPCMouseController>();

    // Start is called before the first frame update
    void Start()
    {
        _baseRing.enabled = false;
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
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hitData, float.PositiveInfinity, _groundMask))
            {
                UpdateRings(hitData);
                Collider[] hitColliders = Physics.OverlapSphere(_baseRing.transform.position, radius);
                for (int i = 0; i < selectedMice.Count; i++)
                {
                    NPCMouseController mouse = selectedMice[i];
                    if (mouse == null)
                    {
                        continue;
                    }
                    mouse.SetSelected(false);
                }
                ClearMouseList();
                foreach (Collider hitCollider in hitColliders)
                {
                    NPCMouseController mouse = hitCollider.GetComponentInChildren<NPCMouseController>();
                    if (mouse)
                    {
                        SelectMouse(mouse);
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            for (int i = 0; i < selectedMice.Count; i++)
            {
                NPCMouseController mouse = selectedMice[i];
                if (mouse == null)
                {
                    continue;
                }
                _player.AddFollower(mouse);
            }
            ClearMouseList();
            EndSelection();
        }

    }

    private void SelectMouse(NPCMouseController mouse)
    {
        mouse.SetSelected(true);
        selectedMice.Add(mouse);
    }

    private void ClearMouseList()
    {
        selectedMice.Clear();
    }

    private void EndSelection()
    {
        shrinkTween = DOTween.To(
            () => _baseRing.Radius,
            val =>
            {
                _baseRing.Radius = val;
            },
            0,
            _shrinkDuration).OnComplete(() =>
            {
                _baseRing.enabled = false;
                radius = 0;
            });
    }

    private void UpdateRings(RaycastHit hit)
    {
        Vector3 position = hit.point;
        
        _baseRing.transform.position = position + new Vector3(0, 0.01f, 0);

        if (radius < _maxRadius)
        {
            radius += _growSpeed * Time.deltaTime;
        }
        _baseRing.Radius = radius;
        transform.up = hit.normal;
    }
}