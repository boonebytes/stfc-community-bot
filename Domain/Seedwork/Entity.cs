using System;
using System.Collections.Generic;
using System.Linq;
using DiscordBot.Domain.Events;
using MediatR;

namespace DiscordBot.Domain.Seedwork
{
    public abstract class Entity
    {
        // Property is set as Virtual, and setter is protected
        // instead of private, for NHibernate support
        public virtual long Id { get; protected set; }
        
        public virtual string ModifiedBy { get; protected set; }
        public virtual DateTime ModifiedDate { get; protected set; }
        
        private List<DomainEvent> _domainEvents;
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents?.AsReadOnly();

        protected Entity()
        {
        }

        protected Entity(long id)
        {
            Id = id;
        }

        public bool IsTransient()
        {
            return this.Id == default(long);
        }

        public void AddDomainEvent(DomainEvent eventItem)
        {
            _domainEvents = _domainEvents ?? new List<DomainEvent>();
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(DomainEvent eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }

        public void ClearDomainEvents(DomainEventType domainEventType)
        {
            if (_domainEvents != null && _domainEvents.Count > 0)
            {
                _domainEvents.RemoveAll(e => e.EventType == domainEventType);
            }
            //_domainEvents?.Clear();
        }

        public override bool Equals(object obj)
        {
            if (obj is not Entity other)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetUnproxiedType(this) != GetUnproxiedType(other))
                return false;

            if (Id.Equals(default) || other.Id.Equals(default))
                return false;

            return Id.Equals(other.Id);
        }

        public static bool operator ==(Entity a, Entity b)
        {
            if (a is null && b is null)
                return true;

            if (a is null || b is null)
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(Entity a, Entity b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return (GetUnproxiedType(this).ToString() + Id).GetHashCode();
        }

        internal static Type GetUnproxiedType(object obj)
        {
            const string EFCoreProxyPrefix = "Castle.Proxies.";
            const string NHibernateProxyPrefix = "Proxy";

            Type type = obj.GetType();
            string typeString = type.ToString();

            if (typeString.Contains(EFCoreProxyPrefix) || typeString.EndsWith(NHibernateProxyPrefix))
                return type.BaseType;

            return type;
        }
    }
}
