using Microsoft.Xna.Framework;

namespace Machina;

public class Transform
{
    private Transform? _parent = null!;
    private Matrix? _cachedMatrix;
    private List<Transform> _children = new();

    private float _localAngle;
    private Vector2 _localPosition;
    private float _localScale = 1f;

    public void AdoptChild(Transform child)
    {
        child._parent = this;
        child.ClearCachedMatrix();
    }

    public Matrix TransformMatrix
    {
        get
        {
            if (_cachedMatrix.HasValue)
            {
                return _cachedMatrix.Value;
            }

            var parentMatrix = Matrix.Identity;
            if (_parent != null)
            {
                parentMatrix = _parent.TransformMatrix;
            }

            _cachedMatrix =
                parentMatrix
                * Matrix.CreateRotationZ(LocalAngle)
                * Matrix.CreateTranslation(new Vector3(LocalPosition, 0))
                * Matrix.CreateScale(LocalScale);

            return _cachedMatrix.Value;
        }
    }

    public float LocalScale
    {
        get => _localScale;
        set
        {
            ClearCachedMatrix();
            _localScale = value;
        }
    }

    public Vector2 LocalPosition
    {
        get => _localPosition;
        set
        {
            ClearCachedMatrix();
            _localPosition = value;
        }
    }

    public float LocalAngle
    {
        get => _localAngle;
        set
        {
            ClearCachedMatrix();
            _localAngle = value;
        }
    }

    private void ClearCachedMatrix()
    {
        _cachedMatrix = null;
        foreach (var child in _children)
        {
            child.ClearCachedMatrix();
        }
    }
}
