﻿namespace SecureAPIs.Helpers
{
    public interface IJwtTokenService
    {
        string GenerateToken(string username, string role);
    }
}
