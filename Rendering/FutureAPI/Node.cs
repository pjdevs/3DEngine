namespace gl.Rendering
{
    public class Node : IDisposable
    {
        private readonly List<Node> _children;

        public Transform Transform;

        public Node? Parent { get; private set; }
        public Scene? Scene { get; internal set; }

        public Node(Node? parent)
        {
            _children = new List<Node>();

            Parent = parent;
            Parent?.AddChild(this);
        }

        public Node()
            : this(null)
        {

        }

        public void AddChild(Node child)
        {
            _children.Add(child);

            if (Scene != null)
            {
                child.Scene = Scene;

                foreach (var childChild in child._children)
                {
                    childChild.Scene = Scene;
                }
            }
        }

        public void Load()
        {
            foreach (var child in _children)
            {
                child.Load();
            }
        }

        public void Render()
        {
            foreach (var child in _children)
            {
                child.Render();
            }
        }

        public void Update(double deltaTime)
        {
            foreach (var child in _children)
            {
                child.Update(deltaTime);
            }
        }

        public void Dispose()
        {
            foreach (var child in _children)
            {
                child.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }
}