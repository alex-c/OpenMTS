using System;

namespace OpenMTS.Services.Authentication
{
    /// <summary>
    /// Indicates that the data supplied for authentication is incomplete or faulty in some was. Is caught in the
    /// authentication controller to return a '400 Bad request' error. This exception does NOT mean that well-
    /// formed data was supplied but authentication failed because of something like a wrong password.
    /// </summary>
    public class MalformedAuthenticationDataException : Exception { }
}
