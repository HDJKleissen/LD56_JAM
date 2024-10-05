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
    [SerializeField] private float _maxRadius;
    [SerializeField] private float _shrinkDuration;
    [SerializeField] private LayerMask _groundMask;

    Tween shrinkTween;
    float radius;

    List<NPCMouseController> selectedMice = new List<NPCMouseController>();

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
            EndSelection();
        }

        if (Input.GetMouseButtonUp(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Vector3 clickLocation;

            if (Physics.Raycast(ray, out RaycastHit hitData, float.PositiveInfinity, _groundMask))
            {
                clickLocation = hitData.point;

                Vector2[] locationInCircle = Sunflower(selectedMice.Count);

                for(int i = 0; i < selectedMice.Count; i++)
                {
                    NPCMouseController mouse = selectedMice[i];
                    mouse.SetDestination(clickLocation + new Vector3(locationInCircle[i].x, 0, locationInCircle[i].y) * Mathf.Min(selectedMice.Count / (Mathf.PI * 2), 2));
                }
            }
        }
    }

    private void SelectMouse(NPCMouseController mouse)
    {
        mouse.SetSelected(true);
        selectedMice.Add(mouse);
    }

    private void ClearMouseList()
    {
        foreach (NPCMouseController mouse in selectedMice)
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
                radius = 0;
            });
    }

    private void UpdateRings(RaycastHit hit)
    {
        Vector3 position = hit.point;
        
        _baseRing.transform.position = position + new Vector3(0, 0.01f, 0);
        _timeRing.transform.position = position + new Vector3(0, 0.02f, 0);

        if (radius < _maxRadius)
        {
            radius += _growSpeed * Time.deltaTime;
        }
        _baseRing.Radius = radius;
        _timeRing.Radius = radius;

        _timeRing.AngRadiansEnd = (radius / _maxRadius) * 2 * Mathf.PI;

        transform.up = hit.normal;
    }

    Vector2[] Sunflower(int n, float alpha = 0, bool geodesic = false)
    {
        float phi = (1 + Mathf.Sqrt(5)) / 2;//golden ratio
        float angle_stride = 360 * phi;
        float radius(float k, float n, float b)
        {
            return k > n - b ? 1 : Mathf.Sqrt(k - 0.5f) / Mathf.Sqrt(n - (b + 1) / 2);
        }

        int b = (int)(alpha * Mathf.Sqrt(n));  //# number of boundary points

        List<Vector2> points = new List<Vector2>();
        for (int k = 0; k < n; k++)
        {
            float r = radius(k, n, b);
            float theta = geodesic ? k * 360 * phi : k * angle_stride;
            float x = !float.IsNaN(r * Mathf.Cos(theta)) ? r * Mathf.Cos(theta) : 0;
            float y = !float.IsNaN(r * Mathf.Sin(theta)) ? r * Mathf.Sin(theta) : 0;
            points.Add(new Vector2(x, y));
        }
        return points.ToArray();
    }
}