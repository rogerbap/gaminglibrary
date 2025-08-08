// src/Core/GamingLibrary.Application/Common/Interfaces/ICommand.cs
// Purpose: CQRS Command interface for operations that change state
using MediatR;

namespace GamingLibrary.Application.Common.Interfaces
{
    /// <summary>
    /// Marker interface for commands (operations that modify state)
    /// </summary>
    /// <typeparam name="TResponse">Response type</typeparam>
    public interface ICommand<out TResponse> : IRequest<TResponse>
    {
    }

    /// <summary>
    /// Command without response
    /// </summary>
    public interface ICommand : IRequest
    {
    }
}
