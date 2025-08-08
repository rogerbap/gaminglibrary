// src/Core/GamingLibrary.Application/Common/Interfaces/ICommandHandler.cs
// Purpose: Command handler interface
using MediatR;

namespace GamingLibrary.Application.Common.Interfaces
{
    /// <summary>
    /// Handler for commands that return a response
    /// </summary>
    public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
    }

    /// <summary>
    /// Handler for commands without response
    /// </summary>
    public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand>
        where TCommand : ICommand
    {
    }
}