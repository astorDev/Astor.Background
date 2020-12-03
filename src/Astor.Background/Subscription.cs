using Astor.Background.Abstractions;

namespace Astor.Background
{
    public class Subscription
    {
        public SubscribedOnAttribute Attribute { get; }
        public Action Action { get; }

        public Subscription(SubscribedOnAttribute attribute, Action handler)
        {
            this.Attribute = attribute;
            this.Action = handler;
        }
    }
}