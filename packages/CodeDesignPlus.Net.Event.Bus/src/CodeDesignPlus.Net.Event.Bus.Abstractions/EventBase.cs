using System.Diagnostics.CodeAnalysis;

namespace CodeDesignPlus.Net.Event.Bus.Abstractions
{
    /// <summary>
    /// EventBase is used to create the events that will be published to the message broker 
    /// </summary>
    public abstract class EventBase : IEquatable<EventBase>
    {
        /// <summary>
        /// Initializes a new instance of the CodeDesignPlus.Event.Bus.Abstractions.EventBase class
        /// </summary>
        protected EventBase()
        {
            this.IdEvent = Guid.NewGuid();
            this.EventDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Gets the id event
        /// </summary>
        public Guid IdEvent { get; set; }
        /// <summary>
        /// Gets the event date
        /// </summary>
        public DateTime EventDate { get; set; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns> true if the current object is equal to the other parameter; otherwise, false.</returns>
        public virtual bool Equals([AllowNull] EventBase other)
        {
            return this.IdEvent == other.IdEvent;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="obj">An object to compare with this object.</param>
        /// <returns> true if the current object is equal to the other parameter; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as EventBase);
        }

        /// <summary>
        /// Diffuses the hash code returned by the specified value.
        /// </summary>
        /// <returns>The hash code that represents the single value.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(this.IdEvent);
        }
    }
}
