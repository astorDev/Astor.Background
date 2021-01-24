using GreenPipes;

namespace Astor.Background
{
    public class EventContext : BasePipeContext, PipeContext
    {
        public Action Action { get; }
        public Input Input { get; }
        public ActionResult ActionResult { get; } = new();

        public EventContext(Action action, Input input)
        {
            this.Action = action;
            this.Input = input;
        }
    }
}