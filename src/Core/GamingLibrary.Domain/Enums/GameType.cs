// src/Core/GamingLibrary.Domain/Enums/GameType.cs
// Purpose: Enumeration of available games
namespace GamingLibrary.Domain.Enums
{
    /// <summary>
    /// Available games in the Gaming Library.
    /// Used for session tracking and game selection.
    /// </summary>
    public enum GameType
    {
        /// <summary>
        /// "Deploy the Cat" - CI/CD Pipeline game with cat interference
        /// Focus: DevOps knowledge, deployment processes
        /// </summary>
        DeployTheCat = 1,
        
        /// <summary>
        /// "Git Blaster" - Git command shooter game  
        /// Focus: Git knowledge, reaction time, version control mastery
        /// </summary>
        GitBlaster = 2
    }
}
