using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public sealed class TiledPlatform : MonoBehaviour
{
    [SerializeField] Mesh _sourceMesh;
    [SerializeField] bool _saveMeshInScene;

    public enum TileMethod { Cube, CircleY }
    [SerializeField] TileMethod _tileMethod;

    [SerializeField] Vector3 _size = Vector3.one;
    [SerializeField] float _uvScale = 1f;
    [SerializeField] Vector2 _uvOffset;
    [SerializeField] bool _needUpdateCollider = true;


    private void OnEnable()
    {
        if (!_saveMeshInScene)
        {
            MeshUpdate();
        }
    }

    private Vector3 GetValidatedSize()
    {
        return (_size - Vector3.one) * 0.5f;
    }

    private Mesh CreateMesh()
    {
        var mesh = Instantiate(_sourceMesh);

        if (!_saveMeshInScene)
        {
            mesh.hideFlags = HideFlags.DontSave;
        }

        return mesh;
    }

    private void ApplyMeshToObject(Mesh mesh)
    {
        var meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        meshFilter.sharedMesh = mesh;

        if (_needUpdateCollider)
        {
            if (TryGetComponent(out MeshCollider meshCollider))
            {
                meshCollider.sharedMesh = mesh;
            }
            else if (TryGetComponent(out BoxCollider boxCollider))
            {
                switch (_tileMethod)
                {
                    case TileMethod.Cube:
                        boxCollider.size = mesh.bounds.size;
                        break;
                    case TileMethod.CircleY:
                        var size = _size;
                        size.z = size.x;
                        boxCollider.size = size;
                        break;
                }
            }
        }
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (!Application.isPlaying)
        {
            MeshUpdate();
        }
    }
#endif

    private void MeshUpdate()
    {
        switch (_tileMethod)
        {
            case TileMethod.Cube: CubePlatform_MeshUpdate(); break;
            case TileMethod.CircleY: CircleYPlatform_MeshUpdate(); break;
        }
    }

    private void CubePlatform_MeshUpdate()
    {
        if (_sourceMesh == null)
            return;

        var validatedSize = GetValidatedSize();
        var mesh = CreateMesh();
        var vertices = mesh.vertices;
        var uv = mesh.uv;
        var normals = mesh.normals;
        var tangents = mesh.tangents;


        for (int i = 0; i < vertices.Length; i++)
        {
            var v = vertices[i];

            v.x += Mathf.Sign(v.x) * validatedSize.x;
            v.y += Mathf.Sign(v.y) * validatedSize.y;
            v.z += Mathf.Sign(v.z) * validatedSize.z;

            vertices[i] = v;
        }

        for (int i = 0; i < uv.Length; i++)
        {
            uv[i] = CubePlatform_ApplySizeForUv(uv[i], validatedSize) * _uvScale + _uvOffset;

#if UNITY_EDITOR
            if (_drawUv)
            {
                Debug.DrawLine(uv[i], new Vector3(uv[i].x, uv[i].y) + (Vector3.forward * 0.03f));
            }
#endif
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.RecalculateBounds();

        ApplyMeshToObject(mesh);
    }

    static Vector2 CubePlatform_ApplySizeForUv(Vector2 uv, Vector3 validatedSize)
    {
        var result = uv;

        if (uv.y > 4.5f) // left
        {
            result.y -= 5f;

            result.x += Mathf.Sign(result.x) * validatedSize.z;
            result.y += Mathf.Sign(result.y) * validatedSize.y;
        }
        else if (uv.y > 3.5f) // forward
        {
            result.y -= 4f;

            result.x += Mathf.Sign(result.x) * validatedSize.x;
            result.y += Mathf.Sign(result.y) * validatedSize.y;
        }
        else if (uv.y > 2.5f) // back
        {
            result.y -= 3f;

            result.x += Mathf.Sign(result.x) * validatedSize.x;
            result.y += Mathf.Sign(result.y) * validatedSize.y;
        }
        else if (uv.y > 1.5f) // down
        {
            result.y -= 2f;

            result.x += Mathf.Sign(result.x) * validatedSize.x;
            result.y += Mathf.Sign(result.y) * validatedSize.z;
        }
        else if (uv.y > 0.5f) // up
        {
            result.y -= 1f;

            result.x += Mathf.Sign(result.x) * validatedSize.z;
            result.y += Mathf.Sign(result.y) * validatedSize.x;
        }
        else if (uv.y > -0.5f)// right
        {
            result.x += Mathf.Sign(result.x) * validatedSize.z;
            result.y += Mathf.Sign(result.y) * validatedSize.y;
        }

        return result;
    }

    private void CircleYPlatform_MeshUpdate()
    {
        if (_sourceMesh == null)
            return;

        var validatedScale = GetValidatedSize();
        var mesh = CreateMesh();
        var vertices = mesh.vertices;
        var uv = mesh.uv;

        

        for (int i = 0; i < uv.Length; i++)
        {
            uv[i] = CircleYPlatform_ApplySizeForUv(uv[i], _size, validatedScale) * _uvScale;

#if UNITY_EDITOR
            if (_drawUv)
            {
                Debug.DrawLine(uv[i], new Vector3(uv[i].x, uv[i].y) + (Vector3.forward * 0.03f));
            }
#endif
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            var v = vertices[i];

            var dir = new Vector3(v.x, 0f, v.z);
            dir.Normalize();
            v += dir * validatedScale.x;

            dir = new Vector3(0f, v.y, 0f);
            dir.Normalize();
            v += dir * validatedScale.y;

            vertices[i] = v;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.RecalculateBounds();

        ApplyMeshToObject(mesh);
    }

    static Vector2 CircleYPlatform_ApplySizeForUv(Vector2 uv, Vector3 size, Vector3 validatedScale)
    {
        if (uv.y > 0.5f) // side
        {
            uv.y -= 1f;

            uv.x *= size.x;
            uv.y += Mathf.Sign(uv.y) * validatedScale.y;
        }
        else // down / up
        {
            uv += uv.normalized * validatedScale.x;
        }

        return uv;
    }

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] bool _drawUv = true;

    private void OnValidate()
    {
        _size.x = Mathf.Max(_size.x, 1f);
        _size.y = Mathf.Max(_size.y, 1f);
        _size.z = Mathf.Max(_size.z, 1f);

        if (Application.isPlaying)
        {
            MeshUpdate();
        }
    }

    [ContextMenu(nameof(FixScale))]
    private void FixScale()
    {
        switch (_tileMethod)
        {
            case TileMethod.Cube:
                _size = Vector3.Scale(_size, transform.localScale);
                transform.localScale = Vector3.one;
                break;
            case TileMethod.CircleY:
                _size.x = Mathf.Max(transform.localScale.x, transform.localScale.z) * _size.x;
                _size.y = transform.localScale.y * _size.y;
                _size.z = 1f;
                transform.localScale = Vector3.one;
                break;
        }

        MeshUpdate();
    }
#endif
}
