using App.Core.Common.LogModels;

namespace App.Core.Interfaces;

public interface IRequestResponseLogCreator {
    RequestResponseLog Log { get; }
    string LogString();
}