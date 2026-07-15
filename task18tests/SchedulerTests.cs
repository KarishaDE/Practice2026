using task18;

namespace task18tests;

public class SchedulerTests
{
    [Fact]
    public void RoundRobinScheduler_SelectsCommandsInCycle()
    {
        var first = new ActionCommand(() => { });
        var second = new ActionCommand(() => { });
        var scheduler = new RoundRobinScheduler();
        scheduler.Add(first);
        scheduler.Add(second);

        var selectedFirst = scheduler.Select();
        scheduler.Add(selectedFirst);
        var selectedSecond = scheduler.Select();
        scheduler.Add(selectedSecond);

        Assert.Same(first, selectedFirst);
        Assert.Same(second, selectedSecond);
        Assert.Same(first, scheduler.Select());
    }

    [Fact]
    public void RoundRobinScheduler_SelectFromEmptyScheduler_ThrowsException()
    {
        var scheduler = new RoundRobinScheduler();

        Assert.False(scheduler.HasCommand());
        Assert.Throws<InvalidOperationException>(() => scheduler.Select());
    }

    private sealed class ActionCommand : ICommand
    {
        private readonly Action _action;

        public ActionCommand(Action action)
        {
            _action = action;
        }

        public void Execute()
        {
            _action();
        }
    }
}
