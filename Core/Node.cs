using System.Numerics;
using Shared;

namespace Core;

public class Node : IDisposable
{
    public delegate void ChildNodeAddedEventHandler(object sender, Node node, Node added);

    public delegate void ChildNodeRemovedEventHandler(object sender, Node node, Node removed);

    readonly List<Node> _children;
    readonly List<Component> _components;
    
    Vector3 _localPosition = Vector3.Zero;
    Quaternion _localRotation = Quaternion.Identity;
    Vector3 _localScale = Vector3.One;

    Node? _parent;
    Matrix4x4? _worldMatrix4X4;

    public Node(
        string name)
    {
        Id = Guid.NewGuid();
        Name = name;
        _components = new List<Component>();
        
        _children = new List<Node>();
    }

    public Vector3 LocalPosition
    {
        get => _localPosition;
        set
        {
            if (_localPosition == value)
                return;

            _localPosition = value;
            ResetWorldMatrix();

        }
    }

    public Quaternion LocalRotation
    {
        get => _localRotation;
        set
        {
            if (_localRotation == value)
                return;

            _localRotation = value;
            ResetWorldMatrix();
        }

    }

    Vector3 LocalScale
    {
        get => _localScale;
        set
        {
            if (_localScale == value)
                return;

            _localScale = value;
            ResetWorldMatrix();
        }
    }

    Matrix4x4 LocalMatrix
    {
        get
        {
            return Matrix4x4.CreateScale(LocalScale) * Matrix4x4.CreateFromQuaternion(LocalRotation) * Matrix4x4.CreateTranslation(LocalPosition);
        }
    }

    public Matrix4x4 WorldMatrix4X4
    {
        get
        {
            _worldMatrix4X4 ??= Parent != null ? LocalMatrix * Parent.WorldMatrix4X4 : LocalMatrix;

            return _worldMatrix4X4.Value;
        }
    }

    public Guid Id { get; }
    public string Name { get; }

    public void AddComponent(Component component)
    {
        component.Connect(this);
        _components.Add(component);
    }
    
    public void RemoveComponent(Component component)
    {
        component.Disconnect();
        _components.Remove(component);
    }

    public Node? Parent
    {
        get => _parent;
        set
        {
            if (_parent != value)
            {
                if (value != null && value.Children.All(c => c.Id != Id))
                {
                    value.AddChild(this);
                }
                else if (value == null)
                {
                    _parent?.RemoveChild(this);
                }

                _parent = value;
            }
        }
    }

    public IReadOnlyCollection<Node> Children => _children;

    void ResetWorldMatrix()
    {
        _worldMatrix4X4 = null;
        foreach (var child in Children)
        {
            child.ResetWorldMatrix();
        }
    }

    public event ChildNodeAddedEventHandler? ChildNodeAdded;
    public event ChildNodeRemovedEventHandler? ChildNodeRemoved;

    public void AddChild(Node child)
    {
        if (Children.All(c => c.Id != child.Id))
        {
            child.Parent?.RemoveChild(child);

            _children.Add(child);

            InvokeOnParentChanged(child);
            child.ResetWorldMatrix();

            foreach (var node in child.Children)
            {
                OnNestedChildNodeAdded(child, node);
            }

            child.ChildNodeAdded += AddHandler;
            child.ChildNodeRemoved += RemoveHandler;

            child.Parent = this;
        }
    }

    public void RemoveChild(Node child)
    {
        if (Children.Any(c => c.Id == child.Id))
        {
            _children.Remove(child);

            OnDirectChildNodeRemoved(child);

            foreach (var node in child.Children)
            {
                OnNestedChildNodeRemoved(child, node);
            }

            child.ChildNodeAdded -= AddHandler;
            child.ChildNodeRemoved -= RemoveHandler;

            child.Parent = null;
        }
    }

    protected void InvokeOnParentChanged(Node added)
    {
        ChildNodeAdded?.Invoke(this, this, added);
    }

    protected void OnNestedChildNodeAdded(Node node, Node added)
    {
        ChildNodeAdded?.Invoke(this, node, added);
    }

    protected void OnDirectChildNodeRemoved(Node removed)
    {
        ChildNodeRemoved?.Invoke(this, this, removed);
    }

    protected void OnNestedChildNodeRemoved(Node node, Node removed)
    {
        ChildNodeRemoved?.Invoke(this, node, removed);
    }

    void AddHandler(object sender, Node node, Node added)
    {
        OnNestedChildNodeAdded(node, added);
    }

    void RemoveHandler(object sender, Node node, Node removed)
    {
        OnNestedChildNodeRemoved(node, removed);
    }

    public void Dispose()
    {
        foreach (var child in Children) 
            child.Dispose();

        foreach (var component in _components) 
            component.Disconnect();
        _components.Clear();
        
        _parent?.RemoveChild(this);
    }
}
