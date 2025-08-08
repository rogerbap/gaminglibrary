// src/Core/GamingLibrary.Application/Common/Interfaces/IQuery.cs
// Purpose: CQRS Query interface for read operations
using MediatR;

namespace GamingLibrary.Application.Common.Interfaces
{
    /// <summary>
    /// Marker interface for queries (read-only operations)
    /// </summary>
    /// <typeparam name="TResponse">Response type</typeparam>
    public interface IQuery<out TResponse> : IRequest<TResponse>
    {
    }
}