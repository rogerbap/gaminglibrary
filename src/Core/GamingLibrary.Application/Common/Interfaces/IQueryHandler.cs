// src/Core/GamingLibrary.Application/Common/Interfaces/IQueryHandler.cs
// Purpose: Query handler interface
using MediatR;

namespace GamingLibrary.Application.Common.Interfaces
{
    /// <summary>
    /// Handler for queries
    /// </summary>
    public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
    }
}