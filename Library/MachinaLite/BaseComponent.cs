using System.Diagnostics;

namespace MachinaLite
{
    public interface IComponent : ICrane
    {
        /// <summary>
        ///     Called when an actor is Destroyed
        /// </summary>
        public void OnActorDestroy();
    }

    /// <summary>
    ///     Base class for all components. This lives in Machina.Components even though it's in the engine folder
    ///     because it's referenced by more Components than it is Engine things. It lives in the Engine folder to
    ///     make it clear this is not a normal component.
    /// </summary>
    public abstract class BaseComponent : NonIteratingCrane, IComponent
    {
        public readonly Actor Actor;

        protected BaseComponent(Actor actor)
        {
            Actor = actor;
            // THIS IS THE ONE TIME IT'S OKAY TO CALL ADD COMPONENT, ALL OTHER TIMES ARE FORBIDDEN
            Actor.AddComponent(this);
        }

        public Transform Transform => Actor.Transform;

        public virtual void OnActorDestroy()
        {
        }

        protected T RequireComponent<T>() where T : BaseComponent
        {
            var component = Actor.GetComponent<T>();
            if (component == null)
            {
                throw new Exception("Missing component " + typeof(T).FullName);
            }

            return component;
        }

        public override string ToString()
        {
            return Actor + "." + GetType().Name;
        }
    }
}