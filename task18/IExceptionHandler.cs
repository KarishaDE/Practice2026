namespace task18;

public interface IExceptionHandler
{
    void Handle(ICommand command, Exception exception);
}
