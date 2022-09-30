using System.Diagnostics;
using ExplogineMonoGame;

namespace MachinaLite
{
    public class Actor : Crane<IComponent>
    {
        public readonly string Name;
        public readonly Scene Scene;
        public readonly Transform Transform;
        private bool _isDirectlyVisible;

        /// <summary>
        ///     Create an actor and add them to the given scene.
        /// </summary>
        /// <param name="name">Human readable name (for debugging)</param>
        /// <param name="scene">Scene that the ctor will add the actor to. Should not be null unless you're a test.</param>
        public Actor(string name, Scene scene)
        {
            Scene = scene;
            Name = name;

            Visible = true;
            Transform = new Transform(this);

            // Transform is "Just" a component, it just happens to be the first component and is added to every actor
            // Niche scenario, AddComponent is OK here.
            AddComponent(Transform);
        }

        public bool Visible
        {
            set
            {
                _isDirectlyVisible = value;
            }
            get
            {
                var parentIsVisible = true;
                if (Transform.Parent != null)
                {
                    parentIsVisible = Transform.Parent.Actor.Visible;
                }
                return _isDirectlyVisible && parentIsVisible;
            }
        }

        public bool IsDestroyed { get; private set; }

        public event Action? Destroyed;
        public event Action? Deleted;

        public override void Draw(Painter painter)
        {
            if (Visible)
            {
                base.Draw(painter);
            }
        }

        public override void PreDraw(Painter painter)
        {
            if (Visible)
            {
                base.PreDraw(painter);
            }
        }

        public T? GetComponentInImmediateParent<T>() where T : BaseComponent
        {
            var parent = Transform.Parent;
            if (parent == null)
            {
                return null;
            }

            return parent.Actor.GetComponent<T>();
        }

        public List<T> GetComponentsInImmediateChildren<T>() where T : BaseComponent
        {
            var result = new List<T>();
            for (var i = 0; i < Transform.ChildCount; i++)
            {
                var child = Transform.ChildAt(i);
                var comp = child?.GetComponent<T>();
                if (comp != null)
                {
                    result.Add(comp);
                }
            }

            return result;
        }

        public void Destroy()
        {
            foreach (var component in Iterables)
            {
                component.OnActorDestroy();
            }

            Destroyed?.Invoke();
            Scene.DeleteActor(this);
            IsDestroyed = true;
        }

        public void Delete()
        {
            // We make actors invisible while we're trying to delete them
            // Sometimes they'll linger for a frame... can we fix that?
            Visible = false;
            Deleted?.Invoke();
            Scene.DeleteActor(this);
        }

        /// <summary>
        ///     SHOULD NOT BE CALLED DIRECTLY UNLESS YOU'RE IN A UNIT TEST
        ///     If you want to add a component call `new YourComponentName(actor);`
        /// </summary>
        /// <param name="component">The component who is being constructed</param>
        /// <returns></returns>
        internal IComponent AddComponent(IComponent component)
        {
            if (component == null)
            {
                throw new ArgumentNullException(nameof(component));
            }

            var type = component.GetType();
            if (type.FullName == null || GetComponentByName(type.FullName) != null)
            {
                throw new("Attempted to add component that already exists " + type.FullName);
            }

            // TODO: This should be AddIterable so we can AddComponent during an update, but then we can't assemble everything on frame 0 and RequireComponent doesn't work
            Iterables.Add(component);
            return component;
        }

        /// <summary>
        ///     Acquire a component of type T if it exists. Otherwise get null.
        /// </summary>
        /// <typeparam name="T">Component that inherits from IComponent</typeparam>
        /// <returns></returns>
        public T? GetComponent<T>() where T : BaseComponent
        {
            return GetComponentUnsafe<T>();
        }

        public IEnumerable<T> GetComponents<T>() where T : BaseComponent
        {
            return GetComponentsUnsafe<T>();
        }

        public void RemoveComponent<T>() where T : BaseComponent
        {
            var comp = GetComponent<T>();
            if (comp != null)
            {
                DeleteIterable(comp);
            }
        }

        private IComponent? GetComponentByName(string fullName)
        {
            foreach (var component in Iterables)
            {
                if (component.GetType().FullName == fullName)
                {
                    return component;
                }
            }

            return null;
        }

        public override string ToString()
        {
            var parentName = "";
            if (Transform.HasParent)
            {
                parentName = Transform.Parent!.Actor + "/";
            }

            return parentName + Name;
        }

        /// <summary>
        ///     Same as GetComponents except T can be any type
        /// </summary>
        /// <typeparam name="T">Any type the component qualifies under</typeparam>
        /// <returns></returns>
        public List<T> GetComponentsUnsafe<T>()
        {
            var result = new List<T>();
            foreach (var component in Iterables)
            {
                if (component is T converted)
                {
                    result.Add(converted);
                }
            }

            return result;
        }

        /// <summary>
        ///     Same as GetComponent except without the type safety
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T? GetComponentUnsafe<T>() where T : class
        {
            foreach (var component in Iterables)
            {
                if (component is T converted)
                {
                    return converted;
                }
            }

            return null;
        }

        public T? GetFirstComponentInProgeny<T>() where T : BaseComponent
        {
            for (var i = 0; i < Transform.ChildCount; i++)
            {
                var child = Transform.ChildAt(i);
                var comp = child?.GetComponent<T>();
                if (comp != null)
                {
                    return comp;
                }

                if (child != null && child.Transform.ChildCount > 0)
                {
                    var comp2 = child.GetFirstComponentInProgeny<T>();
                    if (comp2 != null)
                    {
                        return comp2;
                    }
                }
            }

            return null;
        }

        public static bool AreAllActorsDestroyed(List<Actor> actors)
        {
            foreach (var actor in actors)
            {
                if (!actor.IsDestroyed)
                {
                    return false;
                }
            }

            return true;
        }
    }
}