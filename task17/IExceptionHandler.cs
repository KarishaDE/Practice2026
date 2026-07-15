namespace task17;

public interface IExceptionHandler
{
    void Handle(ICommand command, Exception exception);
}
