using System;

namespace TypeTooling
{
    public class UserViewableException(string message) : Exception(message)
    {
    }
}
