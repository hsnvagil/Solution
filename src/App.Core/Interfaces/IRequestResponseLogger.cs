namespace App.Core.Interfaces;

public interface IRequestResponseLogger {
    void Log(IRequestResponseLogCreator logCreator);
}